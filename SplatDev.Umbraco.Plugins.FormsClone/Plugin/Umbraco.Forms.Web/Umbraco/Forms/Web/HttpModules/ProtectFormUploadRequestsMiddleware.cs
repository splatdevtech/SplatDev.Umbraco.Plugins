
// Type: Umbraco.Forms.Web.HttpModules.ProtectFormUploadRequestsMiddleware
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using System.Security.Claims;

using Umbraco.Cms.Core.Models.Membership;
using Umbraco.Cms.Core.Services;
using Umbraco.Extensions;
using Umbraco.Forms.Core.Configuration;
using Umbraco.Forms.Core.Security;
using Umbraco.Forms.Web.Extensions;


#nullable enable
namespace Umbraco.Forms.Web.HttpModules
{
    public class ProtectFormUploadRequestsMiddleware : IMiddleware
    {
        private readonly SecuritySettings _securitySettings;
        private readonly IUserService _userService;
        private readonly IFormsSecurity _formsSecurity;

        public ProtectFormUploadRequestsMiddleware(
          IOptions<SecuritySettings> securitySettings,
          IUserService userService,
          IFormsSecurity formsSecurity)
        {
            this._securitySettings = securitySettings.Value;
            this._userService = userService;
            this._formsSecurity = formsSecurity;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            string str = context.Request.Path.Value;
            if (this._securitySettings.DisableFileUploadAccessProtection)
                await next(context);
            else if (string.IsNullOrEmpty(str) || !str.StartsWithNormalizedPath("/media/forms/upload/", StringComparison.OrdinalIgnoreCase))
            {
                await next(context);
            }
            else
            {
                CookieAuthenticationOptions authenticationOptions = ServiceProviderServiceExtensions.GetRequiredService<IOptionsSnapshot<CookieAuthenticationOptions>>(context.RequestServices).Get("UmbracoBackOffice");
                if (authenticationOptions != null)
                {
                    switch (authenticationOptions.Cookie?.Name)
                    {
                        case null:
                            break;
                        default:
                            string requestCookie = new ChunkingCookieManager().GetRequestCookie(context, authenticationOptions.Cookie.Name);
                            if (!string.IsNullOrEmpty(requestCookie))
                            {
                                AuthenticationTicket authenticationTicket = authenticationOptions.TicketDataFormat.Unprotect(requestCookie);
                                ClaimsIdentity claimsIdentity = authenticationTicket != null ? authenticationTicket.Principal.GetUmbracoIdentity() : null;
                                if (claimsIdentity == null)
                                {
                                    ProtectFormUploadRequestsMiddleware.ForbidAccess(context);
                                    return;
                                }
                                IUser byUsername = this._userService.GetByUsername(claimsIdentity.Name);
                                if (byUsername == null || !byUsername.AllowedSections.Contains<string>("forms"))
                                {
                                    ProtectFormUploadRequestsMiddleware.ForbidAccess(context);
                                    return;
                                }
                                Guid formId;
                                if (!ProtectFormUploadRequestsMiddleware.TryParseFormIdFromRequestPath(str, out formId))
                                {
                                    ProtectFormUploadRequestsMiddleware.ForbidAccess(context);
                                    return;
                                }
                                if (!this._formsSecurity.CanUserViewEntries(byUsername) || !this._formsSecurity.HasAccessToForm(formId, byUsername))
                                {
                                    ProtectFormUploadRequestsMiddleware.ForbidAccess(context);
                                    return;
                                }
                                await next(context);
                                return;
                            }
                            ProtectFormUploadRequestsMiddleware.ForbidAccess(context);
                            return;
                    }
                }
                ProtectFormUploadRequestsMiddleware.ForbidAccess(context);
            }
        }

        private static void ForbidAccess(HttpContext context) => context.Response.StatusCode = 403;

        private static bool TryParseFormIdFromRequestPath(string requestPath, out Guid formId)
        {
            string str = requestPath.Split('/').FirstOrDefault<string>(x => x.StartsWith("form_"));
            if (str != null)
                return Guid.TryParse(str.Replace("form_", string.Empty), out formId);
            formId = Guid.Empty;
            return false;
        }
    }
}
