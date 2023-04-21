using Microsoft.AspNetCore.SignalR;
using OpenAI_API.Chat;

namespace GPTCodingAssistant.Hubs
{
    public class ChatHub : Hub
    {
        public async IAsyncEnumerable<char> Chat()
        {
            string response = $"Hello World";
            foreach (char c in response)
            {
                yield return c;
                await Task.Delay(10);
            }
        }
    }
}
