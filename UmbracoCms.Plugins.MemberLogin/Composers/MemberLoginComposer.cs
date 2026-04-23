using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using UmbracoCms.Plugins.MemberLogin.Services;

namespace UmbracoCms.Plugins.MemberLogin.Composers;

public class MemberLoginComposer : IComposer
{
    public void Compose(IUmbracoBuilder builder)
    {
        builder.Services.AddScoped<IMemberLoginService, MemberLoginService>();
    }
}
