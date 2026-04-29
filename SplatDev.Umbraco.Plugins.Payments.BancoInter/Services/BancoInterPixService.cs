using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using SplatDev.Payments.BancoInter;
using SplatDev.Payments.BancoInter.Models;

namespace SplatDev.Umbraco.Plugins.Payments.BancoInter.Services;

public class BancoInterPixService(
    IHttpClientFactory httpClientFactory,
    IConfiguration configuration,
    IBancoInterAuthService authService) : IBancoInterPixService
{
    private static readonly string[] PixScopes = ["cob.write", "cob.read", "cobv.write", "cobv.read", "pix.write", "pix.read", "webhook.write", "webhook.read"];

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
        var token = await authService.GetAccessTokenAsync(PixScopes, ct);
        var client = httpClientFactory.CreateClient("BancoInter");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return client;
    }

    public async Task<InterPixChargeResponse> CreateImmediateChargeAsync(
        InterPixChargeRequest request, string? txid = null, CancellationToken ct = default)
    {
        var client = await BuildClientAsync(ct);
        var json = JsonSerializer.Serialize(request, SerializerOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        HttpResponseMessage response;
        if (string.IsNullOrEmpty(txid))
            response = await client.PostAsync($"{BaseUrl}/pix/v2/cob", content, ct);
        else
            response = await client.PutAsync($"{BaseUrl}/pix/v2/cob/{txid}", content, ct);

        response.EnsureSuccessStatusCode();
        return await DeserializeAsync<InterPixChargeResponse>(response, ct);
    }

    public async Task<InterPixChargeResponse> GetImmediateChargeAsync(string txid, CancellationToken ct = default)
    {
        var client = await BuildClientAsync(ct);
        var response = await client.GetAsync($"{BaseUrl}/pix/v2/cob/{txid}", ct);
        response.EnsureSuccessStatusCode();
        return await DeserializeAsync<InterPixChargeResponse>(response, ct);
    }

    public async Task<InterPixChargeResponse> UpdateImmediateChargeAsync(
        string txid, InterPixChargeRequest request, CancellationToken ct = default)
    {
        var client = await BuildClientAsync(ct);
        var json = JsonSerializer.Serialize(request, SerializerOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await client.PatchAsync($"{BaseUrl}/pix/v2/cob/{txid}", content, ct);
        response.EnsureSuccessStatusCode();
        return await DeserializeAsync<InterPixChargeResponse>(response, ct);
    }

    public async Task<InterPixChargeResponse> CreateDueChargeAsync(
        string txid, InterPixChargeRequest request, CancellationToken ct = default)
    {
        var client = await BuildClientAsync(ct);
        var json = JsonSerializer.Serialize(request, SerializerOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await client.PutAsync($"{BaseUrl}/pix/v2/cobv/{txid}", content, ct);
        response.EnsureSuccessStatusCode();
        return await DeserializeAsync<InterPixChargeResponse>(response, ct);
    }

    public async Task<InterPixChargeResponse> GetDueChargeAsync(string txid, CancellationToken ct = default)
    {
        var client = await BuildClientAsync(ct);
        var response = await client.GetAsync($"{BaseUrl}/pix/v2/cobv/{txid}", ct);
        response.EnsureSuccessStatusCode();
        return await DeserializeAsync<InterPixChargeResponse>(response, ct);
    }

    public async Task<InterDevolucao> RequestDevolutionAsync(
        string e2eId, string devolutionId, string value, string description, CancellationToken ct = default)
    {
        var client = await BuildClientAsync(ct);
        var payload = new { valor = value, descricao = description };
        var json = JsonSerializer.Serialize(payload, SerializerOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await client.PutAsync($"{BaseUrl}/pix/v2/{e2eId}/devolucao/{devolutionId}", content, ct);
        response.EnsureSuccessStatusCode();
        return await DeserializeAsync<InterDevolucao>(response, ct);
    }

    public async Task<InterDevolucao> GetDevolutionAsync(string e2eId, string devolutionId, CancellationToken ct = default)
    {
        var client = await BuildClientAsync(ct);
        var response = await client.GetAsync($"{BaseUrl}/pix/v2/{e2eId}/devolucao/{devolutionId}", ct);
        response.EnsureSuccessStatusCode();
        return await DeserializeAsync<InterDevolucao>(response, ct);
    }

    public async Task RegisterWebhookAsync(string pixKey, string webhookUrl, CancellationToken ct = default)
    {
        var client = await BuildClientAsync(ct);
        var payload = new { webhookUrl };
        var json = JsonSerializer.Serialize(payload, SerializerOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await client.PutAsync($"{BaseUrl}/pix/v2/webhook/{Uri.EscapeDataString(pixKey)}", content, ct);
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteWebhookAsync(string pixKey, CancellationToken ct = default)
    {
        var client = await BuildClientAsync(ct);
        var response = await client.DeleteAsync($"{BaseUrl}/pix/v2/webhook/{Uri.EscapeDataString(pixKey)}", ct);
        response.EnsureSuccessStatusCode();
    }

    private static async Task<T> DeserializeAsync<T>(HttpResponseMessage response, CancellationToken ct)
    {
        var body = await response.Content.ReadAsStringAsync(ct);
        return JsonSerializer.Deserialize<T>(body, SerializerOptions)
            ?? throw new InvalidOperationException($"Failed to deserialize {typeof(T).Name} from Inter API response.");
    }
}
