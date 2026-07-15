namespace SplatDev.Messaging.Newsletter.Models
{
    public enum CampaignStatus
    {
        Draft,
        Scheduled,
        Sending,
        Sent
    }

    public class NewsletterCampaign
    {
        public string Id { get; set; } = string.Empty;

        public string ListId { get; set; } = string.Empty;

        public string? TemplateId { get; set; }

        public string Subject { get; set; } = string.Empty;

        public string FromName { get; set; } = string.Empty;

        public string FromAddress { get; set; } = string.Empty;

        public string? HtmlBody { get; set; }

        public string? PlainTextBody { get; set; }

        public CampaignStatus Status { get; set; }

        public bool TrackOpens { get; set; }

        public bool TrackClicks { get; set; }

        public DateTime? ScheduledAt { get; set; }

        public DateTime? SentAt { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}
