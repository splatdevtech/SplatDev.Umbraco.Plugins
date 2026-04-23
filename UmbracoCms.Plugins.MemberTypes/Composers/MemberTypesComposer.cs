using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using UmbracoCms.Plugins.MemberTypes.Services;

namespace UmbracoCms.Plugins.MemberTypes.Composers;

public class MemberTypesComposer : IComposer
{
    public void Compose(IUmbracoBuilder builder)
    {
        builder.Services.AddScoped<IMemberTypesService, MemberTypesService>();
    }
}
