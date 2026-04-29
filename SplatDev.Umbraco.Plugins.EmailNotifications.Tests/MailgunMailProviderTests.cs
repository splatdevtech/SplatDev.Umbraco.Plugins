using System.Net;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using SplatDev.Umbraco.Plugins.EmailNotifications.Services;
using Xunit;

namespace SplatDev.Umbraco.Plugins.EmailNotifications.Tests;

public class MailgunMailProviderTests
{
    private static MailgunMailProvider CreateProvider(HttpMessageHandler handler, MailgunOptions? opts = null)
    {
        opts ??= new MailgunOptions
        {
            ApiKey = "test-key",
            Domain = "test.mailgun.org",
            FromAddress = "noreply@test.mailgun.org",
            WebhookSigningKey = "webhook-secret",
        };

        var handlerMock = Mock.Get(handler);
        var factory = new Mock<IHttpClientFactory>();
        factory.Setup(f => f.CreateClient("Mailgun")).Returns(new HttpClient(handler));

        return new MailgunMailProvider(factory.Object, Options.Create(opts), NullLogger<MailgunMailProvider>.Instance);
    }

    [Fact]
    public async Task SendAsync_ReturnsMessageId_WhenMailgunRespondsSuccess()
    {
        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""{"id": "<20240101.abc@test.mailgun.org>", "message": "Queued"}"""),
            });

        var provider = CreateProvider(handlerMock.Object);
        var msg = new MailMessage("user@example.com", "Hello", "<p>World</p>");

        var id = await provider.SendAsync(msg);

        Assert.NotNull(id);
        Assert.Contains("20240101", id);
    }

    [Fact]
    public async Task SendAsync_ReturnsNull_WhenMailgunReturnsBadRequest()
    {
        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new StringContent("""{"message": "to parameter is not a valid address"}"""),
            });

        var provider = CreateProvider(handlerMock.Object);
        var msg = new MailMessage("bad-address", "Hello", "<p>World</p>");

        var id = await provider.SendAsync(msg);

        Assert.Null(id);
    }

    [Fact]
    public async Task SendAsync_ReturnsNull_WhenNotConfigured()
    {
        var handlerMock = new Mock<HttpMessageHandler>();
        var provider = CreateProvider(handlerMock.Object, new MailgunOptions());

        var id = await provider.SendAsync(new MailMessage("user@example.com", "Test", "<p>Hi</p>"));

        Assert.Null(id);
    }

    [Fact]
    public void VerifyWebhookSignature_ReturnsTrue_ForValidSignature()
    {
        const string signingKey = "webhook-secret";
        const string timestamp = "1704067200";
        const string token = "abc123token";

        // Compute expected HMAC-SHA256
        using var hmac = new System.Security.Cryptography.HMACSHA256(
            System.Text.Encoding.UTF8.GetBytes(signingKey));
        var data = System.Text.Encoding.UTF8.GetBytes(timestamp + token);
        var hash = hmac.ComputeHash(data);
        var signature = Convert.ToHexString(hash).ToLowerInvariant();

        var opts = new MailgunOptions { WebhookSigningKey = signingKey };
        var handlerMock = new Mock<HttpMessageHandler>();
        var provider = CreateProvider(handlerMock.Object, opts);

        Assert.True(provider.VerifyWebhookSignature(timestamp, token, signature));
    }

    [Fact]
    public void VerifyWebhookSignature_ReturnsFalse_ForInvalidSignature()
    {
        var opts = new MailgunOptions { WebhookSigningKey = "webhook-secret" };
        var handlerMock = new Mock<HttpMessageHandler>();
        var provider = CreateProvider(handlerMock.Object, opts);

        Assert.False(provider.VerifyWebhookSignature("1704067200", "token", "wrong-signature"));
    }

    [Fact]
    public void VerifyWebhookSignature_ReturnsFalse_WhenSigningKeyNotConfigured()
    {
        var opts = new MailgunOptions();
        var handlerMock = new Mock<HttpMessageHandler>();
        var provider = CreateProvider(handlerMock.Object, opts);

        Assert.False(provider.VerifyWebhookSignature("ts", "tok", "sig"));
    }

    [Fact]
    public void ProviderName_IsMailgun()
    {
        var handlerMock = new Mock<HttpMessageHandler>();
        var provider = CreateProvider(handlerMock.Object);
        Assert.Equal("Mailgun", provider.ProviderName);
    }
}
