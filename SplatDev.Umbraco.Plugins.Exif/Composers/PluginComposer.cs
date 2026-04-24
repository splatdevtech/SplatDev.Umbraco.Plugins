using Umbraco.Cms.Core.Composing;
using SplatDev.Umbraco.Plugins.Exif.Services;

namespace SplatDev.Umbraco.Plugins.Exif.Composers;

public class PluginComposer : IComposer
{
    public void Compose(IUmbracoBuilder builder)
    {
        builder.Services.AddScoped<IExifService, ExifService>();
    }
}
