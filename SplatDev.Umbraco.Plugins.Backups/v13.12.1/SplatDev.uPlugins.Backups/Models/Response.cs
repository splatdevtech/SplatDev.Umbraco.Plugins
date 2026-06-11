using System.Net;

namespace SplatDev.uPlugins.Backups.Models
{
    public class Response
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public object? Details { get; set; }
        public HttpStatusCode StatusCode { get; set; }
    }
}
