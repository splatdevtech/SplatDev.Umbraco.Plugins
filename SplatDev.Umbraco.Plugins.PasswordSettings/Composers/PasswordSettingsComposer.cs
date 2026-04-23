using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using SplatDev.Umbraco.Plugins.PasswordSettings.Models;
using SplatDev.Umbraco.Plugins.PasswordSettings.Services;

namespace SplatDev.Umbraco.Plugins.PasswordSettings.Composers;

public class PasswordSettingsComposer : IComposer
{
    public void Compose(IUmbracoBuilder builder)
    {
        builder.Services.AddDbContext<PasswordSettingsDbContext>(options =>
            options.UseSqlServer(
                builder.Config.GetConnectionString("umbracoDbDSN") ?? string.Empty));

        builder.Services.AddScoped<IPasswordSettingsService, PasswordSettingsService>();
    }
}
