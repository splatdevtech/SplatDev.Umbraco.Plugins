namespace Umbraco.Plugins.RedirectManager.Models
{
    public class RedirectConfiguration
    {
        public string BasePath { get; set; } = "/quotes";
        public bool ValidateParameters { get; set; } = true;
        public int RateLimitPermit { get; set; } = 100;
        public int RateLimitWindowSeconds { get; set; } = 60;
    }
}