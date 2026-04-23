using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using UmbracoCms.Plugins.Smtp.Services;

namespace UmbracoCms.Plugins.Smtp.Composers;

public class SmtpComposer : IComposer
{
    public void Compose(IUmbracoBuilder builder)
    {
        builder.Services.AddScoped<ISmtpService, SmtpService>();
    }
}
