using Microsoft.Extensions.DependencyInjection;

using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Plugins.RedirectManager.Controllers;
using Umbraco.Plugins.RedirectManager.Repositories;

namespace Umbraco.Plugins.RedirectManager.Composers
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
