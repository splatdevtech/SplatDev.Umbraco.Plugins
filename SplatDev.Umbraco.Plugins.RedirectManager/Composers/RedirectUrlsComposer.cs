using Microsoft.Extensions.DependencyInjection;

using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using SplatDev.Umbraco.Plugins.RedirectManager.Controllers;
using SplatDev.Umbraco.Plugins.RedirectManager.Repositories;

namespace SplatDev.Umbraco.Plugins.RedirectManager.Composers
{
    public class RedirectUrlsComposer : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            //Repository
            builder.Services.AddScoped<RedirectUrlsRepository>();
            builder.Services.AddScoped<FrontEndRedirectionController>();
        }
    }
}
