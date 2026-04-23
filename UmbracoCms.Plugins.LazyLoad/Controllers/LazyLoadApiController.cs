using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Web.Common.Controllers;
using UmbracoCms.Plugins.LazyLoad.Models;
using UmbracoCms.Plugins.LazyLoad.Services;

namespace UmbracoCms.Plugins.LazyLoad.Controllers;

[Route("umbraco/api/lazyload/[action]")]
public class LazyLoadApiController : UmbracoApiController
{
    private readonly ILazyLoadService _service;

    public LazyLoadApiController(ILazyLoadService service)
    {
        _service = service;
    }

    [HttpGet]
    public IActionResult GetSettings()
        => Ok(_service.GetSettings());

    [HttpPost]
    public IActionResult SaveSettings([FromBody] LazyLoadSettings settings)
    {
        _service.SaveSettings(settings);
        return Ok(_service.GetSettings());
    }
}
