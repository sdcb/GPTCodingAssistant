using GPTCodingAssistant.Controllers;
using GPTCodingAssistant.DB;
using GPTCodingAssistant.DB.Helpers;
using Microsoft.EntityFrameworkCore;

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

        public SessionResponse GetSession(int sessionId)
        {
            Session session = _db
                .Sessions
                    .Include(x => x.Ip)
                    .Include(x => x.ChatMessages)
                .Single(x => x.Id == sessionId);

            return new SessionResponse(session.Id, session.Title, session.ChatMessages.Select(x => new ChatMessageResponse(x.Message, ChatMessageHelper.ToRole(x.Role).ToString())).ToArray());
        }

        public Ip GetSessionIp(int sessionId)
        {
            return _db.Sessions.Find(sessionId)!.Ip;
        }

        public SessionResponse CreateSession(string clientIp)
        {
            Ip ip = GetOrCreateIP(clientIp);
            Session session = new Session
            {
                CreateTime = DateTime.Now,
                LastActiveTime = DateTime.Now,
                Ip = ip,
                Title = "无标题",
                ChatMessages = new List<ChatMessage>(),
            };
            _db.Sessions.Add(session);
            _db.SaveChanges();

            return GetSession(session.Id);
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
    }

    public record SessionSimpleResponse(int SessionId, string Title);

    public record SessionResponse(int SessionId, string Title, ChatMessageResponse[] Messages) : SessionSimpleResponse(SessionId, Title);

    public record ChatMessageResponse(string Message, string Role);
}
