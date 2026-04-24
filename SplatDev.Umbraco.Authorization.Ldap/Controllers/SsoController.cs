using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using SplatDev.Umbraco.Authorization.Ldap.Handlers;
using SplatDev.Umbraco.Authorization.Ldap.Models;

using System.Runtime.Versioning;
using System.Security.Principal;

namespace SplatDev.Umbraco.Authorization.Ldap.Controllers
{
    public class SsoController(
        ILogger<SsoController> logger,
        IOptions<LdapConfiguration> configuration,
        IHttpContextAccessor contextAccessor,
        LdapBackofficeAuthenticationHandler handler) : Controller
    {
        private readonly LdapBackofficeAuthenticationHandler _handler = handler;
        private readonly LdapConfiguration _configuration = configuration.Value;
        private readonly ILogger<SsoController> _logger = logger;
        private readonly IHttpContextAccessor _contextAccessor = contextAccessor;

        [SupportedOSPlatform("windows")]
        public IActionResult Index()
        {
            var username = _configuration.TestUser;
            username ??= _contextAccessor.HttpContext!.User?.Identity?.Name;
            username ??= WindowsIdentity.GetCurrent().Name;
            if (username is null) return NotFound();

            if (_handler.TryValidateUser(username, out LdapUser? user))
            {
                var result = _handler.BackOfficeLogin(user!).GetAwaiter().GetResult();
                if (result.Succeeded)
                    return Redirect("/umbraco#/onboarding");

                _logger.LogWarning("{message}: {username}", result.Failure!.Message, username);
                return Redirect("/");
            }
            else
            {
                _logger.LogWarning("User not allowed: {user}", user?.DisplayName);
                return RedirectPermanent("/status-code/401");
            }
        }
    }
}
