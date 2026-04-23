using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using UmbracoCms.Plugins.VideoPreview.Services;

namespace UmbracoCms.Plugins.VideoPreview.Composers;

public class VideoPreviewComposer : IComposer
{
    public void Compose(IUmbracoBuilder builder)
    {
        builder.Services.AddHttpClient();
        builder.Services.AddScoped<IVideoPreviewService, VideoPreviewService>();
    }
}
