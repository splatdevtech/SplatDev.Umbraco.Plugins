using Xunit;
using System.Net;
using System.Text.Json;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;
using Moq.Protected;
using SplatDev.Payments.BancoInter.Models;
using SplatDev.Umbraco.Plugins.Payments.BancoInter.Services;

namespace SplatDev.Umbraco.Plugins.Payments.BancoInter.Tests;

public class BancoInterPixServiceTests
{
    private static IConfiguration BuildConfig(bool sandbox = true) =>
        new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["BancoInter:Sandbox"] = sandbox.ToString()
            })
            .Build();

    private static (BancoInterPixService Service, Mock<HttpMessageHandler> Handler) BuildService(
        IConfiguration config, string responseJson, HttpStatusCode status = HttpStatusCode.OK)
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
                StatusCode = status,
                Content = new StringContent(responseJson, System.Text.Encoding.UTF8, "application/json")
            });

        var factory = new Mock<IHttpClientFactory>();
        factory.Setup(f => f.CreateClient("BancoInter"))
               .Returns(new HttpClient(handler.Object));

        var authService = new Mock<IBancoInterAuthService>();
        authService.Setup(a => a.GetAccessTokenAsync(It.IsAny<string[]>(), It.IsAny<CancellationToken>()))
                   .ReturnsAsync("mock-access-token");

        var service = new BancoInterPixService(factory.Object, config, authService.Object);
        return (service, handler);
    }

    [Fact]
    public async Task CreateImmediateChargeAsync_WithoutTxid_PostsToCorrectEndpoint()
    {
        var responsePayload = new InterPixChargeResponse
        {
            Txid = "auto-txid-1234",
            Status = "ATIVA",
            Chave = "pix@example.com",
            PixCopiaECola = "00020101021226..."
        };
        var json = JsonSerializer.Serialize(responsePayload, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

        var (service, handler) = BuildService(BuildConfig(), json);
        var request = new InterPixChargeRequest
        {
            Chave = "pix@example.com",
            Valor = new InterValor { Original = "100.00" }
        };

        var result = await service.CreateImmediateChargeAsync(request);

        result.Txid.Should().Be("auto-txid-1234");
        result.Status.Should().Be("ATIVA");
        result.PixCopiaECola.Should().StartWith("00020");

        handler.Protected().Verify(
            "SendAsync",
            Times.Once(),
            ItExpr.Is<HttpRequestMessage>(r => r.Method == HttpMethod.Post),
            ItExpr.IsAny<CancellationToken>());
    }

    [Fact]
    public async Task CreateImmediateChargeAsync_WithTxid_PutsToCorrectEndpoint()
    {
        var txid = "mytxid123456789012345678901234";
        var responsePayload = new InterPixChargeResponse { Txid = txid, Status = "ATIVA", Chave = "pix@example.com" };
        var json = JsonSerializer.Serialize(responsePayload, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

        var (service, handler) = BuildService(BuildConfig(), json);
        var request = new InterPixChargeRequest
        {
            Chave = "pix@example.com",
            Valor = new InterValor { Original = "50.00" }
        };

        var result = await service.CreateImmediateChargeAsync(request, txid);

        result.Txid.Should().Be(txid);
        handler.Protected().Verify(
            "SendAsync",
            Times.Once(),
            ItExpr.Is<HttpRequestMessage>(r =>
                r.Method == HttpMethod.Put &&
                r.RequestUri!.AbsolutePath.Contains(txid)),
            ItExpr.IsAny<CancellationToken>());
    }

    [Fact]
    public async Task GetImmediateChargeAsync_ReturnsCharge()
    {
        var txid = "gettxid123456";
        var responsePayload = new InterPixChargeResponse { Txid = txid, Status = "CONCLUIDA", Chave = "pix@example.com" };
        var json = JsonSerializer.Serialize(responsePayload, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

        var (service, _) = BuildService(BuildConfig(), json);
        var result = await service.GetImmediateChargeAsync(txid);

        result.Txid.Should().Be(txid);
        result.Status.Should().Be("CONCLUIDA");
    }
}
