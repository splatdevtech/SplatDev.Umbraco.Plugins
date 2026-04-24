using Microsoft.Extensions.Logging;

using SplatDev.Umbraco.Authorization.Ldap.Models;

using Umbraco.Cms.Core.Models.Membership;
using Umbraco.Cms.Core.Services;

namespace SplatDev.Umbraco.Authorization.Ldap.Extensions
{
    public static class LdapUserExtensions
    {
        public static string RemoveDomain(this string username)
        {
            if (string.IsNullOrEmpty(username))
                return username;

            if (username.Contains('\\'))
                return username[(username.IndexOf('\\') + 1)..];

            if (username.Contains('@'))
                return username[..username.IndexOf('@')];

            return username;
        }

        public static void PopulateUmbracoUser(this LdapUser user, IUserService userService)
        {
            var uUser = userService.GetByUsername(user.Username);
            user.UmbracoUser = uUser;
        }

        public static Tuple<IUser?, LdapUser, bool> CreateBackOfficeUser<T>(
            this LdapUser user,
            IUserService userService,
            LdapConfiguration configuration,
            ILogger<T> logger) where T : class
        {
            IUser? existingUser = userService.GetByUsername(user.Email);
            bool isNewUser = false;
            if (existingUser is null)
            {
                isNewUser = true;
                try
                {
                    existingUser ??= userService.CreateUserWithIdentity(user.Email!, user.Email!);
                    existingUser.Name = user.DisplayName;
                    existingUser.IsApproved = true;
                    existingUser.LastLoginDate = DateTime.Now;
                    userService.Save(existingUser);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Could not create new User");
                }

                existingUser ??= userService.GetByUsername(user.Email);
                if (existingUser is null) return new Tuple<IUser?, LdapUser, bool>(null, user, isNewUser);
            }

            if (existingUser is not null && user.Groups is not null && user.Groups.Length != 0
                && (isNewUser || configuration.SetGroupsOnLogin == true))
            {
                bool groupsChanged = false;
                foreach (var group in user.Groups)
                {
                    var groupMappings = configuration.GroupBindings;
                    var groupAlias = groupMappings.TryGetValue(group, out var binding) ? binding.Alias : null;
                    if (groupAlias is null) continue;

#if !NET10_0_OR_GREATER
                    var uGroup = userService.GetUserGroupByAlias(groupAlias);
                    if (uGroup is not null && !existingUser.Groups.Any(g => g.Id == uGroup.Id))
                    {
                        existingUser.AddGroup((IReadOnlyUserGroup)uGroup);
                        groupsChanged = true;
                    }
#endif
                }

                if (groupsChanged)
                    userService.Save(existingUser);
            }

            user.UmbracoUser = existingUser;
            return new Tuple<IUser?, LdapUser, bool>(existingUser!, user, isNewUser);
        }
    }
}
