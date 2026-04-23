using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Web.Common.Controllers;
using UmbracoCms.Plugins.PropertiesReport.Services;

namespace UmbracoCms.Plugins.PropertiesReport.Controllers;

[Route("umbraco/api/propertiesreport/[action]")]
public class PropertiesReportApiController : UmbracoApiController
{
    private readonly IPropertiesReportService _service;

    public PropertiesReportApiController(IPropertiesReportService service)
    {
        _service = service;
    }

    [HttpGet]
    public IActionResult GetReport()
    {
        var result = _service.GetReport();
        return Ok(result);
    }

    [HttpGet]
    public IActionResult GetByContentType([FromQuery] string alias)
    {
        if (string.IsNullOrWhiteSpace(alias))
            return BadRequest("Content type alias is required.");

        var result = _service.GetByContentType(alias);
        return Ok(result);
    }
}
