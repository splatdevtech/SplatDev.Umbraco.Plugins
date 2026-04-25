using SplatDev.Umbraco.Authorization.Ldap.Services;

using System.Runtime.Versioning;

using Umbraco.Cms.Core.Security;

namespace SplatDev.Umbraco.Authorization.Ldap.Handlers
{
    public class LdapBackOfficePasswordChecker(LdapService ldapService) : IBackOfficeUserPasswordChecker
    {
        private readonly LdapService _ldapService = ldapService;

        [SupportedOSPlatform("windows")]
        public Task<BackOfficeUserPasswordCheckerResult> CheckPasswordAsync(BackOfficeIdentityUser user, string password)
        {
            ArgumentNullException.ThrowIfNull(user);

            var ldapUser = _ldapService.GetUser(user.UserName);
            var result = ldapUser is not null && ldapUser.IsAllowedBackOffice
                ? BackOfficeUserPasswordCheckerResult.ValidCredentials
                : BackOfficeUserPasswordCheckerResult.InvalidCredentials;

            return Task.FromResult(result);
        }
    }
}
