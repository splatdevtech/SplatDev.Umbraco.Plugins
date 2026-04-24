using Microsoft.AspNetCore.Http;

using SplatDev.Umbraco.Authorization.Ldap.Models;
using SplatDev.Umbraco.Authorization.Ldap.Services;

using System.Runtime.Versioning;

namespace SplatDev.Umbraco.Authorization.Ldap.Middleware
{
    public class LdapFrontEndAuthorizedMiddleware(LdapService ldapService) : IMiddleware
    {
        private readonly LdapService _ldapService = ldapService;

        [SupportedOSPlatform("windows")]
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            if (context.Request.Path.Value!.Contains("/status-codes/") ||
                context.Request.Path.Value!.Contains("/css/") ||
                context.Request.Path.Value!.Contains("/media/") ||
                context.Request.Path.Value!.Contains("/scripts/") ||
                context.Request.Path.Value!.Contains("/images/") ||
                context.Request.Path.Value!.Contains("/assets/") ||
                context.Request.Path.Value!.Contains("/App_Plugins/") ||
                context.Request.Path.Value!.Contains("/umbraco") ||
                context.Request.Path.Value!.StartsWith("/impersonate"))
            {
                context.Response.StatusCode = StatusCodes.Status200OK;
                await next(context);
                return;
            }

            if (!IsAuthorized(context))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.Redirect(context.Request.PathBase + "/status-codes/401");
            }
            await next(context);
        }

        [SupportedOSPlatform("windows")]
        private bool IsAuthorized(HttpContext context)
        {
            LdapUser? user = _ldapService.GetUser();
            context.Items["User"] = user;
            return user is not null && user.IsAllowed;
        }
    }
}
