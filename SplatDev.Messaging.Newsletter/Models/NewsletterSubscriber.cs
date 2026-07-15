namespace SplatDev.Messaging.Newsletter.Models
{
    public enum SubscriberStatus
    {
        Subscribed,
        Unsubscribed,
        Pending,
        Bounced,
        Complained
    }

    public class NewsletterSubscriber
    {
        public string Id { get; set; } = string.Empty;

        public string ListId { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string? Name { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public SubscriberStatus Status { get; set; }

        public Dictionary<string, string>? CustomFields { get; set; }

        public string? ProviderExternalId { get; set; }

        public DateTime SubscribedAt { get; set; }

        public DateTime? UnsubscribedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}
