using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using UmbracoCms.Plugins.Smtp.Models;

namespace UmbracoCms.Plugins.Smtp.Services;

public class SmtpService : ISmtpService
{
    private readonly IConfiguration _configuration;

    public SmtpService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public SmtpSettings GetSettings()
    {
        var section = _configuration.GetSection("SmtpSettings");
        return new SmtpSettings
        {
            Host = section["Host"] ?? string.Empty,
            Port = int.TryParse(section["Port"], out var port) ? port : 587,
            Username = section["Username"] ?? string.Empty,
            Password = section["Password"] ?? string.Empty,
            EnableSsl = bool.TryParse(section["EnableSsl"], out var ssl) ? ssl : true,
            FromEmail = section["FromEmail"] ?? string.Empty,
            FromName = section["FromName"] ?? string.Empty
        };
    }

    public async Task<SmtpTestResult> TestConnectionAsync(SmtpSettings settings)
    {
        try
        {
            using var client = new SmtpClient(settings.Host, settings.Port)
            {
                EnableSsl = settings.EnableSsl,
                Credentials = new NetworkCredential(settings.Username, settings.Password),
                DeliveryMethod = SmtpDeliveryMethod.Network,
                Timeout = 10000
            };

            var from = new MailAddress(settings.FromEmail, settings.FromName);
            var message = new MailMessage(from, from)
            {
                Subject = "Umbraco SMTP Test",
                Body = "This is a test email sent from the Umbraco SMTP Plugin to verify your configuration."
            };

            await client.SendMailAsync(message);

            return new SmtpTestResult
            {
                Success = true,
                Message = $"Test email sent successfully to {settings.FromEmail}."
            };
        }
        catch (Exception ex)
        {
            return new SmtpTestResult
            {
                Success = false,
                Message = "Failed to send test email.",
                Error = ex.Message
            };
        }
    }
}
