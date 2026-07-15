namespace SplatDev.Messaging.Newsletter.Events
{
    using SplatDev.Messaging.Newsletter.Models;

    public class NewsletterEventArgs : EventArgs
    {
        public NewsletterList? List { get; set; }

        public NewsletterSubscriber? Subscriber { get; set; }

        public NewsletterCampaign? Campaign { get; set; }

        public CampaignStats? Stats { get; set; }

        public string EventType { get; set; } = string.Empty;

        public DateTime Timestamp { get; set; }

        public string? ProviderName { get; set; }

        public Dictionary<string, object>? RawPayload { get; set; }
    }
}
