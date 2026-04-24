using FormBuilder.Core.Configuration;
using FormBuilder.Core.Security.Interfaces;
using FormBuilder.Web.Extensions;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using System.Security.Claims;

using Umbraco.Cms.Core.Models.Membership;
using Umbraco.Cms.Core.Services;
using Umbraco.Extensions;

namespace FormBuilder.Web.HttpModules
{
    /// <summary>
    /// Ensures that requests to the form's upload path are authorized and only authenticated back-office users
    /// with permissions for the form have access.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the     /// </remarks>
    public class ProtectFormUploadRequestsMiddleware(
      IOptions<SecuritySettings> securitySettings,
      IUserService userService,
      IFormsSecurity formsSecurity) : IMiddleware
    {
        private readonly SecuritySettings _securitySettings = securitySettings.Value;
        private readonly IUserService _userService = userService;
        private readonly IFormsSecurity _formsSecurity = formsSecurity;

        /// <inheritdoc />
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            string? str = context.Request.Path.Value;
            if (_securitySettings.DisableFileUploadAccessProtection)
                await next(context);
            else if (string.IsNullOrEmpty(str) || !str.StartsWithNormalizedPath("/media/forms/upload/", StringComparison.OrdinalIgnoreCase))
            {
                await next(context);
            }
            else
            {
                CookieAuthenticationOptions? authenticationOptions = context.RequestServices.GetRequiredService<IOptionsSnapshot<CookieAuthenticationOptions>>().Get("UmbracoBackOffice");
                if (authenticationOptions is not null)
                {
                    switch (authenticationOptions.Cookie?.Name)
                    {
                        case null:
                            break;

                        default:
                            string? requestCookie = new ChunkingCookieManager().GetRequestCookie(context, authenticationOptions.Cookie.Name);
                            if (!string.IsNullOrEmpty(requestCookie))
                            {
                                AuthenticationTicket? authenticationTicket = authenticationOptions.TicketDataFormat.Unprotect(requestCookie);
                                ClaimsIdentity? claimsIdentity = authenticationTicket?.Principal.GetUmbracoIdentity();
                                if (claimsIdentity is null)
                                {
                                    ForbidAccess(context);
                                    return;
                                }
                                IUser? byUsername = _userService.GetByUsername(claimsIdentity.Name);
                                if (byUsername is null || !byUsername.AllowedSections.Contains("forms"))
                                {
                                    ForbidAccess(context);
                                    return;
                                }
                                if (!TryParseFormIdFromRequestPath(str, out Guid formId))
                                {
                                    ForbidAccess(context);
                                    return;
                                }
                                if (!_formsSecurity.CanUserViewEntries(byUsername) || !_formsSecurity.HasAccessToForm(formId, byUsername))
                                {
                                    ForbidAccess(context);
                                    return;
                                }
                                await next(context);
                                return;
                            }
                            ForbidAccess(context);
                            return;
                    }
                }
                ForbidAccess(context);
            }
        }

        private static void ForbidAccess(HttpContext context) => context.Response.StatusCode = 403;

        private static bool TryParseFormIdFromRequestPath(string requestPath, out Guid formId)
        {
            string? str = requestPath.Split('/').FirstOrDefault(x => x.StartsWith("form_"));
            if (str is not null)
                return Guid.TryParse(str.Replace("form_", string.Empty), out formId);
            formId = Guid.Empty;
            return false;
        }
    }
}