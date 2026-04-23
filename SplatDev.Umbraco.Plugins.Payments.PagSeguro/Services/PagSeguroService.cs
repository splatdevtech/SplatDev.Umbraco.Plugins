using System.Text;
using System.Xml.Linq;
using Microsoft.Extensions.Configuration;
using SplatDev.Umbraco.Plugins.Payments.PagSeguro.Models;

namespace SplatDev.Umbraco.Plugins.Payments.PagSeguro.Services;

public class PagSeguroService : IPagSeguroService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;

    public PagSeguroService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
    }

    public PagSeguroConfig GetConfig()
    {
        return new PagSeguroConfig
        {
            Email   = _configuration["PagSeguro:Email"] ?? string.Empty,
            Token   = _configuration["PagSeguro:Token"] ?? string.Empty,
            Sandbox = bool.TryParse(_configuration["PagSeguro:Sandbox"], out var sb) && sb
        };
    }

    private string BaseUrl(bool sandbox) =>
        sandbox
            ? "https://ws.sandbox.pagseguro.uol.com.br"
            : "https://ws.pagseguro.uol.com.br";

    public async Task<string> CreateTransaction(string orderRef, decimal amount, string description)
    {
        var config = GetConfig();
        var client = _httpClientFactory.CreateClient();

        var xmlBody = new XDocument(
            new XElement("checkout",
                new XElement("currency", "BRL"),
                new XElement("reference", orderRef),
                new XElement("items",
                    new XElement("item",
                        new XElement("id", "1"),
                        new XElement("description", description),
                        new XElement("amount", amount.ToString("F2", System.Globalization.CultureInfo.InvariantCulture)),
                        new XElement("quantity", "1")
                    )
                )
            )
        );

        var url = $"{BaseUrl(config.Sandbox)}/v2/checkout?email={Uri.EscapeDataString(config.Email)}&token={config.Token}";
        var content = new StringContent(xmlBody.ToString(), Encoding.UTF8, "application/xml");

        var response = await client.PostAsync(url, content);
        response.EnsureSuccessStatusCode();

        var responseXml = await response.Content.ReadAsStringAsync();
        var doc = XDocument.Parse(responseXml);
        var code = doc.Root?.Element("code")?.Value ?? string.Empty;

        return $"{BaseUrl(config.Sandbox)}/v2/checkout/payment.html?code={code}";
    }

    public async Task<string> GetTransactionStatus(string code)
    {
        var config = GetConfig();
        var client = _httpClientFactory.CreateClient();

        var url = $"{BaseUrl(config.Sandbox)}/v3/transactions/{code}?email={Uri.EscapeDataString(config.Email)}&token={config.Token}";
        var response = await client.GetAsync(url);
        response.EnsureSuccessStatusCode();

        var responseXml = await response.Content.ReadAsStringAsync();
        var doc = XDocument.Parse(responseXml);
        return doc.Root?.Element("status")?.Value ?? string.Empty;
    }
}
