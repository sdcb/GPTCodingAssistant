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
        [Route("echo-ip")]
        public string EchoIP()
        {
            return _ipAccessor.GetClientIPAddress();
        }

        [HttpGet]
        [Route("session")]
        public SessionSimpleResponse[] GetSessions()
        {
            string ip = _ipAccessor.GetClientIPAddress();
            return _db.GetSessions(ip);
        }

        [HttpGet]
        [Route("session/{sessionId}")]
        public ActionResult<SessionResponse> GetSessionById(int sessionId)
        {
            Ip? sessionIp = _db.GetSessionIp(sessionId);

            if (sessionIp == null || sessionIp.Ip1 != _ipAccessor.GetClientIPAddress())
            {
                return NotFound();
            }

            return _db.GetSessionById(sessionId);
        }

        [HttpPost]
        [Route("session")]
        public SessionResponse CreateSession()
        {
            return _db.CreateSession(_ipAccessor.GetClientIPAddress());
        }

        [HttpDelete]
        [Route("session/{sessionId}")]
        public ActionResult DeleteSession(int sessionId)
        {
            Ip? sessionIp = _db.GetSessionIp(sessionId);

            if (sessionIp == null || sessionIp.Ip1 != _ipAccessor.GetClientIPAddress())
            {
                return NotFound();
            }

            _db.DeleteSession(sessionId);
            return Ok();
        }
    }    
}
