using GPTCodingAssistant.Controllers;
using GPTCodingAssistant.DB;
using Microsoft.AspNetCore.SignalR;
using OpenAI_API;
using OpenAI_API.Chat;
using GPTCodingAssistant.DB.Helpers;
using Microsoft.EntityFrameworkCore;
using AI = OpenAI_API.Chat;

namespace GPTCodingAssistant.Hubs
{
    public class ChatHub : Hub
    {
        public ChatHub(IConfiguration config, ChatGPTDB db)
        {
            _config = config;
            _db = db;
        }

        private readonly IConfiguration _config;
        private readonly ChatGPTDB _db;

        public async IAsyncEnumerable<string> Chat(int sessionId, string input)
        {
            OpenAIAPI api = OpenAIAPI.ForAzure(_config["AzureAI:ResourceName"], _config["AzureAI:DeploymentId"], _config["AzureAI:ApiKey"]);
            api.ApiVersion = "2023-03-15-preview";
            Conversation chat = api.Chat.CreateConversation();

            Session session = _db.Sessions
                .Include(x => x.ChatMessages)
                .Include(x => x.Ip)
                .Single(x => x.Id == sessionId);
            if (session.Ip.Ip1 != Context.GetHttpContext()?.Connection.RemoteIpAddress?.ToString())
            {
                throw new InvalidOperationException("Session IP not match");
            }

            session.ChatMessages.Add(new DB.ChatMessage
            {
                SessionId = sessionId,
                CreateTime = DateTime.Now,
                Message = input,
                Role = ChatMessageRole.User.ToDB(),
            });
            _db.SaveChanges();

            foreach (AI.ChatMessage msg in session.ChatMessages.Select(ChatMessageHelper.ToAI))
            {
                chat.AppendMessage(msg);
            }

            await foreach (string c in chat.StreamResponseEnumerableFromChatbotAsync())
            {
                yield return c;
            }
            chat.Messages[^1].Role = ChatMessageRole.Assistant;

            // insert last message
            session.ChatMessages.Add(new DB.ChatMessage
            {
                SessionId = sessionId,
                CreateTime = DateTime.Now,
                Message = chat.Messages[^1].Content,
                Role = ChatMessageRole.Assistant.ToDB(),
            });
            _db.SaveChanges();
        }
    }
}
