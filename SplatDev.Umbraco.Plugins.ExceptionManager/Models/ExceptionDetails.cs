namespace SplatDev.Umbraco.Plugins.ExceptionManager.Models
{
    public class ExceptionDetails
    {
        public string Url { get; set; } = string.Empty;
        public string IP { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string? Message { get; set; }
        public string? StackTrace { get; set; }
        public string? User { get; set; }
        public int StatusCode { get; set; }
        public string? Domain { get; set; }
    }
}
