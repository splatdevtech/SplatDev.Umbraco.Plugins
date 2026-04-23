using Microsoft.Extensions.DependencyInjection;

using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Plugins.Mailer.Services;

namespace Umbraco.Plugins.Mailer.Composers
{
    public class MailerComposer : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            builder.Services.AddTransient<MailerService>();
            builder.Services.AddTransient<MicrosoftGraphMailerService>();
        }
    }
}
