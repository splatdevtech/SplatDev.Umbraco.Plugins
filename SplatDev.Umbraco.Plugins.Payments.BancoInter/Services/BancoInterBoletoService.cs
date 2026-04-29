using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using SplatDev.Payments.BancoInter;
using SplatDev.Payments.BancoInter.Models;

namespace SplatDev.Umbraco.Plugins.Payments.BancoInter.Services;

public class BancoInterBoletoService(
    IHttpClientFactory httpClientFactory,
    IConfiguration configuration,
    IBancoInterAuthService authService) : IBancoInterBoletoService
{
    private static readonly string[] BoletoScopes = ["boleto-cobranca.write", "boleto-cobranca.read"];

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
        var token = await authService.GetAccessTokenAsync(BoletoScopes, ct);
        var client = httpClientFactory.CreateClient("BancoInter");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return client;
    }

    public async Task<InterBoletoResponse> IssueBoletoAsync(InterBoletoRequest request, CancellationToken ct = default)
    {
        var client = await BuildClientAsync(ct);
        var json = JsonSerializer.Serialize(request, SerializerOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await client.PostAsync($"{BaseUrl}/cobranca/v3/cobrancas", content, ct);
        response.EnsureSuccessStatusCode();
        return await DeserializeAsync<InterBoletoResponse>(response, ct);
    }

    public async Task<InterBoletoResponse> GetBoletoAsync(string nossoNumero, CancellationToken ct = default)
    {
        var client = await BuildClientAsync(ct);
        var response = await client.GetAsync($"{BaseUrl}/cobranca/v3/cobrancas/{nossoNumero}", ct);
        response.EnsureSuccessStatusCode();
        return await DeserializeAsync<InterBoletoResponse>(response, ct);
    }

    public async Task<byte[]> ExportBoletoPdfAsync(string nossoNumero, CancellationToken ct = default)
    {
        var client = await BuildClientAsync(ct);
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/pdf"));
        var response = await client.GetAsync($"{BaseUrl}/cobranca/v3/cobrancas/{nossoNumero}/pdf", ct);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsByteArrayAsync(ct);
    }

    public async Task CancelBoletoAsync(string nossoNumero, string cancellationReason, CancellationToken ct = default)
    {
        var client = await BuildClientAsync(ct);
        var payload = new { motivoCancelamento = cancellationReason };
        var json = JsonSerializer.Serialize(payload, SerializerOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await client.PostAsync($"{BaseUrl}/cobranca/v3/cobrancas/{nossoNumero}/cancelar", content, ct);
        response.EnsureSuccessStatusCode();
    }

    private static async Task<T> DeserializeAsync<T>(HttpResponseMessage response, CancellationToken ct)
    {
        var body = await response.Content.ReadAsStringAsync(ct);
        return JsonSerializer.Deserialize<T>(body, SerializerOptions)
            ?? throw new InvalidOperationException($"Failed to deserialize {typeof(T).Name} from Inter API response.");
    }
}
