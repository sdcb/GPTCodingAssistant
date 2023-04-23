using GPTCodingAssistant.Controllers;
using GPTCodingAssistant.DB;
using Microsoft.AspNetCore.SignalR;
using OpenAI_API;
using OpenAI_API.Chat;

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

        public async IAsyncEnumerable<string> Chat(string input)
        {
            OpenAIAPI api = OpenAIAPI.ForAzure(_config["AzureAI:ResourceName"], _config["AzureAI:DeploymentId"], _config["AzureAI:ApiKey"]);
            api.ApiVersion = "2023-03-15-preview";
            Conversation chat = api.Chat.CreateConversation();
            _db.ChatMessages.Add(new DB.ChatMessage
            {
                CreateTime = DateTime.Now,
                Message = input,
                Role = (int)ChatMessageRole.User,
            });
            _db.CreateUserMessage(input);
            foreach (ChatMessage msg in _db.Messages)
            {
                chat.AppendMessage(msg);
            }

            await foreach (string c in chat.StreamResponseEnumerableFromChatbotAsync())
            {
                yield return c;
            }
            chat.Messages[^1].Role = ChatMessageRole.Assistant;
            _db.CreateAssistantMessage(chat.Messages[^1].Content);
        }
    }
}
