using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Web.Common.Controllers;
using SplatDev.Umbraco.Plugins.PasswordSettings.Models;
using SplatDev.Umbraco.Plugins.PasswordSettings.Services;

namespace SplatDev.Umbraco.Plugins.PasswordSettings.Controllers;

[Route("umbraco/api/passwordsettings/[action]")]
public class PasswordSettingsApiController : ControllerBase
{
    private readonly IPasswordSettingsService _service;

    public PasswordSettingsApiController(IPasswordSettingsService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetPolicy()
    {
        var policy = await _service.GetPolicyAsync();
        if (policy is null)
            return Ok(new PasswordPolicy());

        return Ok(policy);
    }

    [HttpPost]
    public async Task<IActionResult> SavePolicy([FromBody] PasswordPolicy policy)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var saved = await _service.SavePolicyAsync(policy);
        return Ok(saved);
    }

    [HttpPost]
    public async Task<IActionResult> ValidatePassword([FromBody] ValidatePasswordRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Password))
            return BadRequest("Password is required.");

        var policy = await _service.GetPolicyAsync() ?? new PasswordPolicy();
        var (valid, errors) = await _service.ValidatePasswordAsync(request.Password, policy);
        return Ok(new { valid, errors });
    }

    [HttpPost]
    public async Task<IActionResult> RecordPasswordChange([FromBody] RecordPasswordChangeRequest request)
    {
        if (request.MemberId <= 0 || string.IsNullOrWhiteSpace(request.PasswordHash))
            return BadRequest("MemberId and PasswordHash are required.");

        await _service.RecordPasswordChangeAsync(request.MemberId, request.PasswordHash);
        return Ok(new { message = "Password change recorded." });
    }

    [HttpGet]
    public async Task<IActionResult> IsPasswordReused(
        [FromQuery] int memberId,
        [FromQuery] string passwordHash)
    {
        if (memberId <= 0 || string.IsNullOrWhiteSpace(passwordHash))
            return BadRequest("MemberId and passwordHash are required.");

        var policy = await _service.GetPolicyAsync() ?? new PasswordPolicy();
        var reused = await _service.IsPasswordReusedAsync(memberId, passwordHash, policy.HistoryCount);
        return Ok(new { reused });
    }
}

public class ValidatePasswordRequest
{
    public string Password { get; set; } = string.Empty;
}

public class RecordPasswordChangeRequest
{
    public int MemberId { get; set; }
    public string PasswordHash { get; set; } = string.Empty;
}
