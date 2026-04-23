using Umbraco.Cms.Core.Security;

namespace SplatDev.Umbraco.Authorization.Ldap.Handlers
{
    public class LdapBackOfficePasswordChecker : IBackOfficeUserPasswordChecker
    {
        public async Task<BackOfficeUserPasswordCheckerResult> CheckPasswordAsync(BackOfficeIdentityUser user, string password)
        {
            await Task.FromResult(0);
            return user == null
                ? throw new ArgumentNullException(nameof(user))
                : BackOfficeUserPasswordCheckerResult.ValidCredentials;
        }
    }
}
