using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using SplatDev.Payments.BancoInter;
using SplatDev.Payments.BancoInter.Models;

namespace SplatDev.Umbraco.Plugins.Payments.BancoInter.Services;

public class BancoInterBankingService(
    IHttpClientFactory httpClientFactory,
    IConfiguration configuration,
    IBancoInterAuthService authService) : IBancoInterBankingService
{
    private static readonly string[] BankingScopes =
    [
        "extrato.read",
        "pagamento-pix.write", "pagamento-pix.read",
        "pagamento-boleto.write", "pagamento-boleto.read"
    ];

    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
    };

    private string BaseUrl => new BancoInterSettings
    {
        Sandbox = bool.TryParse(configuration["BancoInter:Sandbox"], out var sb) && sb
    }.BaseUrl;

    private async Task<HttpClient> BuildClientAsync(CancellationToken ct)
    {
        var token = await authService.GetAccessTokenAsync(BankingScopes, ct);
        var client = httpClientFactory.CreateClient("BancoInter");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return client;
    }

    public async Task<InterBankingBalance> GetBalanceAsync(CancellationToken ct = default)
    {
        var client = await BuildClientAsync(ct);
        var response = await client.GetAsync($"{BaseUrl}/banking/v2/saldo", ct);
        response.EnsureSuccessStatusCode();
        return await DeserializeAsync<InterBankingBalance>(response, ct);
    }

    public async Task<InterBankingStatement> GetStatementAsync(string startDate, string endDate, CancellationToken ct = default)
    {
        var client = await BuildClientAsync(ct);
        var url = $"{BaseUrl}/banking/v2/extrato?dataInicio={Uri.EscapeDataString(startDate)}&dataFim={Uri.EscapeDataString(endDate)}";
        var response = await client.GetAsync(url, ct);
        response.EnsureSuccessStatusCode();
        return await DeserializeAsync<InterBankingStatement>(response, ct);
    }

    public async Task<InterPixPaymentResponse> SendPixPaymentAsync(InterPixPaymentRequest request, CancellationToken ct = default)
    {
        var client = await BuildClientAsync(ct);
        var json = JsonSerializer.Serialize(request, SerializerOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await client.PostAsync($"{BaseUrl}/banking/v2/pix", content, ct);
        response.EnsureSuccessStatusCode();
        return await DeserializeAsync<InterPixPaymentResponse>(response, ct);
    }

    public async Task<InterPixPaymentResponse> GetPixPaymentAsync(string codigoSolicitacao, CancellationToken ct = default)
    {
        var client = await BuildClientAsync(ct);
        var response = await client.GetAsync($"{BaseUrl}/banking/v2/pix/{codigoSolicitacao}", ct);
        response.EnsureSuccessStatusCode();
        return await DeserializeAsync<InterPixPaymentResponse>(response, ct);
    }

    public async Task<InterBoletoPaymentResponse> PayBoletoAsync(InterBoletoPaymentRequest request, CancellationToken ct = default)
    {
        var client = await BuildClientAsync(ct);
        var json = JsonSerializer.Serialize(request, SerializerOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await client.PostAsync($"{BaseUrl}/banking/v2/pagamento", content, ct);
        response.EnsureSuccessStatusCode();
        return await DeserializeAsync<InterBoletoPaymentResponse>(response, ct);
    }

    public async Task<InterBoletoPaymentResponse> GetBoletoPaymentAsync(string codigoSolicitacao, CancellationToken ct = default)
    {
        var client = await BuildClientAsync(ct);
        var response = await client.GetAsync($"{BaseUrl}/banking/v2/pagamento/{codigoSolicitacao}", ct);
        response.EnsureSuccessStatusCode();
        return await DeserializeAsync<InterBoletoPaymentResponse>(response, ct);
    }

    private static async Task<T> DeserializeAsync<T>(HttpResponseMessage response, CancellationToken ct)
    {
        var body = await response.Content.ReadAsStringAsync(ct);
        return JsonSerializer.Deserialize<T>(body, SerializerOptions)
            ?? throw new InvalidOperationException($"Failed to deserialize {typeof(T).Name} from Inter Banking API response.");
    }
}
