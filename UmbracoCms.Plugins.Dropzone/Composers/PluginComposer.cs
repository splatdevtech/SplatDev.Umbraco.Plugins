using Umbraco.Cms.Core.Composing;
using UmbracoCms.Plugins.Dropzone.Services;

namespace UmbracoCms.Plugins.Dropzone.Composers;

public class PluginComposer : IComposer
{
    public void Compose(IUmbracoBuilder builder)
    {
        builder.Services.AddScoped<IDropzoneService, DropzoneService>();
    }
}
