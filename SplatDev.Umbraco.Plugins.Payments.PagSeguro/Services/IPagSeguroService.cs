using SplatDev.Umbraco.Plugins.Payments.PagSeguro.Models;

namespace SplatDev.Umbraco.Plugins.Payments.PagSeguro.Services;

public interface IPagSeguroService
{
    PagSeguroConfig GetConfig();
    Task<string> CreateTransaction(string orderRef, decimal amount, string description);
    Task<string> GetTransactionStatus(string code);
}
