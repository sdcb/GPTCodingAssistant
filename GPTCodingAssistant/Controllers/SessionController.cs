using GPTCodingAssistant.DB;
using GPTCodingAssistant.DB.Helpers;
using GPTCodingAssistant.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OpenAI_API;
using OpenAI_API.Chat;

namespace GPTCodingAssistant.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SessionController : ControllerBase
    {
        private readonly ChatGPTRepository _db;
        private readonly IPAccessor _ipAccessor;

        public SessionController(ChatGPTRepository db, IPAccessor ipAccessor)
        {
            _db = db;
            _ipAccessor = ipAccessor;
        }

        [HttpGet]
        [Route("session")]
        public SessionSimpleResponse[] GetSessions()
        {
            string ip = _ipAccessor.GetIP();
            return _db.GetSessions(ip);
        }

        [HttpGet]
        [Route("session/{sessionId}")]
        public SessionResponse GetSessionById(int sessionId)
        {
            if (_db.GetSessionIp(sessionId).Ip1 != _ipAccessor.GetIP())
            {
                throw new InvalidOperationException("Session IP not match client's IP");
            }

            return _db.GetSession(sessionId);
        }

        [HttpPost]
        [Route("session")]
        public SessionResponse CreateSession()
        {
            return _db.CreateSession(_ipAccessor.GetIP());
        }

        [HttpDelete]
        [Route("session/{sessionId}")]
        public void DeleteSession(int sessionId)
        {
            if (_db.GetSessionIp(sessionId).Ip1 != _ipAccessor.GetIP())
            {
                throw new InvalidOperationException("Session IP not match client's IP");
            }
            _db.DeleteSession(sessionId);
        }
    }    
}
