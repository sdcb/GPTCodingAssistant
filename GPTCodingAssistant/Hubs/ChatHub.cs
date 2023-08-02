using GPTCodingAssistant.DB;
using Microsoft.AspNetCore.SignalR;
using OpenAI_API;
using OpenAI_API.Chat;
using AI = OpenAI_API.Chat;
using GPTCodingAssistant.Services;

namespace GPTCodingAssistant.Hubs
{
    public class ChatHub : Hub
    {
        public ChatHub(IConfiguration config, ChatGPTRepository db, IPAccessor ipAccessor)
        {
            _config = config;
            _db = db;
            _ipAccessor = ipAccessor;
        }

        private readonly IConfiguration _config;
        private readonly ChatGPTRepository _db;
        private readonly IPAccessor _ipAccessor;

        public IAsyncEnumerable<string> Append(int sessionId, string input)
        {
            CheckIP(sessionId);
            return AppendChatNoValidation(sessionId, input);
        }

        public IAsyncEnumerable<string> RegenerateFor(int sessionId, int assistantMessageId)
        {
            CheckIP(sessionId);
            AI.ChatMessage[] latest2msgs = _db.GetLatestNChatMessage(sessionId, assistantMessageId, 2);
            if (latest2msgs.Length != 2 && latest2msgs[0].Role != ChatMessageRole.User && latest2msgs[1].Role != ChatMessageRole.Assistant)
            {
                throw new InvalidOperationException("Chat message is not assistant's message");
            }

            _db.DeleteSessionMessagesAfterInclude(sessionId, assistantMessageId);
            return AppendChatNoValidation(sessionId, latest2msgs[0].Content);
        }

        public IAsyncEnumerable<string> Edit(int sessionId, int userChatMessageId, string input)
        {
            CheckIP(sessionId);
            AI.ChatMessage? latestMessage = _db.GetLatestNChatMessage(sessionId, userChatMessageId, 1)
                .FirstOrDefault();
            if (latestMessage is null || latestMessage.Role != ChatMessageRole.User)
            {
                throw new InvalidOperationException("Chat message is not user's message");
            }

            _db.DeleteSessionMessagesAfterInclude(sessionId, userChatMessageId);
            return AppendChatNoValidation(sessionId, input);
        }

        private void CheckIP(int sessionId)
        {
            Ip sessionIp = _db.GetSessionIp(sessionId);
            if (sessionIp?.Ip1 != _ipAccessor.GetClientIPAddress())
            {
                throw new InvalidOperationException("Session IP not match client's IP");
            }
        }

        Conversation CreateConversation()
        {
            OpenAIAPI api = OpenAIAPI.ForAzure(_config["AzureAI:ResourceName"], _config["AzureAI:DeploymentId"], _config["AzureAI:ApiKey"]);
            api.ApiVersion = "2023-03-15-preview";
            Conversation chat = api.Chat.CreateConversation();

            return chat;
        }

        async IAsyncEnumerable<string> AppendChatNoValidation(int sessionId, string input)
        {
            Conversation chat = CreateConversation();

            _db.AppendMessageToSession(sessionId, ChatMessageRole.User, input);

            foreach (AI.ChatMessage msg in _db.GetSessionMessagesForAI(sessionId)) chat.AppendMessage(msg);
            await foreach (string c in chat.StreamResponseEnumerableFromChatbotAsync())
            {
                yield return c;
            }
            chat.Messages[^1].Role = ChatMessageRole.Assistant;

            // insert last message
            _db.AppendMessageToSession(sessionId, ChatMessageRole.Assistant, input);
        }
    }
}
