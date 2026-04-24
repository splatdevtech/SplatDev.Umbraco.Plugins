using Umbraco.Cms.Core.Composing;
using SplatDev.Umbraco.Plugins.Dropzone.Services;

namespace SplatDev.Umbraco.Plugins.Dropzone.Composers;

public class PluginComposer : IComposer
{
    public void Compose(IUmbracoBuilder builder)
    {
        builder.Services.AddScoped<IDropzoneService, DropzoneService>();
    }
}
