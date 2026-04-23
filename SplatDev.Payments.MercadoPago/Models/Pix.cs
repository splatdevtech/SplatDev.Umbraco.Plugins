namespace SplatDev.Payments.MercadoPago.Models
{
    using System;

    using SplatDev.Payments.Interfaces;
    public class Pix : IPayment
    {
        public IPaymentMethod Details { get; set; }
        public string PaymentMethodId { get; set; }
        public decimal? TransactionAmount { get; set; }
        public string Description { get; set; }
        public IPayer Payer { get; set; }
        public DateTime ExpirationDate { get; set; }
    }
}
