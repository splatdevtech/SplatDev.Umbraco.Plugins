using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.Configuration.Models;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Web.Common.ApplicationBuilder;
using SplatDev.Umbraco.Plugins.CustomLogin.Configuration;
using SplatDev.Umbraco.Plugins.CustomLogin.Middleware;
using SplatDev.Umbraco.Plugins.CustomLogin.Services;

namespace SplatDev.Umbraco.Plugins.CustomLogin.Composers;

public class CustomLoginComposer : IComposer
{
    public void Compose(IUmbracoBuilder builder)
    {
        builder.Services.AddSingleton<ICustomLoginService, CustomLoginService>();

        builder.Services.AddSingleton<IPostConfigureOptions<ContentSettings>, CustomLoginContentSettingsConfigurer>();
        builder.Services.AddSingleton<IPostConfigureOptions<SecuritySettings>, CustomLoginSecuritySettingsConfigurer>();

        builder.Services.Configure<UmbracoPipelineOptions>(options =>
        {
            options.AddFilter(new UmbracoPipelineFilter("SplatDev.CustomLogin")
            {
                PrePipeline = app =>
                {
                    app.UseMiddleware<CustomLoginAssetsMiddleware>();
                },
            });
        });
    }
}
