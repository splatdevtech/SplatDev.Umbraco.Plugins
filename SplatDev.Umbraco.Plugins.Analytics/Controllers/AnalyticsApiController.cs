using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Web.Common.Controllers;
using SplatDev.Umbraco.Plugins.Analytics.Models;
using SplatDev.Umbraco.Plugins.Analytics.Services;

namespace SplatDev.Umbraco.Plugins.Analytics.Controllers;

[Route("umbraco/api/analytics/[action]")]
public class AnalyticsApiController : ControllerBase
{
    private readonly IAnalyticsService _service;

    public AnalyticsApiController(IAnalyticsService service)
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
    public async Task<IActionResult> SaveSettings([FromBody] AnalyticsSettings settings)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        await _service.SaveSettingsAsync(settings);
        return Ok(new { message = "Settings saved." });
    }

    [HttpGet]
    public async Task<IActionResult> GetPageViews([FromQuery] string? measurementId = null)
    {
        if (string.IsNullOrWhiteSpace(measurementId))
        {
            var current = await _service.GetSettingsAsync();
            measurementId = current.MeasurementId;
        }

        if (string.IsNullOrWhiteSpace(measurementId))
            return BadRequest("Measurement ID is required.");

        var views = await _service.GetPageViewsAsync(measurementId);
        return Ok(views);
    }
}
