using FormBuilder.Extension.Interfaces;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using System.Net;
using System.Net.Mail;

namespace FormBuilder.Extension.Services;

public class EmailService(IOptions<EmailServiceOptions> options, ILogger<EmailService> logger) : IEmailService
{
    private readonly EmailServiceOptions _config = options.Value;
    private readonly ILogger<EmailService> _logger = logger;

    public async Task SendAsync(string to, string subject, string body, string? from = null)
    {
        if (string.IsNullOrEmpty(to))
            throw new ArgumentException("Recipient email address cannot be null or empty.", nameof(to));

        try
        {
            using var smtpClient = new SmtpClient(_config.SmtpHost, _config.SmtpPort)
            {
                Credentials = new NetworkCredential(_config.SmtpUsername, _config.SmtpPassword),
                EnableSsl = _config.EnableSsl,
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(from ?? _config.DefaultFromAddress, _config.DefaultFromName),
                Subject = subject,
                Body = body,
                IsBodyHtml = true,
            };

            mailMessage.To.Add(to);

            await smtpClient.SendMailAsync(mailMessage);
            _logger.LogInformation("Email sent successfully to {to}.", to);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {to}.", to);
            throw;
        }
    }

    public async Task SendToMultipleAsync(string[] recipients, string subject, string body, string? from = null)
    {
        if (recipients == null || recipients.Length == 0)
            throw new ArgumentException("Recipients list cannot be null or empty.", nameof(recipients));

        try
        {
            using var smtpClient = new SmtpClient(_config.SmtpHost, _config.SmtpPort)
            {
                Credentials = new NetworkCredential(_config.SmtpUsername, _config.SmtpPassword),
                EnableSsl = _config.EnableSsl,
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(from ?? _config.DefaultFromAddress, _config.DefaultFromName),
                Subject = subject,
                Body = body,
                IsBodyHtml = true,
            };

            foreach (var recipient in recipients)
                mailMessage.To.Add(recipient);

            await smtpClient.SendMailAsync(mailMessage);
            _logger.LogInformation("Email sent to {count} recipients.", recipients.Length);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to multiple recipients.");
            throw;
        }
    }
}
