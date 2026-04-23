using UmbracoCms.Plugins.Payments.MercadoPago.Models;

namespace UmbracoCms.Plugins.Payments.MercadoPago.Services;

public interface IMercadoPagoService
{
    MercadoPagoConfig GetConfig();
    Task<string> CreatePaymentPreference(string orderRef, decimal amount, string description);
    Task<string> GetPaymentStatus(string paymentId);
}
