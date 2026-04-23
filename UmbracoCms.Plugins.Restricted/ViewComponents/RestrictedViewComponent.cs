using Microsoft.AspNetCore.Mvc;
using UmbracoCms.Plugins.Restricted.Services;

namespace UmbracoCms.Plugins.Restricted.ViewComponents;

public class RestrictedViewComponent : ViewComponent
{
    private readonly IRestrictedContentService _service;

    public RestrictedViewComponent(IRestrictedContentService service)
    {
        _service = service;
    }

    public async Task<IViewComponentResult> InvokeAsync(int nodeId)
    {
        var groups = await _service.GetRequiredGroupsAsync(nodeId);
        ViewBag.NodeId = nodeId;
        ViewBag.IsRestricted = groups.Any();
        return View(groups);
    }
}
