using SplatDev.Umbraco.Plugins.Payments.MercadoPago.Models;

namespace SplatDev.Umbraco.Plugins.Payments.MercadoPago.Services;

public interface IMercadoPagoService
{
    MercadoPagoConfig GetConfig();
    Task<string> CreatePaymentPreference(string orderRef, decimal amount, string description);
    Task<string> GetPaymentStatus(string paymentId);
}
