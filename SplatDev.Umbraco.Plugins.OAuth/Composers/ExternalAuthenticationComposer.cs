using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using SplatDev.Umbraco.Plugins.OAuth.Extensions;
using SplatDev.Umbraco.Plugins.OAuth.Providers;

using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;

namespace SplatDev.Umbraco.Plugins.OAuth.Composers
{
    public class ExternalAuthenticationComposer : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            var config = builder.Config;

            if (!string.IsNullOrEmpty(config.GetValue<string>("OAuth:Applications:Google:ClientId")))
            {
                builder.Services.ConfigureOptions<GoogleMemberExternalLoginProviderOptions>();
                builder.AddGoogleMemberAuthentication();
            }

            if (!string.IsNullOrEmpty(config.GetValue<string>("OAuth:Applications:Facebook:AppId")))
            {
                builder.Services.ConfigureOptions<FacebookMemberExternalLoginProviderOptions>();
                builder.AddFacebookMemberAuthentication();
            }

            if (!string.IsNullOrEmpty(config.GetValue<string>("OAuth:Applications:X:ConsumerKey")))
            {
                builder.Services.ConfigureOptions<XMemberExternalLoginProviderOptions>();
                builder.AddXMemberAuthentication();
            }
        }
    }
}
