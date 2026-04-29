using SplatDev.Payments.BancoInter.Models;

namespace SplatDev.Umbraco.Plugins.Payments.BancoInter.Services;

public interface IBancoInterBankingService
{
    Task<InterBankingBalance> GetBalanceAsync(CancellationToken ct = default);
    Task<InterBankingStatement> GetStatementAsync(string startDate, string endDate, CancellationToken ct = default);
    Task<InterPixPaymentResponse> SendPixPaymentAsync(InterPixPaymentRequest request, CancellationToken ct = default);
    Task<InterPixPaymentResponse> GetPixPaymentAsync(string codigoSolicitacao, CancellationToken ct = default);
    Task<InterBoletoPaymentResponse> PayBoletoAsync(InterBoletoPaymentRequest request, CancellationToken ct = default);
    Task<InterBoletoPaymentResponse> GetBoletoPaymentAsync(string codigoSolicitacao, CancellationToken ct = default);
}
