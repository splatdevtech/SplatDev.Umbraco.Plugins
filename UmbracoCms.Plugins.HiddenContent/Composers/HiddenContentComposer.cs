using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using UmbracoCms.Plugins.HiddenContent.Services;

namespace UmbracoCms.Plugins.HiddenContent.Composers;

public class HiddenContentComposer : IComposer
{
    public void Compose(IUmbracoBuilder builder)
    {
        builder.Services.AddScoped<IHiddenContentService, HiddenContentService>();
    }
}
