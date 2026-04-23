namespace SplatDev.Payments.Interfaces
{
    using System.Threading.Tasks;
    public interface IPayment<T>
    {
        Task<T> CreatePaymentRequestAsync(IPayment model);
        Task<T> GetPaymentCodeAsync(IPayment model, string contentType);
        Task<T> GetTransactionAsync(string notificationCode, string receiver, string token);
        Task<bool> ConfirmTransationAsync(string transaction, string referenceCode, string receiver, string token);
    }
}
