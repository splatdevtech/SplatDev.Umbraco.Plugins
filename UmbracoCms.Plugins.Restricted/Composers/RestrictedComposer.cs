using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using UmbracoCms.Plugins.Restricted.Services;

namespace UmbracoCms.Plugins.Restricted.Composers;

public class RestrictedComposer : IComposer
{
    public void Compose(IUmbracoBuilder builder)
    {
        builder.Services.AddScoped<IRestrictedContentService, RestrictedContentService>();
    }
}
