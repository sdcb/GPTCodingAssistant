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
}
