using GPTCodingAssistant.Controllers;
using Microsoft.AspNetCore.SignalR;
using OpenAI_API;
using OpenAI_API.Chat;

namespace GPTCodingAssistant.Hubs
{
    public class ChatHub : Hub
    {
        public ChatHub(IConfiguration config)
        {
            _config = config;
        }

        private static DB _db = DB.Instance;
        private readonly IConfiguration _config;

        public async IAsyncEnumerable<string> Chat(string input)
        {
            OpenAIAPI api = OpenAIAPI.ForAzure(_config["AzureAI:ResourceName"], _config["AzureAI:DeploymentId"], _config["AzureAI:ApiKey"]);
            api.ApiVersion = "2023-03-15-preview";
            Conversation chat = api.Chat.CreateConversation();
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
