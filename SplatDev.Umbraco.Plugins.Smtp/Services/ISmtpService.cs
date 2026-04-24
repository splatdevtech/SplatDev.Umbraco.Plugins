using SplatDev.Umbraco.Plugins.Smtp.Models;

namespace SplatDev.Umbraco.Plugins.Smtp.Services;

public interface ISmtpService
{
    SmtpSettings GetSettings();
    Task<SmtpTestResult> TestConnectionAsync(SmtpSettings settings);
}
