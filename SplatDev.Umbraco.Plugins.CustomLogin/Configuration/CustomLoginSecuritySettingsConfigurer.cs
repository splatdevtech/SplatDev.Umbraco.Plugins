using Microsoft.Extensions.Options;
using Umbraco.Cms.Core.Configuration.Models;
using SplatDev.Umbraco.Plugins.CustomLogin.Services;

namespace SplatDev.Umbraco.Plugins.CustomLogin.Configuration;

public class CustomLoginSecuritySettingsConfigurer(ICustomLoginService service)
    : IPostConfigureOptions<SecuritySettings>
{
    public void PostConfigure(string? name, SecuritySettings options)
    {
        var settings = service.GetSettings();
        options.AllowPasswordReset = settings.AllowPasswordReset;
    }
}
