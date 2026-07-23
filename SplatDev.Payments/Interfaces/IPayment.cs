namespace SplatDev.Payments.Interfaces
{
    public interface IPayment
    {
        IPaymentMethod Details { get; set; }
        string PaymentMethodId { get; set; }
        decimal? TransactionAmount { get; set; }
        string Description { get; set; }
    }
}