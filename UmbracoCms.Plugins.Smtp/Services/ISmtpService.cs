using UmbracoCms.Plugins.Smtp.Models;

namespace UmbracoCms.Plugins.Smtp.Services;

public interface ISmtpService
{
    SmtpSettings GetSettings();
    Task<SmtpTestResult> TestConnectionAsync(SmtpSettings settings);
}
