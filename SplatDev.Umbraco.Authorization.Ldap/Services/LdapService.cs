using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using SplatDev.Umbraco.Authorization.Ldap.Extensions;
using SplatDev.Umbraco.Authorization.Ldap.Models;

using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Runtime.Versioning;
using System.Security.Principal;

using Umbraco.Cms.Core.Cache;
using Umbraco.Extensions;

namespace SplatDev.Umbraco.Authorization.Ldap.Services
{
    public class LdapService
    {
        private readonly LdapConfiguration _ldapConfiguration;
        private readonly ILogger<LdapService>? _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAppPolicyCache _runtimeCache;

        [SupportedOSPlatform("windows")]
        public LdapService(
            IOptions<LdapConfiguration> options,
            ILogger<LdapService> logger,
            IHttpContextAccessor httpContextAccessor,
            AppCaches appCaches)
        {
            _ldapConfiguration = options.Value;
            _runtimeCache = appCaches.RuntimeCache;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        [SupportedOSPlatform("windows")]
        private AdGroups? GetGroups(UserPrincipal? user)
        {
            if (user is null) return default;

            try
            {
                var groups = user.GetAuthorizationGroups();
                return new AdGroups { PrincipalGroups = groups };
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error retrieving groups for {displayName}", user?.DisplayName);
                return null;
            }
        }

        [SupportedOSPlatform("windows")]
        public LdapUser? GetUser(string? username = null)
        {
            username ??= _ldapConfiguration.TestUser;
            username ??= _httpContextAccessor.HttpContext!.User?.Identity?.Name;
            username ??= WindowsIdentity.GetCurrent().Name;
            if (username is null) return null;

            username = username.RemoveDomain();
            LdapUser? cachedResult = _runtimeCache.GetCacheItem(username!, () =>
            {
                try
                {
                    var context = new PrincipalContext(
                        ContextType.Domain,
                        _ldapConfiguration.Credentials.Instance,
                        _ldapConfiguration.Credentials.Username,
                        _ldapConfiguration.Credentials.Password
                    );

                    UserPrincipal? user = UserPrincipal.FindByIdentity(
                        context,
                        IdentityType.SamAccountName,
                        username
                    );

                    if (user is null)
                    {
                        var queryUser = new UserPrincipal(context) { SamAccountName = username };
                        using var searcher = new PrincipalSearcher(queryUser);
                        user = searcher.FindAll().FirstOrDefault() as UserPrincipal;
                    }

                    if (user is null) return null;

                    var groups = GetGroups(user)?.PrincipalGroups?.Select(x => x.Name).ToList() ?? [];
                    var isAllowed = groups.Intersect(_ldapConfiguration.AllowedGroupsFrontEnd).Any();
                    var isAllowedBackoffice = groups.Intersect(_ldapConfiguration.AllowedGroupsBackOffice).Any();

                    return new LdapUser(isAllowed)
                    {
                        DisplayName = user.DisplayName,
                        Email = user.EmailAddress,
                        FirstName = user.GivenName,
                        Groups = groups.ToArray(),
                        LastName = user.Surname,
                        Username = user.SamAccountName,
                        IsAllowedBackOffice = isAllowedBackoffice,
                    };
                }
                catch (Exception ex)
                {
                    _logger?.LogError(exception: ex, message: "Couldn't Get User {username}", args: username);
                    return default;
                }
            }, TimeSpan.FromMinutes(30));

            return cachedResult;
        }

        [SupportedOSPlatform("windows")]
        public SearchResultCollection? GetAll()
        {
            var connection = $"LDAP://{_ldapConfiguration.Credentials.Instance}/CN=Users";
            foreach (var dc in _ldapConfiguration.Credentials.DCs)
                connection += $",DC={dc}";

            try
            {
                DirectoryEntry ldap = new(connection, _ldapConfiguration.Credentials.Username, _ldapConfiguration.Credentials.Password);
                DirectorySearcher dSearch = new(ldap);
                return dSearch.FindAll();
            }
            catch (Exception ex)
            {
                _logger?.LogError(exception: ex, message: "Couldn't Get All users");
                return default;
            }
        }
    }
}
