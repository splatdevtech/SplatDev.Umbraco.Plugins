namespace SplatDev.Payments.MercadoPago.Models
{

    using SplatDev.Payments.Interfaces;

    public struct Subscription : ISubscription
    {
        public string Id { get; set; }

        public string PrepprovalId { get; set; }

        public long ApplicationId { get; set; }

        public int PayerId { get; set; }

        public AutoRecurring AutoRecurring { get; set; }

        public string BackUrl { get; set; }

        public long CollectorId { get; set; }

        public string ExternalReference { get; set; }

        public string PayerEmail { get; set; }

        public string Reason { get; set; }

        public string PreapprovalPlanId { get; set; }

        public string CardTokenId { get; set; }

        public string Status { get; set; }
    }
}
