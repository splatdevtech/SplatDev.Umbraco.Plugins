using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Web.Common.Controllers;
using UmbracoCms.Plugins.CustomLogin.Models;
using UmbracoCms.Plugins.CustomLogin.Services;

namespace UmbracoCms.Plugins.CustomLogin.Controllers;

[Route("umbraco/api/customlogin/[action]")]
public class CustomLoginApiController : UmbracoApiController
{
    private readonly ICustomLoginService _service;

    public CustomLoginApiController(ICustomLoginService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetSettings()
    {
        var settings = await _service.GetSettingsAsync();
        return Ok(settings);
    }

    [HttpPost]
    public async Task<IActionResult> SaveSettings([FromBody] CustomLoginSettings settings)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        await _service.SaveSettingsAsync(settings);
        return Ok(new { message = "Settings saved." });
    }

    [HttpPost]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
            return BadRequest("Username and password are required.");

        var success = await _service.LoginAsync(request.Username, request.Password);
        if (!success)
            return Unauthorized(new { message = "Invalid credentials." });

        return Ok(new { message = "Login successful." });
    }

    [HttpGet]
    public async Task<IActionResult> ValidateMember([FromQuery] string username)
    {
        if (string.IsNullOrWhiteSpace(username))
            return BadRequest("Username is required.");

        var valid = await _service.ValidateMemberAsync(username);
        return Ok(new { valid });
    }
}

public record LoginRequest(string Username, string Password);
