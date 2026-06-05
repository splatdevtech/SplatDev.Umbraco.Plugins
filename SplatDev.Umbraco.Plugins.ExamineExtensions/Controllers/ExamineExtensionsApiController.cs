using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Web.Common.Controllers;
using SplatDev.Umbraco.Plugins.ExamineExtensions.Models;
using SplatDev.Umbraco.Plugins.ExamineExtensions.Services;

namespace SplatDev.Umbraco.Plugins.ExamineExtensions.Controllers;

[Route("umbraco/api/examineextensions/[action]")]
public class ExamineExtensionsApiController : ControllerBase
{
    private readonly IExamineExtensionsService _service;

    public ExamineExtensionsApiController(IExamineExtensionsService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetIndexes()
    {
        var indexes = await _service.GetAllIndexesAsync();
        return Ok(indexes);
    }

    [HttpPost]
    public async Task<IActionResult> Search([FromBody] SearchRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Query))
            return BadRequest("Query is required.");

        var result = await _service.SearchAsync(request);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> RebuildIndex([FromBody] string indexName)
    {
        if (string.IsNullOrWhiteSpace(indexName))
            return BadRequest("Index name is required.");

        await _service.RebuildIndexAsync(indexName);
        return Ok(new { success = true, message = $"Index '{indexName}' rebuild triggered." });
    }
}
