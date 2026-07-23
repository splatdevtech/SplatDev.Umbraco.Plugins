using FormBuilder.Extension.Interfaces;

using Microsoft.Extensions.Logging;

using System.Net;
using System.Net.Mail;

namespace FormBuilder.Extension.Services
{
    /// <summary>
    /// Service for handling email sending functionality.
    /// </summary>
    public class EmailService(ILogger<EmailService> logger) : IEmailService
    {
        private readonly ILogger<EmailService> _logger = logger;

        // SMTP configuration (can be loaded from appsettings.json or environment variables)
        private readonly string _smtpHost = "smtp.example.com"; // Replace with your SMTP server
        private readonly int _smtpPort = 587; // Default SMTP port
        private readonly string _smtpUsername = "your-smtp-username"; // Replace with your SMTP username
        private readonly string _smtpPassword = "your-smtp-password"; // Replace with your SMTP password
        private readonly string _defaultSender = "no-reply@example.com"; // Default sender email

        public async Task SendAsync(string to, string subject, string body, string? from = null)
        {
            if (string.IsNullOrEmpty(to))
                throw new ArgumentException("Recipient email address cannot be null or empty.", nameof(to));

            try
            {
                using var smtpClient = new SmtpClient(_smtpHost, _smtpPort)
                {
                    Credentials = new NetworkCredential(_smtpUsername, _smtpPassword),
                    EnableSsl = true // Use SSL for secure communication
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(from ?? _defaultSender),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true // Set to true if the body contains HTML content
                };

                mailMessage.To.Add(to);

                await smtpClient.SendMailAsync(mailMessage);
                _logger.LogInformation("Email sent successfully to {to}.", to);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {to}. Error: {message}", to, ex.Message);
                throw;
            }
            await Task.FromResult(0);
        }

        public async Task SendToMultipleAsync(string[] recipients, string subject, string body, string? from = null)
        {
            if (recipients == null || recipients.Length == 0)
                throw new ArgumentException("Recipients list cannot be null or empty.", nameof(recipients));

            try
            {
                using var smtpClient = new SmtpClient(_smtpHost, _smtpPort)
                {
                    Credentials = new NetworkCredential(_smtpUsername, _smtpPassword),
                    EnableSsl = true // Use SSL for secure communication
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(from ?? _defaultSender),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true // Set to true if the body contains HTML content
                };

                foreach (var recipient in recipients)
                {
                    mailMessage.To.Add(recipient);
                }

                await smtpClient.SendMailAsync(mailMessage);
                _logger.LogInformation("Email sent successfully to multiple recipients: {recipients}.", string.Join(", ", recipients));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to multiple recipients. Error: {message}", ex.Message);
                throw;
            }
        }
    }
}
