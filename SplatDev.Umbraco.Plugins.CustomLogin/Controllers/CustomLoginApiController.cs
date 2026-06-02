using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Web.Common.Controllers;
using SplatDev.Umbraco.Plugins.CustomLogin.Models;
using SplatDev.Umbraco.Plugins.CustomLogin.Services;

namespace SplatDev.Umbraco.Plugins.CustomLogin.Controllers;

[Route("umbraco/api/customlogin/[action]")]
#pragma warning disable CS0618 // UmbracoApiController is obsolete in Umbraco 17 but required for v13 compatibility
public class CustomLoginApiController(ICustomLoginService service) : UmbracoApiController
#pragma warning restore CS0618
{
    [HttpGet]
    public async Task<IActionResult> GetSettings()
    {
        var settings = await service.GetSettingsAsync();
        return Ok(settings);
    }

    [HttpPost]
    public async Task<IActionResult> SaveSettings([FromBody] CustomLoginSettings settings)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        await service.SaveSettingsAsync(settings);
        return Ok(new { message = "Settings saved." });
    }

    [HttpGet]
    public IActionResult PreviewCss()
    {
        var settings = service.GetSettings();
        var css = LoginPageCssGenerator.Generate(settings);
        return Content(css, "text/css");
    }

    [HttpGet]
    public IActionResult PreviewLocalization([FromQuery] string culture = "en")
    {
        var settings = service.GetSettings();
        var js = culture.StartsWith("es", StringComparison.OrdinalIgnoreCase)
            ? LoginPageLocalizationGenerator.GenerateSpanish(settings)
            : LoginPageLocalizationGenerator.GenerateEnglish(settings);
        return Content(js, "application/javascript");
    }

    [HttpPost]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
            return BadRequest("Username and password are required.");

        var success = await service.LoginAsync(request.Username, request.Password);
        if (!success)
            return Unauthorized(new { message = "Invalid credentials." });

        return Ok(new { message = "Login successful." });
    }

    [HttpGet]
    public async Task<IActionResult> ValidateMember([FromQuery] string username)
    {
        if (string.IsNullOrWhiteSpace(username))
            return BadRequest("Username is required.");

        var valid = await service.ValidateMemberAsync(username);
        return Ok(new { valid });
    }
}

public record LoginRequest(string Username, string Password);
