using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Web.Common.Controllers;
using SplatDev.Umbraco.Plugins.WhatsApp.Services;

namespace SplatDev.Umbraco.Plugins.WhatsApp.Controllers;

[Route("umbraco/api/whatsapp/[action]")]
public class WhatsAppApiController : ControllerBase
{
    private readonly IWhatsAppService _service;

    public WhatsAppApiController(IWhatsAppService service)
    {
        _service = service;
    }

    [HttpGet]
    public IActionResult GetSettings()
    {
        var settings = _service.GetSettings();
        return Ok(settings);
    }

    [HttpGet]
    public IActionResult GetWhatsAppUrl([FromQuery] string? phoneNumber = null, [FromQuery] string? message = null)
    {
        var settings = _service.GetSettings();
        var phone = phoneNumber ?? settings.PhoneNumber;
        var msg = message ?? settings.WelcomeMessage;

        if (string.IsNullOrWhiteSpace(phone))
            return BadRequest("Phone number is required.");

        var url = _service.GenerateWhatsAppUrl(phone, msg);
        return Ok(new { url });
    }
}
