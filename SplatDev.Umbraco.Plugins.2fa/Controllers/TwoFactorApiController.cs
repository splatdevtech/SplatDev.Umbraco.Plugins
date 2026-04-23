using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Web.Common.Controllers;
using SplatDev.Umbraco.Plugins.TwoFactor.Services;

namespace SplatDev.Umbraco.Plugins.TwoFactor.Controllers;

[Route("umbraco/api/twofactor/[action]")]
public class TwoFactorApiController : UmbracoApiController
{
    private readonly ITwoFactorService _service;

    public TwoFactorApiController(ITwoFactorService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult> SetupTotp([FromQuery] int memberId)
    {
        if (memberId <= 0)
            return BadRequest("Valid memberId is required.");

        var setup = await _service.SetupTotpAsync(memberId);
        return Ok(new
        {
            setup.Id,
            setup.MemberId,
            setup.SecretKey,
            setup.IsEnabled,
            setup.CreatedAt
        });
    }

    [HttpPost]
    public async Task<IActionResult> VerifyTotp([FromQuery] int memberId, [FromQuery] string code)
    {
        if (memberId <= 0 || string.IsNullOrWhiteSpace(code))
            return BadRequest("memberId and code are required.");

        var valid = await _service.VerifyTotpAsync(memberId, code);
        return Ok(new { valid });
    }

    [HttpPost]
    public async Task<IActionResult> GenerateBackupCodes([FromQuery] int memberId, [FromQuery] int count = 8)
    {
        if (memberId <= 0)
            return BadRequest("Valid memberId is required.");

        try
        {
            var codes = await _service.GenerateBackupCodesAsync(memberId, count);
            return Ok(new { codes });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost]
    public async Task<IActionResult> UseBackupCode([FromQuery] int memberId, [FromQuery] string code)
    {
        if (memberId <= 0 || string.IsNullOrWhiteSpace(code))
            return BadRequest("memberId and code are required.");

        var used = await _service.UseBackupCodeAsync(memberId, code);
        return Ok(new { used });
    }

    [HttpGet]
    public async Task<IActionResult> IsEnabled([FromQuery] int memberId)
    {
        if (memberId <= 0)
            return BadRequest("Valid memberId is required.");

        var enabled = await _service.IsEnabledAsync(memberId);
        return Ok(new { enabled });
    }

    [HttpPost]
    public async Task<IActionResult> Disable([FromQuery] int memberId)
    {
        if (memberId <= 0)
            return BadRequest("Valid memberId is required.");

        await _service.DisableAsync(memberId);
        return Ok(new { message = "2FA disabled." });
    }
}
