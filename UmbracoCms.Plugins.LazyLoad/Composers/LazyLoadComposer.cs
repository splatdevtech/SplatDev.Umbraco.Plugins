using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using UmbracoCms.Plugins.LazyLoad.Services;
using UmbracoCms.Plugins.LazyLoad.TagHelpers;

namespace UmbracoCms.Plugins.LazyLoad.Composers;

public class LazyLoadComposer : IComposer
{
    public void Compose(IUmbracoBuilder builder)
    {
        builder.Services.AddScoped<ILazyLoadService, LazyLoadService>();
        builder.Services.AddTransient<LazyImageTagHelper>();
        builder.Services.AddTransient<LazyIframeTagHelper>();
    }
}
