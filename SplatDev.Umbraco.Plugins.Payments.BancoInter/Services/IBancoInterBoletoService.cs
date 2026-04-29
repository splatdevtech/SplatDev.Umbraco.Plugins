using SplatDev.Payments.BancoInter.Models;

namespace SplatDev.Umbraco.Plugins.Payments.BancoInter.Services;

public interface IBancoInterBoletoService
{
    Task<InterBoletoResponse> IssueBoletoAsync(InterBoletoRequest request, CancellationToken ct = default);
    Task<InterBoletoResponse> GetBoletoAsync(string nossoNumero, CancellationToken ct = default);
    Task<byte[]> ExportBoletoPdfAsync(string nossoNumero, CancellationToken ct = default);
    Task CancelBoletoAsync(string nossoNumero, string cancellationReason, CancellationToken ct = default);
}
