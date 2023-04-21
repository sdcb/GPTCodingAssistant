using Microsoft.AspNetCore.SignalR;
using OpenAI_API.Chat;

namespace GPTCodingAssistant.Hubs
{
    public class ChatHub : Hub
    {
        public async IAsyncEnumerable<char> Chat(ChatMessage[] chatMessages)
        {
            string response = $"Hello World: {chatMessages.Length}";
            foreach (char c in response)
            {
                yield return c;
                await Task.Delay(10);
            }
        }
    }
}
