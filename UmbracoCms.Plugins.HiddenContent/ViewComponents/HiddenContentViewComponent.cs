using Microsoft.AspNetCore.Mvc;
using UmbracoCms.Plugins.HiddenContent.Services;

namespace UmbracoCms.Plugins.HiddenContent.ViewComponents;

public class HiddenContentViewComponent : ViewComponent
{
    private readonly IHiddenContentService _service;

    public HiddenContentViewComponent(IHiddenContentService service)
    {
        _service = service;
    }

    public async Task<IViewComponentResult> InvokeAsync(int nodeId)
    {
        var isHidden = await _service.IsHiddenAsync(nodeId);
        ViewBag.NodeId = nodeId;
        ViewBag.IsHidden = isHidden;
        return View(isHidden);
    }
}
