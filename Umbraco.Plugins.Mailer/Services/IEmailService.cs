using Umbraco.Plugins.Mailer.Models;

namespace Umbraco.Plugins.Mailer.Services
{
    public interface IEmailService<T> where T : class
    {
        ViewRenderService ViewRenderer { get; }
        Task<string> GenerateEmailContentAsync(EmailModel<T> model);
        Task SendEmailAsync(EmailModel<T> model);
    }
}
