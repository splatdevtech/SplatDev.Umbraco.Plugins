using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;

using UmbracoCms.Plugins.Settings.Models;
using UmbracoCms.Plugins.Settings.Services;

namespace UmbracoCms.Plugins.Settings.Composers
{
    public class SettingsComposer : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            builder.Services.AddDbContext<SettingsDbContext>(options =>
                options.UseSqlServer(builder.Config.GetConnectionString("umbracoDbDSN") ?? string.Empty));

            builder.Services.AddScoped<ISettingsService, SettingsService>();
        }
    }
}
