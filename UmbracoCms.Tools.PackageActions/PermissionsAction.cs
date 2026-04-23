using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.Membership;
using Umbraco.Cms.Core.Services;

namespace UmbracoCms.Tools.PackageActions;

public abstract class PermissionsAction : IPackageAction
{
    private readonly IContentService _contentService;
    private readonly IUserService _userService;

    protected PermissionsAction(IContentService contentService, IUserService userService)
    {
        _contentService = contentService;
        _userService = userService;
    }

    public abstract string Name { get; }
    protected abstract string GroupName { get; }
    protected abstract IEnumerable<string> PermissionChars { get; }
    protected abstract int ContentId { get; }

    public virtual Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        var content = _contentService.GetById(ContentId);
        if (content is null)
            return Task.CompletedTask;

        var groups = _userService.GetAllUserGroups()
            .Where(g => g.Name == GroupName);

        foreach (var group in groups)
        {
            var permissions = new ContentPermissions(
                new EntityPermission(group.Id, ContentId, PermissionChars.ToArray()));
            _contentService.SetPermissions(permissions);
        }

        return Task.CompletedTask;
    }
}
