using Umbraco.Cms.Core.Services;

namespace SplatDev.Umbraco.Tools.PackageActions;

public abstract class PermissionsAction : IPackageAction
{
    private readonly IContentService _contentService;
#if NET10_0_OR_GREATER
    private readonly IUserGroupService _userGroupService;

    protected PermissionsAction(IContentService contentService, IUserGroupService userGroupService)
    {
        _contentService = contentService;
        _userGroupService = userGroupService;
    }
#else
    private readonly IUserService _userService;

    protected PermissionsAction(IContentService contentService, IUserService userService)
    {
        _contentService = contentService;
        _userService = userService;
    }
#endif

    public abstract string Name { get; }
    protected abstract string GroupName { get; }
    protected abstract IEnumerable<string> PermissionChars { get; }
    protected abstract int ContentId { get; }

#if NET10_0_OR_GREATER
    public virtual async Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        var content = _contentService.GetById(ContentId);
        if (content is null)
            return;

        var allGroups = await _userGroupService.GetAllAsync(0, int.MaxValue);
        var groups = allGroups.Items
            .Where(g => g.Name == GroupName);

        foreach (var group in groups)
        {
            foreach (var permission in PermissionChars)
            {
                _contentService.SetPermission(content, permission, new[] { group.Id });
            }
        }
    }
#else
    public virtual Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        var content = _contentService.GetById(ContentId);
        if (content is null)
            return Task.CompletedTask;

        var groups = _userService.GetAllUserGroups()
            .Where(g => g.Name == GroupName);

        foreach (var group in groups)
        {
            foreach (var permission in PermissionChars)
            {
                _contentService.SetPermission(content, permission[0], new[] { group.Id });
            }
        }

        return Task.CompletedTask;
    }
#endif
}
