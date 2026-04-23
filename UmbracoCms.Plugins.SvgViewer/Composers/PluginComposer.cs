using Umbraco.Cms.Core.Composing;
using UmbracoCms.Plugins.SvgViewer.Services;

namespace UmbracoCms.Plugins.SvgViewer.Composers;

public class PluginComposer : IComposer
{
    public void Compose(IUmbracoBuilder builder)
    {
        builder.Services.AddScoped<ISvgViewerService, SvgViewerService>();
    }
}
