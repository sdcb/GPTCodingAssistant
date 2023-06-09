﻿namespace GPTCodingAssistant.Services
{
    public class IPAccessor
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public IPAccessor(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string GetClientIPAddress()
        {
            HttpContext? httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null) 
            {
                throw new Exception("HttpContext is null");
            }

            string? forwarded = httpContext.Request.Headers["X-Forwarded-For"];
            if (forwarded != null)
            {
                return forwarded.Split(',', StringSplitOptions.RemoveEmptyEntries)[0];
            }

            string? realIp = httpContext?.Connection.RemoteIpAddress?.ToString();
            if (realIp == null)
            {
                throw new Exception("Real ip can't be null");
            }

            return realIp;
        }
    }
}
