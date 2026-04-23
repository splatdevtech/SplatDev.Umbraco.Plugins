using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Web.Common.Controllers;

namespace SplatDev.Umbraco.Plugins.CharLimit.Controllers;

[Route("umbraco/api/charlimit/[action]")]
public class CharLimitApiController : UmbracoApiController
{
    [HttpGet]
    public IActionResult GetConfig()
    {
        var config = new CharLimitConfiguration();
        return Ok(config);
    }
}
