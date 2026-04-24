using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.IISIntegration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using SplatDev.Umbraco.Authorization.Ldap.Controllers;
using SplatDev.Umbraco.Authorization.Ldap.Handlers;
using SplatDev.Umbraco.Authorization.Ldap.Middleware;
using SplatDev.Umbraco.Authorization.Ldap.Models;
using SplatDev.Umbraco.Authorization.Ldap.Services;
using SplatDev.Umbraco.Authorization.Ldap.StartupFilters;

using System.Runtime.Versioning;

using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.Security;
using Umbraco.Cms.Web.Common.ApplicationBuilder;
using Umbraco.Extensions;

namespace SplatDev.Umbraco.Authorization.Ldap.Composers
{
    public class LdapComposer : IComposer
    {
        [SupportedOSPlatform("windows")]
        public void Compose(IUmbracoBuilder builder)
        {
            builder.Services.AddHttpContextAccessor();

            if (builder.Config.GetValue<bool>("LdapConfiguration:EnableSSO") == false) return;

            builder.Services.Configure<LdapConfiguration>(builder.Config.GetSection("LdapConfiguration"));
            builder.Services.AddSingleton<LdapService>();
            builder.Services.AddTransient<LdapFrontEndAuthorizedMiddleware>();
            builder.Services.AddTransient<LdapBackofficeAuthenticationHandler>();
            builder.Services.AddTransient<IStartupFilter, LdapStartupFilter>();

            builder.Services.AddAuthentication(IISDefaults.AuthenticationScheme);
            builder.Services.AddAuthorization();

            builder.Services.AddDistributedMemoryCache();

            builder.Services.Configure<UmbracoPipelineOptions>(options =>
            {
                options.AddFilter(new UmbracoPipelineFilter("LdapMiddleware")
                {
                    PrePipeline = app => app.UseMiddleware<LdapFrontEndAuthorizedMiddleware>()
                });
            });

            builder.Services.Configure<UmbracoPipelineOptions>(options =>
            {
                options.AddFilter(new UmbracoPipelineFilter(nameof(SsoController))
                {
                    Endpoints = app => app.UseEndpoints(endpoints =>
                    {
                        endpoints.MapControllerRoute(
                            "Single Sign-On controller",
                            "/sso",
                            new { Controller = "Sso", Action = "Index" });
                        endpoints.MapControllerRoute(
                            "Impersonate controller",
                            "/impersonate",
                            new { Controller = "Impersonate", Action = "Index" });
                    })
                });
            });

            builder.Services.AddUnique<IBackOfficeUserPasswordChecker, LdapBackOfficePasswordChecker>();
        }
    }
}
