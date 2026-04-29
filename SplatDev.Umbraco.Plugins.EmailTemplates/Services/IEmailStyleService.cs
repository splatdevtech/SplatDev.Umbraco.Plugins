using SplatDev.Umbraco.Plugins.EmailTemplates.Models;

namespace SplatDev.Umbraco.Plugins.EmailTemplates.Services;

public interface IEmailStyleService
{
    EmailStyle Get();
    EmailStyle Save(EmailStyle style);
}
