using Microsoft.AspNetCore.Mvc;
using OpenAI_API;
using OpenAI_API.Chat;

namespace GPTCodingAssistant.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ChatController : ControllerBase
    {
        private static DB _db = DB.Instance;

        [HttpGet]
        public IEnumerable<object> List()
        {
            return _db.Messages.Select(x => new { Role = x.Role.ToString(), Content = x.Content });
        }
    }

    public class DB
    {
        public static DB Instance = new DB();

        public List<ChatMessage> Messages { get; set; } = new List<ChatMessage>();

        internal void CreateAssistantMessage(string input)
        {
            Messages.Add(new ChatMessage(ChatMessageRole.Assistant, input));
        }

        internal void CreateUserMessage(string input)
        {
            Messages.Add(new ChatMessage(ChatMessageRole.User, input));
        }
    }
}
