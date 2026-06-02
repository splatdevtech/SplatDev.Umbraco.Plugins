using Microsoft.AspNetCore.Mvc;
using SplatDev.Umbraco.Plugins.CustomLogin.Services;

namespace SplatDev.Umbraco.Plugins.CustomLogin.ViewComponents;

public class CustomLoginViewComponent(ICustomLoginService service) : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync()
    {
        var settings = await service.GetSettingsAsync();
        return View(settings);
    }
}
