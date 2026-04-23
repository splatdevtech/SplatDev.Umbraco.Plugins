using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using SplatDev.Umbraco.Plugins.Payments.MercadoPago.Models;

namespace SplatDev.Umbraco.Plugins.Payments.MercadoPago.Services;

public class MercadoPagoService : IMercadoPagoService
{
    private const string HttpClientName = "MercadoPago";
    private const string BaseUrl = "https://api.mercadopago.com";

    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;

    public MercadoPagoService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
    }

    public MercadoPagoConfig GetConfig()
    {
        return new MercadoPagoConfig
        {
            AccessToken = _configuration["MercadoPago:AccessToken"] ?? string.Empty,
            PublicKey   = _configuration["MercadoPago:PublicKey"] ?? string.Empty,
            Sandbox     = bool.TryParse(_configuration["MercadoPago:Sandbox"], out var sb) && sb
        };
    }

    public async Task<string> CreatePaymentPreference(string orderRef, decimal amount, string description)
    {
        var config = GetConfig();
        var client = _httpClientFactory.CreateClient(HttpClientName);
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", config.AccessToken);

        var payload = new
        {
            items = new[]
            {
                new
                {
                    title = description,
                    quantity = 1,
                    unit_price = amount
                }
            },
            external_reference = orderRef
        };

        var json = JsonSerializer.Serialize(payload);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await client.PostAsync($"{BaseUrl}/checkout/preferences", content);
        response.EnsureSuccessStatusCode();

        var responseBody = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(responseBody);

        return doc.RootElement.TryGetProperty("id", out var idElement)
            ? idElement.GetString() ?? string.Empty
            : string.Empty;
    }

    public async Task<string> GetPaymentStatus(string paymentId)
    {
        var config = GetConfig();
        var client = _httpClientFactory.CreateClient(HttpClientName);
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", config.AccessToken);

        var response = await client.GetAsync($"{BaseUrl}/v1/payments/{paymentId}");
        response.EnsureSuccessStatusCode();

        var responseBody = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(responseBody);

        return doc.RootElement.TryGetProperty("status", out var statusElement)
            ? statusElement.GetString() ?? string.Empty
            : string.Empty;
    }
}
