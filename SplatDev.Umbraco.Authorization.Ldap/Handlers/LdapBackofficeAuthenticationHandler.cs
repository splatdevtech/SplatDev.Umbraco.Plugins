using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using SplatDev.Umbraco.Authorization.Ldap.Extensions;
using SplatDev.Umbraco.Authorization.Ldap.Models;
using SplatDev.Umbraco.Authorization.Ldap.Services;

using System.Runtime.Versioning;
using System.Security.Claims;
using System.Text.Json;

using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Models.Membership;
using Umbraco.Cms.Core.Security;
using Umbraco.Cms.Core.Services;
#if !NET10_0_OR_GREATER
using Umbraco.Cms.Web.BackOffice.Security;
#endif
using Umbraco.Extensions;

namespace SplatDev.Umbraco.Authorization.Ldap.Handlers
{
    public class LdapBackofficeAuthenticationHandler(
        IOptions<LdapConfiguration> ldapOptions,
        IServiceProvider services,
        IServiceScopeFactory serviceScopeFactory)
    {
        private readonly ILogger<LdapBackofficeAuthenticationHandler> _logger = services.GetRequiredService<ILogger<LdapBackofficeAuthenticationHandler>>();
        private readonly IUserService _userService = services.GetRequiredService<IUserService>();
        private readonly IBackOfficeUserManager _userManager = services.GetRequiredService<IBackOfficeUserManager>();
        private readonly LdapService _ldapService = services.GetRequiredService<LdapService>();
#if !NET10_0_OR_GREATER
        private readonly BackOfficeSignInManager _signInManager = services.GetRequiredService<BackOfficeSignInManager>();
#endif
        private readonly LdapConfiguration _ldapConfiguration = ldapOptions.Value;
        private readonly IServiceScopeFactory _serviceScopeFactory = serviceScopeFactory;

        [SupportedOSPlatform("windows")]
        public bool TryValidateUser(string username, out LdapUser? user)
        {
            user = _ldapService.GetUser(username);
            return user is not null && user.IsAllowed;
        }

        public async Task<HandleRequestResult> BackOfficeLogin(LdapUser user)
        {
#if !NET10_0_OR_GREATER
            IUser? existingUser = null;
            bool isNewUser = false;

            _logger.LogInformation("AD User: {user}", JsonSerializer.Serialize(user));

            if (user is not null && user.IsAllowed)
            {
                var tuple = user.CreateBackOfficeUser(_userService, _ldapConfiguration, _logger);
                existingUser = tuple.Item1 as IUser;
                user = tuple.Item2 as LdapUser;
                isNewUser = tuple.Item3;

                _logger.LogInformation("Umbraco User: {user}", JsonSerializer.Serialize(existingUser));
            }
            _ = user;
            if (existingUser is not null)
            {
                BackOfficeIdentityUser? identityUser = await _userManager.FindByEmailAsync(existingUser.Email);

                if (identityUser is not null)
                {
                    DateTime? previousLastLoginDate = identityUser.LastLoginDateUtc;

                    AuthenticationProperties properties = _signInManager.ConfigureExternalAuthenticationProperties(
                        _ldapConfiguration.DisplayName, "", Constants.System.RootString);

                    ClaimsPrincipal principal = await _signInManager.CreateUserPrincipalAsync(identityUser);

                    AuthenticationTicket ticket = new(principal, properties, Constants.Security.BackOfficeExternalAuthenticationType);

                    await _signInManager.RefreshSignInAsync(identityUser);
                    identityUser.LastLoginDateUtc = previousLastLoginDate;
                    await _userManager.UpdateAsync(identityUser);

                    if (isNewUser)
                    {
                        _logger.LogInformation("New user created and logged in: {email}", existingUser.Email);
                    }

                    return HandleRequestResult.Success(ticket);
                }
            }
            return HandleRequestResult.Fail(new Exception($"{existingUser?.Name} Could not log into the backoffice"));
#else
            // TODO: Implement Umbraco 17 backoffice login via Management API
            await Task.CompletedTask;
            return HandleRequestResult.Fail(new Exception("Umbraco 17 backoffice login not yet implemented"));
#endif
        }
    }
}
