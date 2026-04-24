using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using SplatDev.Umbraco.Plugins.Backups.Services;

namespace SplatDev.Umbraco.Plugins.Backups.Composers;

public class BackupsComposer : IComposer
{
    public void Compose(IUmbracoBuilder builder)
    {
        builder.Services.AddScoped<IBackupsService, BackupsService>();
    }
}
