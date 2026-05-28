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

public class BancoInterBoletoServiceTests
{
    private static IConfiguration BuildConfig() =>
        new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["BancoInter:Sandbox"] = "true"
            })
            .Build();

    private static (BancoInterBoletoService Service, Mock<HttpMessageHandler> Handler) BuildService(
        string responseJson, HttpStatusCode status = HttpStatusCode.OK)
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
                   .ReturnsAsync("mock-token");

        var service = new BancoInterBoletoService(factory.Object, BuildConfig(), authService.Object);
        return (service, handler);
    }

    [Fact]
    public async Task IssueBoletoAsync_ReturnsNossoNumero()
    {
        var response = new InterBoletoResponse
        {
            NossoNumero = "00123456789",
            LinhaDigitavel = "00190.00009 01234.567890 12345.678901 9 99990000010000",
            CodigoBarras = "00199999900000100000000001234567890123456789",
            Situacao = "EMABERTO",
            ValorNominal = 100.00m
        };
        var json = JsonSerializer.Serialize(response, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

        var (service, _) = BuildService(json);
        var request = new InterBoletoRequest
        {
            ValorNominal = 100.00m,
            DataVencimento = "2026-12-31",
            SeuNumero = "REF-001",
            Pagador = new InterPagador
            {
                CpfCnpj = "12345678901",
                Nome = "Test Payer",
                Endereco = "Rua Teste 123",
                Cidade = "São Paulo",
                Uf = "SP",
                Cep = "01310100"
            }
        };

        var result = await service.IssueBoletoAsync(request);

        result.NossoNumero.Should().Be("00123456789");
        result.Situacao.Should().Be("EMABERTO");
        result.LinhaDigitavel.Should().NotBeEmpty();
    }

    [Fact]
    public async Task CancelBoletoAsync_SendsPostToCorrectEndpoint()
    {
        var (service, handler) = BuildService("{}");

        await service.CancelBoletoAsync("00123456789", "APEDIDODOCLIENTE");

        handler.Protected().Verify(
            "SendAsync",
            Times.Once(),
            ItExpr.Is<HttpRequestMessage>(r =>
                r.Method == HttpMethod.Post &&
                r.RequestUri!.AbsolutePath.Contains("cancelar")),
            ItExpr.IsAny<CancellationToken>());
    }
}
