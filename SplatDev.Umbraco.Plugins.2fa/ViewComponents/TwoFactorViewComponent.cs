using Microsoft.AspNetCore.Mvc;
using SplatDev.Umbraco.Plugins.TwoFactor.Services;

namespace SplatDev.Umbraco.Plugins.TwoFactor.ViewComponents;

public class TwoFactorViewComponent : ViewComponent
{
    private readonly ITwoFactorService _service;

    public TwoFactorViewComponent(ITwoFactorService service)
    {
        _service = service;
    }

    public async Task<IViewComponentResult> InvokeAsync(int memberId)
    {
        var isEnabled = await _service.IsEnabledAsync(memberId);
        ViewBag.MemberId = memberId;
        ViewBag.IsEnabled = isEnabled;
        return View();
    }
}
