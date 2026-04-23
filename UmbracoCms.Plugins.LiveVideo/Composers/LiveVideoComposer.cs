using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using UmbracoCms.Plugins.LiveVideo.Services;

namespace UmbracoCms.Plugins.LiveVideo.Composers;

public class LiveVideoComposer : IComposer
{
    public void Compose(IUmbracoBuilder builder)
    {
        builder.Services.AddScoped<ILiveVideoService, LiveVideoService>();
    }
}
