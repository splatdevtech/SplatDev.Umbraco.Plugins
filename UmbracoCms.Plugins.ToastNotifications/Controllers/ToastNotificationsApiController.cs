using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Web.Common.Controllers;
using UmbracoCms.Plugins.ToastNotifications.Models;
using UmbracoCms.Plugins.ToastNotifications.Services;

namespace UmbracoCms.Plugins.ToastNotifications.Controllers;

[Route("umbraco/api/toastnotifications/[action]")]
public class ToastNotificationsApiController : UmbracoApiController
{
    private readonly IToastNotificationsService _service;

    public ToastNotificationsApiController(IToastNotificationsService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetActive()
    {
        var toasts = await _service.GetActiveToastsAsync();
        return Ok(toasts);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ToastMessage toast)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var created = await _service.CreateToastAsync(toast);
        return Ok(created);
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromQuery] int id, [FromBody] ToastMessage toast)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var updated = await _service.UpdateToastAsync(id, toast);
        if (updated is null)
            return NotFound();

        return Ok(updated);
    }

    [HttpDelete]
    public async Task<IActionResult> Delete([FromQuery] int id)
    {
        var deleted = await _service.DeleteToastAsync(id);
        if (!deleted)
            return NotFound();

        return Ok(new { message = "Toast deleted." });
    }
}
