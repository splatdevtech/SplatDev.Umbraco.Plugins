using SplatDev.Umbraco.Plugins.Mailer.Models;

namespace SplatDev.Umbraco.Plugins.Mailer.Services
{
    public interface IEmailService<T> where T : class
    {
        ViewRenderService ViewRenderer { get; }
        Task<string> GenerateEmailContentAsync(EmailModel<T> model);
        Task SendEmailAsync(EmailModel<T> model);
    }
}
