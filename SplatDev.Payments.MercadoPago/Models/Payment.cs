namespace SplatDev.Payments.MercadoPago.Models
{

    using System;
    using System.Collections.Generic;

    using SplatDev.Payments.Interfaces;
    public class Payment : IPayment
    {
        public AdditionalInfo AdditionalInfo { get; set; }
        public int ApplicationFee { get; set; }
        public bool BinaryMode { get; set; }
        public string CallbackUrl { get; set; }
        public int CampaignId { get; set; }
        public bool Capture { get; set; } = true;
        public bool Captured { get; set; }
        public CreditCard Card { get; set; }
        public CardHolder Cardholder { get; set; }
        public decimal CouponAmount { get; set; }
        public string CouponCode { get; set; }
        public string CurrencyId { get; set; }
        public DateTime DateApproved { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateLastUpdated { get; set; }
        public DateTime DateOfExpiration { get; set; }
        public string Description { get; set; }
        public IPaymentMethod Details { get; set; }
        public int DifferentialPricingId { get; set; }
        public string Email { get; set; }
        public string ExternalReference { get; set; }
        public long Id { get; set; }
        public int Installments { get; set; }
        public string IssuerId { get; set; }
        public bool LiveMode { get; set; }
        public bool LuhnValidation { get; set; }
        public string Message { get; set; }
        public IDictionary<string, object> Metadata { get; set; }
        public DateTime MoneyReleaseDate { get; set; }
        public string NotificationUrl { get; set; }
        public string OperationType { get; set; }
        public Order Order { get; set; }
        public Payer Payer { get; set; }
        public string PaymentMethodId { get; set; }
        public string PaymentTypeId { get; set; }
        public string Phone { get; set; }
        public Shipment Shipping { get; set; }
        public string StatementDescriptor { get; set; }
        public string Status { get; set; }
        public string StatusDetail { get; set; }
        public string Token { get; set; }
        public decimal? TransactionAmount { get; set; }
        public decimal TransactionAmountRefunded { get; set; }
        public string TransactionDescription { get; set; }
        public TransactionDetails TransactionDetails { get; set; }
    }
}
