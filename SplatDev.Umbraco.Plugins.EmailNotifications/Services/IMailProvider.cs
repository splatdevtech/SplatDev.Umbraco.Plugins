namespace SplatDev.Umbraco.Plugins.EmailNotifications.Services;

public record MailMessage(
    string To,
    string Subject,
    string HtmlBody,
    string? PlainTextBody = null,
    string? FromName = null,
    string? FromAddress = null,
    string? ReplyTo = null,
    IReadOnlyList<string>? Tags = null);

public interface IMailProvider
{
    string ProviderName { get; }

    Task<string?> SendAsync(MailMessage message, CancellationToken ct = default);
}
