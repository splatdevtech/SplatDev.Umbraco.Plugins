using Umbraco.Cms.Core.Composing;
using SplatDev.Umbraco.Plugins.SvgViewer.Services;

namespace SplatDev.Umbraco.Plugins.SvgViewer.Composers;

public class PluginComposer : IComposer
{
    public void Compose(IUmbracoBuilder builder)
    {
        builder.Services.AddScoped<ISvgViewerService, SvgViewerService>();
    }
}
