using Umbraco.Cms.Core.Composing;
using UmbracoCms.Plugins.Exif.Services;

namespace UmbracoCms.Plugins.Exif.Composers;

public class PluginComposer : IComposer
{
    public void Compose(IUmbracoBuilder builder)
    {
        builder.Services.AddScoped<IExifService, ExifService>();
    }
}
