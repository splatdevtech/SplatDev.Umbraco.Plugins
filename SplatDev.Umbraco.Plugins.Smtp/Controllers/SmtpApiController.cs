using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Web.Common.Controllers;
using SplatDev.Umbraco.Plugins.Smtp.Models;
using SplatDev.Umbraco.Plugins.Smtp.Services;

namespace SplatDev.Umbraco.Plugins.Smtp.Controllers;

[Route("umbraco/api/smtp/[action]")]
public class SmtpApiController : ControllerBase
{
    private readonly ISmtpService _service;

    public SmtpApiController(ISmtpService service)
    {
        _service = service;
    }

    [HttpGet]
    public IActionResult GetSettings()
    {
        var settings = _service.GetSettings();
        // Mask password before returning
        var masked = new SmtpSettings
        {
            Host = settings.Host,
            Port = settings.Port,
            Username = settings.Username,
            Password = string.IsNullOrEmpty(settings.Password) ? string.Empty : "********",
            EnableSsl = settings.EnableSsl,
            FromEmail = settings.FromEmail,
            FromName = settings.FromName
        };
        return Ok(masked);
    }

    [HttpPost]
    public async Task<IActionResult> TestConnection([FromBody] SmtpSettings settings)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _service.TestConnectionAsync(settings);
        return Ok(result);
    }
}
