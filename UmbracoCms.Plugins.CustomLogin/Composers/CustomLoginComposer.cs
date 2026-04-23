using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using UmbracoCms.Plugins.CustomLogin.Services;

namespace UmbracoCms.Plugins.CustomLogin.Composers;

public class CustomLoginComposer : IComposer
{
    public void Compose(IUmbracoBuilder builder)
    {
        builder.Services.AddScoped<ICustomLoginService, CustomLoginService>();
    }
}
