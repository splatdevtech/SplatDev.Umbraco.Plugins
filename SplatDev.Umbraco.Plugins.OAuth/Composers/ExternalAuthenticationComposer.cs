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
            builder.Services.ConfigureOptions<GoogleMemberExternalLoginProviderOptions>();
            builder.AddGoogleMemberAuthentication();

            builder.Services.ConfigureOptions<FacebookMemberExternalLoginProviderOptions>();
            builder.AddFacebookMemberAuthentication();

            builder.Services.ConfigureOptions<XMemberExternalLoginProviderOptions>();
            builder.AddXMemberAuthentication();
        }
    }
}
