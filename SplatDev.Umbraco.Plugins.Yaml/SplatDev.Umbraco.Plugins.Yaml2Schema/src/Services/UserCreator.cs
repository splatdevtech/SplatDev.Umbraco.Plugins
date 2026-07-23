using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Models.Membership;
using Umbraco.Cms.Core.Services;
using SplatDev.Umbraco.Plugins.Yaml2Schema.Models;

// IUserGroupService with UpdateUserGroupsOnUsersAsync was introduced in Umbraco 14.
// Umbraco 13 uses IUserService for group assignments.

namespace SplatDev.Umbraco.Plugins.Yaml2Schema.Services
{
    public class UserCreator
    {
        private readonly IUserService _userService;
#if !NET8_0
        // IUserGroupService with async group assignment methods is a Umbraco 14+ API
        private readonly IUserGroupService _userGroupService;
#endif
        private readonly ILogger<UserCreator>? _logger;

        public UserCreator(
            IUserService userService,
#if !NET8_0
            IUserGroupService userGroupService,
#endif
            ILogger<UserCreator>? logger = null)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
#if !NET8_0
            _userGroupService = userGroupService ?? throw new ArgumentNullException(nameof(userGroupService));
#endif
            _logger = logger;
        }

        public void CreateUsers(List<YamlUser> users)
        {
            if (users == null) throw new ArgumentNullException(nameof(users));

            var processedEmails = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var yamlUser in users)
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(yamlUser.Email))
                    {
                        _logger?.LogWarning("User entry is missing an email. Skipping.");
                        continue;
                    }

                    if (processedEmails.Contains(yamlUser.Email))
                    {
                        _logger?.LogWarning("User '{Email}' is a duplicate and will be skipped.", yamlUser.Email);
                        continue;
                    }

                    // [REMOVE]
                    if (yamlUser.Remove)
                    {
                        var toDelete = _userService.GetByEmail(yamlUser.Email);
                        if (toDelete != null)
                        {
                            _userService.Delete(toDelete);
                            _logger?.LogInformation("User '{Email}' removed.", yamlUser.Email);
                        }
                        else
                        {
                            _logger?.LogDebug("User '{Email}' not found for removal. Skipping.", yamlUser.Email);
                        }
                        processedEmails.Add(yamlUser.Email);
                        continue;
                    }

                    var existing = _userService.GetByEmail(yamlUser.Email);

                    // [UPDATE]
                    if (yamlUser.Update && existing != null)
                    {
                        existing.Name = yamlUser.Name;
                        existing.Username = yamlUser.Username ?? existing.Username;
                        AssignGroups(existing, yamlUser.UserGroups);
                        _userService.Save(existing);
                        _logger?.LogInformation("User '{Email}' updated.", yamlUser.Email);
                        processedEmails.Add(yamlUser.Email);
                        continue;
                    }

                    if (existing != null)
                    {
                        _logger?.LogInformation("User '{Email}' already exists. Skipping.", yamlUser.Email);
                        processedEmails.Add(yamlUser.Email);
                        continue;
                    }

                    // Create
                    var user = _userService.CreateUserWithIdentity(
                        yamlUser.Username ?? yamlUser.Email,
                        yamlUser.Email);

                    if (user == null)
                    {
                        _logger?.LogWarning("Failed to create user '{Email}'.", yamlUser.Email);
                        processedEmails.Add(yamlUser.Email);
                        continue;
                    }

                    user.Name = yamlUser.Name;
                    AssignGroups(user, yamlUser.UserGroups);
                    _userService.Save(user);

                    _logger?.LogInformation("User '{Email}' created.", yamlUser.Email);
                    processedEmails.Add(yamlUser.Email);
                }
                catch (Exception ex)
                {
                    _logger?.LogError(ex, "Error processing user '{Email}'.", yamlUser.Email);
                    throw;
                }
            }
        }

        private void AssignGroups(IUser user, List<string> groupAliases)
        {
            if (groupAliases == null || !groupAliases.Any())
                return;

#if NET8_0
            // Umbraco 13: IUserGroupService lacks async group-assignment helpers.
            // Group membership for newly created users must be managed via the
            // Umbraco back-office or a custom migration after the fact.
            _logger?.LogWarning(
                "UserCreator: automatic group assignment for user '{Email}' is not supported on Umbraco 13 (net8.0). " +
                "Assign the user to groups [{Groups}] manually in the back-office.",
                user.Email, string.Join(", ", groupAliases));
#else
            if (user.Key == Guid.Empty)
                return;

            var groups = _userGroupService.GetAsync(groupAliases.ToArray()).GetAwaiter().GetResult();
            var groupKeys = new HashSet<Guid>(groups.Select(g => g.Key));
            var userKeys = new HashSet<Guid> { user.Key };
            _userGroupService.UpdateUserGroupsOnUsersAsync(groupKeys, userKeys).GetAwaiter().GetResult();
#endif
        }
    }
}
