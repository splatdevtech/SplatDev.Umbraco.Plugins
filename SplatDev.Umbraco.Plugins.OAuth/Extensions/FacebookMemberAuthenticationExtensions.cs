using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using SplatDev.Umbraco.Plugins.OAuth.Providers;

using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Extensions;

namespace SplatDev.Umbraco.Plugins.OAuth.Extensions
{
    public static class FacebookMemberAuthenticationExtensions
    {
        public static IUmbracoBuilder AddFacebookMemberAuthentication(this IUmbracoBuilder builder)
        {
            builder.Services.ConfigureOptions<FacebookMemberExternalLoginProviderOptions>();

            builder.AddMemberExternalLogins(logins =>
            {
                logins.AddMemberLogin(
                    memberAuthenticationBuilder =>
                    {
                        // The scheme must be set with this method to work for the back office
                        var schemeName =
                            memberAuthenticationBuilder.SchemeForMembers(FacebookMemberExternalLoginProviderOptions
                                .SchemeName);

                        ArgumentNullException.ThrowIfNull(schemeName);

                        var config = builder.Config;

                        memberAuthenticationBuilder.AddFacebook(
                            schemeName,
                            options =>
                            {
                                options.CallbackPath = config.GetValue<string>("OAuth:Applications:Facebook:CallbackPath"); ///oauth%3Fp=facebook
                                options.ClientId = config.GetValue<string>("OAuth:Applications:Facebook:AppId") ?? "";
                                options.ClientSecret = config.GetValue<string>("OAuth:Applications:Facebook:AppSecret") ?? "";
                            });
                    });
            });
            return builder;
        }
    }
}
