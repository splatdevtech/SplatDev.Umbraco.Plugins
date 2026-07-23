namespace SplatDev.Payments.MercadoPago.Models
{
    public struct PayerCosts
    {
        public decimal InstallmentRate { get; set; }
        public decimal DiscountRate { get; set; }
        public decimal MinAllowedAmount { get; set; }
        public int Installments { get; set; }
        public decimal ReimbursementRate { get; set; }
        public decimal MaxAllowedAmount { get; set; }
        public string PaymentMethodOptionId { get; set; }
    }
}
