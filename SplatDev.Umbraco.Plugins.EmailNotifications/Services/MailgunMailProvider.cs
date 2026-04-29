using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace SplatDev.Umbraco.Plugins.EmailNotifications.Services;

public class MailgunOptions
{
    public string ApiKey { get; set; } = string.Empty;
    public string Domain { get; set; } = string.Empty;
    public string FromName { get; set; } = "Notifications";
    public string FromAddress { get; set; } = string.Empty;
    public string? WebhookSigningKey { get; set; }
    public string BaseUrl { get; set; } = "https://api.mailgun.net";
}

public class MailgunMailProvider(
    IHttpClientFactory httpClientFactory,
    IOptions<MailgunOptions> options,
    ILogger<MailgunMailProvider> logger) : IMailProvider
{
    private readonly MailgunOptions _options = options.Value;

    public string ProviderName => "Mailgun";

    public async Task<string?> SendAsync(MailMessage message, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(_options.ApiKey) || string.IsNullOrWhiteSpace(_options.Domain))
        {
            logger.LogWarning("Mailgun is not configured — ApiKey or Domain is missing.");
            return null;
        }

        var fromAddress = message.FromAddress ?? _options.FromAddress;
        var fromName = message.FromName ?? _options.FromName;
        var from = string.IsNullOrWhiteSpace(fromName) ? fromAddress : $"{fromName} <{fromAddress}>";

        var fields = new List<KeyValuePair<string, string>>
        {
            new("from", from),
            new("to", message.To),
            new("subject", message.Subject),
            new("html", message.HtmlBody),
        };

        if (!string.IsNullOrWhiteSpace(message.PlainTextBody))
            fields.Add(new("text", message.PlainTextBody));

        if (!string.IsNullOrWhiteSpace(message.ReplyTo))
            fields.Add(new("h:Reply-To", message.ReplyTo));

        if (message.Tags is { Count: > 0 })
        {
            foreach (var tag in message.Tags)
                fields.Add(new("o:tag", tag));
        }

        var client = httpClientFactory.CreateClient("Mailgun");
        using var content = new FormUrlEncodedContent(fields);
        var url = $"{_options.BaseUrl.TrimEnd('/')}/v3/{_options.Domain}/messages";

        try
        {
            var response = await client.PostAsync(url, content, ct);
            var body = await response.Content.ReadAsStringAsync(ct);

            if (!response.IsSuccessStatusCode)
            {
                logger.LogError("Mailgun send failed {StatusCode}: {Body}", (int)response.StatusCode, body);
                return null;
            }

            using var doc = JsonDocument.Parse(body);
            return doc.RootElement.TryGetProperty("id", out var idProp) ? idProp.GetString() : null;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Mailgun send threw an exception for {To}", message.To);
            return null;
        }
    }

    public bool VerifyWebhookSignature(string timestamp, string token, string signature)
    {
        if (string.IsNullOrWhiteSpace(_options.WebhookSigningKey))
            return false;

        var data = timestamp + token;
        var keyBytes = Encoding.UTF8.GetBytes(_options.WebhookSigningKey);
        var dataBytes = Encoding.UTF8.GetBytes(data);

        using var hmac = new HMACSHA256(keyBytes);
        var computed = hmac.ComputeHash(dataBytes);
        var computedHex = Convert.ToHexString(computed).ToLowerInvariant();
        return computedHex == signature.ToLowerInvariant();
    }
}

public static class MailgunHttpClientExtensions
{
    public static IHttpClientBuilder AddMailgunHttpClient(this IServiceCollection services, MailgunOptions options)
    {
        return services.AddHttpClient("Mailgun", client =>
        {
            var credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes($"api:{options.ApiKey}"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);
        });
    }
}
