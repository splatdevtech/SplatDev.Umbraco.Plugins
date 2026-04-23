using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using UmbracoCms.Plugins.PasswordSettings.Models;
using UmbracoCms.Plugins.PasswordSettings.Services;

namespace UmbracoCms.Plugins.PasswordSettings.Composers;

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
