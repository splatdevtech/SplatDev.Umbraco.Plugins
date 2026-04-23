using Umbraco.Cms.Core.Models.Membership;

namespace UmbracoCms.Plugins.MemberGroups.Helpers
{
    public static class GroupHelpers
    {
        public static IReadOnlyUserGroup ToReadOnlyGroup(this IUserGroup group)
        {
            return group as IReadOnlyUserGroup
                ?? new ReadOnlyUserGroup(
                    group.Id,
                    group.Name,
                    group.Icon,
                    group.StartContentId,
                    group.StartMediaId,
                    group.Alias,
                    group.AllowedSections,
                    group.Permissions);
        }
    }
}
