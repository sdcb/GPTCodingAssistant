using GPTCodingAssistant.Controllers;
using GPTCodingAssistant.DB;
using GPTCodingAssistant.DB.Helpers;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace GPTCodingAssistant.Services
{
    public class ChatGPTRepository
    {
        private readonly ChatGPTDB _db;

        public ChatGPTRepository(ChatGPTDB db)
        {
            _db = db;
        }

        public Ip GetOrCreateIP(string ipAddress)
        {
            Ip? existing = _db.Ips.FirstOrDefault(x => x.Ip1 == ipAddress);
            if (existing != null)
            {
                return existing;
            }
            else
            {
                Ip ip = new()
                {
                    Ip1 = ipAddress,
                };
                _db.Ips.Add(ip);
                _db.SaveChanges();
                return ip;
            }
        }

        public SessionResponse GetSessionById(int sessionId)
        {
            Session session = _db
                .Sessions
                    .Include(x => x.Ip)
                    .Include(x => x.ChatMessages)
                .Single(x => x.Id == sessionId);

            return new SessionResponse(session.Id, session.Title, session.ChatMessages.Select(x => new ChatMessageResponse(x.Id, x.Message, ChatMessageHelper.ToRole(x.Role).ToString())).ToArray());
        }

        public Ip GetSessionIp(int sessionId)
        {
            return _db.Sessions.Find(sessionId)!.Ip;
        }

        public SessionResponse CreateSession(string clientIp)
        {
            Ip ip = GetOrCreateIP(clientIp);

            int untitledCount = _db.Sessions
                .Count(x => x.Ip == ip && x.Title.StartsWith("无标题"));

            Session session = new Session
            {
                CreateTime = DateTime.Now,
                LastActiveTime = DateTime.Now,
                Ip = ip,
                Title = "无标题" + (untitledCount + 1),
                ChatMessages = new List<ChatMessage>(),
            };
            _db.Sessions.Add(session);
            _db.SaveChanges();

            return GetSessionById(session.Id);
        }

        public void DeleteSession(int sessionId)
        {
            Session session = _db.Sessions.Find(sessionId)!;
            _db.Sessions.Remove(session);
            _db.SaveChanges();
        }

        public SessionSimpleResponse[] GetSessions(string ip)
        {
            return _db.Sessions
                .Where(x => x.Ip.Ip1 == ip)
                .Select(x => new SessionSimpleResponse(x.Id, x.Title))
                .ToArray();
        }

        public IEnumerable<OpenAI_API.Chat.ChatMessage> GetSessionMessagesForAI(int sessionId)
        {
            return _db.ChatMessages
                .Where(x => x.SessionId == sessionId)
                .Select(ChatMessageHelper.ToAI);
        }

        public void AppendMessageToSession(int sessionId, OpenAI_API.Chat.ChatMessageRole role, string input)
        {
            Session session = _db.Sessions.Find(sessionId)!;
            session.LastActiveTime = DateTime.Now;
            session.ChatMessages.Add(new ChatMessage
            {
                Message = input,
                Role = role.ToDB(),
                CreateTime = DateTime.Now,
            });
            _db.SaveChanges();
        }

        public OpenAI_API.Chat.ChatMessage[] GetLatestNChatMessage(int sessionId, int beforeChatMessageId, int n)
        {
            return _db.ChatMessages
                .Where(x => x.SessionId == sessionId && x.Id <= beforeChatMessageId)
                .OrderByDescending(x => x.Id)
                .Take(n)
                .Select(ChatMessageHelper.ToAI)
                .ToArray();
        }

        public void DeleteSessionMessagesAfterInclude(int sessionId, int afterIncludeChatMessageId)
        {
            Session session = _db.Sessions.Find(sessionId)!;
            session.LastActiveTime = DateTime.Now;
            session.ChatMessages = session.ChatMessages.Where(x => x.Id >= afterIncludeChatMessageId).ToList();
            _db.SaveChanges();
        }
    }

    public record SessionSimpleResponse(int SessionId, string Title);

    public record SessionResponse(int SessionId, string Title, ChatMessageResponse[] Messages) : SessionSimpleResponse(SessionId, Title);

    public record ChatMessageResponse(long ChatMessageId, string Message, string Role);
}
