using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using SplatDev.Umbraco.Plugins.WhatsApp.Services;

namespace SplatDev.Umbraco.Plugins.WhatsApp.Composers;

public class WhatsAppComposer : IComposer
{
    public void Compose(IUmbracoBuilder builder)
    {
        builder.Services.AddScoped<IWhatsAppService, WhatsAppService>();
    }
}
