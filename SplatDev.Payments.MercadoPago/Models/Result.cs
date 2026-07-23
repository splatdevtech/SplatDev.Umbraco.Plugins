namespace SplatDev.Payments.MercadoPago.Models
{

    using System;
    public struct Result
    {
        public int AccreditationTime { get; set; }
        public CreditCard Card { get; set; }
        public int CollectorId { get; set; }
        public int? CorporationId { get; set; }
        public decimal CouponAmount { get; set; }
        public string CurrencyId { get; set; }
        public DateTime DateApproved { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateLastUpdated { get; set; }
        public string DeferredCapture { get; set; }
        public string Description { get; set; }
        public FeeDetails[] FeeDetails { get; set; }
        public string Id { get; set; }
        public int Installments { get; set; }
        public Issuer Issuer { get; set; }
        public string Marketplace { get; set; }
        public int MaxAccreditationDays { get; set; }
        public int MinAccreditationDays { get; set; }
        public DateTime MoneyReleaseDate { get; set; }
        public string MoneyReleaseSchema { get; set; }
        public string Name { get; set; }
        public string NotificationUrl { get; set; }
        public string OperationType { get; set; }
        public Payer Payer { get; set; }
        public PayerCosts[] PayerCosts { get; set; }
        public string PaymentMethodId { get; set; }
        public string PaymentTypeId { get; set; }
        public string ProcessingMode { get; set; }
        public string SecureThumbnail { get; set; }
        public string StatementDescriptor { get; set; }
        public string Status { get; set; }
        public string StatusDetails { get; set; }
        public string Thumbnail { get; set; }
        public decimal TotalFinancialCost { get; set; }
        public decimal TransactionAmount { get; set; }
        public decimal TransactionAmountRefunded { get; set; }
        public TransactionDetails TransactionDetails { get; set; }
    }
}
