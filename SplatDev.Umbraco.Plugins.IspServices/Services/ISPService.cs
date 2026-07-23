using Microsoft.AspNetCore.Http;

namespace SplatDev.Umbraco.Plugins.IspServices.Services
{
    public class ISPService(IHttpContextAccessor httpContextAccessor) : IISPService
    {
        private readonly IHttpContextAccessor httpContextAccessor = httpContextAccessor;

        public string? GetClientIp()
        {
            var Ip = httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString();
            if (string.IsNullOrEmpty(Ip))
            {
                Ip = httpContextAccessor.HttpContext?.Request.Headers["X-Forwarded-For"].ToString();
            }
            return Ip;
        }
    }
}
