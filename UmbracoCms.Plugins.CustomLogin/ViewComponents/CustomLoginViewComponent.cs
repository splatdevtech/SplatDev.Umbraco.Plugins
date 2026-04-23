using Microsoft.AspNetCore.Mvc;
using UmbracoCms.Plugins.CustomLogin.Services;

namespace UmbracoCms.Plugins.CustomLogin.ViewComponents;

public class CustomLoginViewComponent : ViewComponent
{
    private readonly ICustomLoginService _service;

    public CustomLoginViewComponent(ICustomLoginService service)
    {
        _service = service;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var settings = await _service.GetSettingsAsync();
        return View(settings);
    }
}
