using Microsoft.AspNetCore.Mvc;
using UmbracoCms.Plugins.PasswordSettings.Services;

namespace UmbracoCms.Plugins.PasswordSettings.ViewComponents;

public class PasswordSettingsViewComponent : ViewComponent
{
    private readonly IPasswordSettingsService _service;

    public PasswordSettingsViewComponent(IPasswordSettingsService service)
    {
        _service = service;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var policy = await _service.GetPolicyAsync();
        return View(policy);
    }
}
