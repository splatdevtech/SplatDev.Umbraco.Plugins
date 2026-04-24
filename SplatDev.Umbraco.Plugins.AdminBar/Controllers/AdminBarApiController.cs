using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Web.Common.Controllers;

namespace SplatDev.Umbraco.Plugins.AdminBar.Controllers;

[Route("umbraco/api/adminbar/[action]")]
public class AdminBarApiController : UmbracoApiController
{
    private readonly IContentService _contentService;
    private readonly IUmbracoContextAccessor _umbracoContextAccessor;

    public AdminBarApiController(
        IContentService contentService,
        IUmbracoContextAccessor umbracoContextAccessor)
    {
        _contentService = contentService;
        _umbracoContextAccessor = umbracoContextAccessor;
    }

    [HttpGet]
    public IActionResult GetCurrentPageInfo([FromQuery] int nodeId)
    {
        if (nodeId <= 0)
            return BadRequest("nodeId must be greater than 0.");

        _umbracoContextAccessor.TryGetUmbracoContext(out var ctx);
        var published = ctx?.Content?.GetById(nodeId);

        if (published is null)
        {
            var content = _contentService.GetById(nodeId);
            if (content is null)
                return NotFound();

            return Ok(new
            {
                id = content.Id,
                name = content.Name,
                published = content.Published,
                url = (string?)null
            });
        }

        return Ok(new
        {
            id = published.Id,
            name = published.Name,
            published = true,
            url = published.Url()
        });
    }

    [HttpPost]
    public IActionResult PublishPage([FromQuery] int nodeId)
    {
        if (nodeId <= 0)
            return BadRequest("nodeId must be greater than 0.");

        var content = _contentService.GetById(nodeId);
        if (content is null)
            return NotFound();

        var result = _contentService.SaveAndPublish(content);

        if (result.Success)
            return Ok(new { message = $"'{content.Name}' published successfully." });

        return StatusCode(500, new { message = $"Publish failed: {result.Result}" });
    }

    [HttpPost]
    public IActionResult UnpublishPage([FromQuery] int nodeId)
    {
        if (nodeId <= 0)
            return BadRequest("nodeId must be greater than 0.");

        var content = _contentService.GetById(nodeId);
        if (content is null)
            return NotFound();

        var result = _contentService.Unpublish(content);

        if (result.Success)
            return Ok(new { message = $"'{content.Name}' unpublished successfully." });

        return StatusCode(500, new { message = $"Unpublish failed: {result.Result}" });
    }
}
