using Umbraco.Cms.Core.Models.Membership;

namespace SplatDev.Umbraco.Plugins.MemberGroups.Helpers
{
    public static class GroupHelpers
    {
        public static IReadOnlyUserGroup ToReadOnlyGroup(this IUserGroup group)
        {
            if (group is IReadOnlyUserGroup readOnly)
                return readOnly;

#if NET10_0_OR_GREATER
            // Umbraco 17: ReadOnlyUserGroup(int id, Guid key, string? name, string? icon,
            //   int? startContentId, int? startMediaId, string? alias,
            //   IEnumerable<int> allowedLanguages, IEnumerable<string> allowedSections,
            //   ISet<string> permissions, ISet<IGranularPermission> granularPermissions,
            //   bool hasAccessToAllLanguages)
            return new ReadOnlyUserGroup(
                group.Id,
                group.Key,
                group.Name ?? string.Empty,
                group.Icon ?? string.Empty,
                group.StartContentId,
                group.StartMediaId,
                group.Alias ?? string.Empty,
                group.AllowedLanguages,
                group.AllowedSections,
                group.Permissions ?? new HashSet<string>(),
                group.GranularPermissions,
                group.HasAccessToAllLanguages);
#else
            // Umbraco 13 (net8.0): 8-parameter legacy constructor (still compiles in v13)
            return new ReadOnlyUserGroup(
                group.Id,
                group.Name,
                group.Icon,
                group.StartContentId,
                group.StartMediaId,
                group.Alias,
                group.AllowedSections,
                group.Permissions);
#endif
        }
    }
}
