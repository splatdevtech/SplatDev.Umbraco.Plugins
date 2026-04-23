using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using UmbracoCms.Plugins.Analytics.Services;

namespace UmbracoCms.Plugins.Analytics.Composers;

public class AnalyticsComposer : IComposer
{
    public void Compose(IUmbracoBuilder builder)
    {
        builder.Services.AddScoped<IAnalyticsService, AnalyticsService>();
    }
}
