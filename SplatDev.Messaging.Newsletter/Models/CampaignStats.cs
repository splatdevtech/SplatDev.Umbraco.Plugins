namespace SplatDev.Messaging.Newsletter.Models
{
    public class CampaignStats
    {
        public string CampaignId { get; set; } = string.Empty;

        public int Opens { get; set; }

        public int Clicks { get; set; }

        public int Delivered { get; set; }

        public int Bounced { get; set; }

        public int Complaints { get; set; }

        public int Unsubscribes { get; set; }

        public DateTime FetchedAt { get; set; }
    }
}
