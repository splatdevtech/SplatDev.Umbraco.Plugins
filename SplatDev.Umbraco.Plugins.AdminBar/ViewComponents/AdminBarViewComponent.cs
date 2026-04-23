using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Core.Security;
using Umbraco.Cms.Core.Web;

namespace SplatDev.Umbraco.Plugins.AdminBar.ViewComponents;

public class AdminBarViewComponent : ViewComponent
{
    private readonly IUmbracoContextAccessor _umbracoContextAccessor;
    private readonly IBackOfficeSecurityAccessor _backOfficeSecurityAccessor;

    public AdminBarViewComponent(
        IUmbracoContextAccessor umbracoContextAccessor,
        IBackOfficeSecurityAccessor backOfficeSecurityAccessor)
    {
        _umbracoContextAccessor = umbracoContextAccessor;
        _backOfficeSecurityAccessor = backOfficeSecurityAccessor;
    }

    public IViewComponentResult Invoke(int nodeId = 0)
    {
        var backOfficeSecurity = _backOfficeSecurityAccessor.BackOfficeSecurity;
        var isLoggedIn = backOfficeSecurity?.IsAuthenticated() == true;

        if (!isLoggedIn)
            return Content(string.Empty);

        var currentUser = backOfficeSecurity?.CurrentUser;
        var userName = currentUser?.Name ?? string.Empty;

        _umbracoContextAccessor.TryGetUmbracoContext(out var umbracoContext);
        var publishedContent = nodeId > 0
            ? umbracoContext?.Content?.GetById(nodeId)
            : null;

        var model = new AdminBarViewModel
        {
            NodeId = nodeId,
            NodeName = publishedContent?.Name ?? string.Empty,
            UserName = userName,
            IsLoggedIn = isLoggedIn
        };

        return View(model);
    }
}

public class AdminBarViewModel
{
    public int NodeId { get; set; }
    public string NodeName { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public bool IsLoggedIn { get; set; }
}
