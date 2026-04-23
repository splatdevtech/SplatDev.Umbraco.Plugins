using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using UmbracoCms.Plugins.Backups.Services;

namespace UmbracoCms.Plugins.Backups.Composers;

public class BackupsComposer : IComposer
{
    public void Compose(IUmbracoBuilder builder)
    {
        builder.Services.AddScoped<IBackupsService, BackupsService>();
    }
}
