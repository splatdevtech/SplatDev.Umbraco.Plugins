using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using SplatDev.Payments.BancoInter;
using SplatDev.Payments.BancoInter.Models;

namespace SplatDev.Umbraco.Plugins.Payments.BancoInter.Services;

public class BancoInterAuthService(
    IHttpClientFactory httpClientFactory,
    IConfiguration configuration,
    IMemoryCache cache) : IBancoInterAuthService
{
    private const string CacheKeyPrefix = "BancoInter:Token:";
    private const int SafetyMarginSeconds = 30;

    private BancoInterSettings GetSettings() => new()
    {
        ClientId = configuration["BancoInter:ClientId"] ?? string.Empty,
        ClientSecret = configuration["BancoInter:ClientSecret"] ?? string.Empty,
        Sandbox = bool.TryParse(configuration["BancoInter:Sandbox"], out var sb) && sb,
        CertificatePath = configuration["BancoInter:CertificatePath"],
        CertificateKeyPath = configuration["BancoInter:CertificateKeyPath"]
    };

    public async Task<string> GetAccessTokenAsync(string[] scopes, CancellationToken ct = default)
    {
        var scopeKey = string.Join(" ", scopes.OrderBy(s => s));
        var cacheKey = $"{CacheKeyPrefix}{scopeKey}";

        if (cache.TryGetValue(cacheKey, out string? cachedToken) && !string.IsNullOrEmpty(cachedToken))
            return cachedToken;

        var token = await FetchTokenAsync(scopes, ct);

        var expiry = TimeSpan.FromSeconds(Math.Max(0, token.ExpiresIn - SafetyMarginSeconds));
        cache.Set(cacheKey, token.AccessToken, expiry);

        return token.AccessToken;
    }

    private async Task<InterTokenResponse> FetchTokenAsync(string[] scopes, CancellationToken ct)
    {
        var settings = GetSettings();
        var client = httpClientFactory.CreateClient("BancoInter");

        var formData = new Dictionary<string, string>
        {
            ["grant_type"] = "client_credentials",
            ["client_id"] = settings.ClientId,
            ["client_secret"] = settings.ClientSecret,
            ["scope"] = string.Join(" ", scopes)
        };

        var request = new HttpRequestMessage(HttpMethod.Post, settings.TokenUrl)
        {
            Content = new FormUrlEncodedContent(formData)
        };
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        var response = await client.SendAsync(request, ct);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync(ct);
        var token = JsonSerializer.Deserialize<InterTokenResponse>(json)
            ?? throw new InvalidOperationException("Failed to deserialize Inter token response.");

        token.ExpiresAt = DateTimeOffset.UtcNow.AddSeconds(token.ExpiresIn);
        return token;
    }
}
