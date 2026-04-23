using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using SplatDev.Umbraco.Plugins.Analytics.Services;

namespace SplatDev.Umbraco.Plugins.Analytics.Composers;

public class AnalyticsComposer : IComposer
{
    public void Compose(IUmbracoBuilder builder)
    {
        builder.Services.AddScoped<IAnalyticsService, AnalyticsService>();
    }
}
