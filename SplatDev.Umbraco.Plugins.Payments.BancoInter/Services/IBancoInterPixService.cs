using SplatDev.Payments.BancoInter.Models;

namespace SplatDev.Umbraco.Plugins.Payments.BancoInter.Services;

public interface IBancoInterPixService
{
    Task<InterPixChargeResponse> CreateImmediateChargeAsync(InterPixChargeRequest request, string? txid = null, CancellationToken ct = default);
    Task<InterPixChargeResponse> GetImmediateChargeAsync(string txid, CancellationToken ct = default);
    Task<InterPixChargeResponse> UpdateImmediateChargeAsync(string txid, InterPixChargeRequest request, CancellationToken ct = default);

    Task<InterPixChargeResponse> CreateDueChargeAsync(string txid, InterPixChargeRequest request, CancellationToken ct = default);
    Task<InterPixChargeResponse> GetDueChargeAsync(string txid, CancellationToken ct = default);

    Task<InterDevolucao> RequestDevolutionAsync(string e2eId, string devolutionId, string value, string description, CancellationToken ct = default);
    Task<InterDevolucao> GetDevolutionAsync(string e2eId, string devolutionId, CancellationToken ct = default);

    Task RegisterWebhookAsync(string pixKey, string webhookUrl, CancellationToken ct = default);
    Task DeleteWebhookAsync(string pixKey, CancellationToken ct = default);
}
