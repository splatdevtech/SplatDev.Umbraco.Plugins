using Microsoft.Extensions.Options;
using Umbraco.Cms.Core.Configuration.Models;
using SplatDev.Umbraco.Plugins.CustomLogin.Services;

namespace SplatDev.Umbraco.Plugins.CustomLogin.Configuration;

public class CustomLoginContentSettingsConfigurer(ICustomLoginService service)
    : IPostConfigureOptions<ContentSettings>
{
    public void PostConfigure(string? name, ContentSettings options)
    {
        var settings = service.GetSettings();

        if (!string.IsNullOrWhiteSpace(settings.LogoUrl))
            options.LoginLogoImage = settings.LogoUrl;

        if (!string.IsNullOrWhiteSpace(settings.LogoAlternativeUrl))
            options.LoginLogoImageAlternative = settings.LogoAlternativeUrl;

        if (!string.IsNullOrWhiteSpace(settings.BackgroundImageUrl))
            options.LoginBackgroundImage = settings.BackgroundImageUrl;
    }
}
