using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using SplatDev.Umbraco.Authorization.Ldap.Handlers;
using SplatDev.Umbraco.Authorization.Ldap.Models;

using System.Runtime.Versioning;

using Umbraco.Cms.Core.Models.Membership;
using Umbraco.Cms.Core.Services;

namespace SplatDev.Umbraco.Authorization.Ldap.Controllers
{
    public class ImpersonateController(
        LdapBackofficeAuthenticationHandler handler,
        IOptions<LdapConfiguration> configuration,
        ILogger<SsoController> logger,
        IUserService userService) : Controller
    {
        private readonly LdapBackofficeAuthenticationHandler _handler = handler;
        private readonly LdapConfiguration _configuration = configuration.Value;
        private readonly ILogger<SsoController> _logger = logger;
        private readonly IUserService _userService = userService;

        [HttpGet]
        [SupportedOSPlatform("windows")]
        public async Task<IActionResult> Index(string username)
        {
            if (!_configuration.AllowImpersonation) return Forbid();
            if (username is null) return NotFound();

            var currentUsername = User.Identity?.Name;
            if (currentUsername is null) return BadRequest();

            var umbracoUser = _userService.GetByUsername(currentUsername);

            if (umbracoUser is null || umbracoUser.UserState != UserState.Active)
            {
                _logger.LogWarning("Current user is not a valid active Umbraco user: {currentUsername}", currentUsername);
                return RedirectPermanent("/status-code/401");
            }

            if (!umbracoUser.Groups.Any(g => g.Alias == "admin"))
            {
                _logger.LogWarning("Current user is not in the backoffice admin group: {currentUsername}", currentUsername);
                return RedirectPermanent("/status-code/401");
            }

            if (_handler.TryValidateUser(username, out LdapUser? user))
            {
                var result = await _handler.BackOfficeLogin(user!);
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
