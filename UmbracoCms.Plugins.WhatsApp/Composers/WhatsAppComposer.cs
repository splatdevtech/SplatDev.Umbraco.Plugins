using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using UmbracoCms.Plugins.WhatsApp.Services;

namespace UmbracoCms.Plugins.WhatsApp.Composers;

public class WhatsAppComposer : IComposer
{
    public void Compose(IUmbracoBuilder builder)
    {
        builder.Services.AddScoped<IWhatsAppService, WhatsAppService>();
    }
}
