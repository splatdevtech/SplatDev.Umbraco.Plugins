using System.Net;
using System.Text.Json;
using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Moq;
using Moq.Protected;
using SplatDev.Payments.BancoInter.Models;
using SplatDev.Umbraco.Plugins.Payments.BancoInter.Services;

namespace SplatDev.Umbraco.Plugins.Payments.BancoInter.Tests;

public class BancoInterAuthServiceTests
{
    private static IConfiguration BuildConfig(bool sandbox = true) =>
        new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["BancoInter:ClientId"] = "16007823-0e5d-48af-b4ae-593f5811dd31",
                ["BancoInter:ClientSecret"] = "ab8086b1-0e0d-45ad-b693-727b72c02593",
                ["BancoInter:Sandbox"] = sandbox.ToString()
            })
            .Build();

    private static (BancoInterAuthService Service, Mock<HttpMessageHandler> Handler) BuildService(
        IConfiguration config, string tokenJson)
    {
        var handler = new Mock<HttpMessageHandler>();
        handler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(tokenJson, System.Text.Encoding.UTF8, "application/json")
            });

        var factory = new Mock<IHttpClientFactory>();
        factory.Setup(f => f.CreateClient("BancoInter"))
               .Returns(new HttpClient(handler.Object));

        var cache = new MemoryCache(new MemoryCacheOptions());
        var service = new BancoInterAuthService(factory.Object, config, cache);
        return (service, handler);
    }

    [Fact]
    public async Task GetAccessTokenAsync_ReturnsToken_OnFirstCall()
    {
        var tokenResponse = new InterTokenResponse
        {
            AccessToken = "test-token-abc123",
            ExpiresIn = 3600,
            TokenType = "Bearer",
            Scope = "cob.write cob.read"
        };
        var json = JsonSerializer.Serialize(tokenResponse,
            new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower });

        var (service, _) = BuildService(BuildConfig(), json);

        var token = await service.GetAccessTokenAsync(["cob.write", "cob.read"]);

        token.Should().Be("test-token-abc123");
    }

    [Fact]
    public async Task GetAccessTokenAsync_ReturnsCachedToken_OnSubsequentCall()
    {
        var tokenResponse = new InterTokenResponse
        {
            AccessToken = "cached-token",
            ExpiresIn = 3600,
            TokenType = "Bearer",
            Scope = "cob.write"
        };
        var json = JsonSerializer.Serialize(tokenResponse,
            new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower });

        var (service, handler) = BuildService(BuildConfig(), json);
        var scopes = new[] { "cob.write" };

        await service.GetAccessTokenAsync(scopes);
        var secondToken = await service.GetAccessTokenAsync(scopes);

        secondToken.Should().Be("cached-token");
        // Only one HTTP call should have been made
        handler.Protected().Verify(
            "SendAsync",
            Times.Once(),
            ItExpr.IsAny<HttpRequestMessage>(),
            ItExpr.IsAny<CancellationToken>());
    }

    [Fact]
    public async Task GetAccessTokenAsync_FetchesSeparateTokens_ForDifferentScopes()
    {
        var calls = 0;
        var handler = new Mock<HttpMessageHandler>();
        handler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(() =>
            {
                calls++;
                var token = new { access_token = $"token-{calls}", expires_in = 3600, token_type = "Bearer", scope = "x" };
                return new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(JsonSerializer.Serialize(token), System.Text.Encoding.UTF8, "application/json")
                };
            });

        var factory = new Mock<IHttpClientFactory>();
        factory.Setup(f => f.CreateClient("BancoInter")).Returns(new HttpClient(handler.Object));
        var cache = new MemoryCache(new MemoryCacheOptions());
        var service = new BancoInterAuthService(factory.Object, BuildConfig(), cache);

        var t1 = await service.GetAccessTokenAsync(["cob.write"]);
        var t2 = await service.GetAccessTokenAsync(["boleto-cobranca.write"]);

        t1.Should().NotBe(t2);
        calls.Should().Be(2);
    }
}
