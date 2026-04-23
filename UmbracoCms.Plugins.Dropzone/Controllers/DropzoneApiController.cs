using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Web.Common.Controllers;
using UmbracoCms.Plugins.Dropzone.Models;
using UmbracoCms.Plugins.Dropzone.Services;

namespace UmbracoCms.Plugins.Dropzone.Controllers;

[Route("umbraco/api/dropzone/[action]")]
public class DropzoneApiController : UmbracoApiController
{
    private readonly IDropzoneService _service;

    public DropzoneApiController(IDropzoneService service)
    {
        _service = service;
    }

    [HttpPost]
    [DisableRequestSizeLimit]
    public async Task<IActionResult> Upload([FromForm] IFormFile file, [FromForm] string? folderName, [FromForm] int? parentMediaId)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file provided.");

        var request = new UploadRequest
        {
            FolderName = folderName ?? "",
            ParentMediaId = parentMediaId
        };

        var result = await _service.UploadFileAsync(file, request);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetMedia([FromQuery] int? parentId)
    {
        var items = await _service.GetMediaItemsAsync(parentId);
        var data = items.Select(m => new
        {
            id = m.Id,
            key = m.Key,
            name = m.Name,
            contentType = m.ContentType.Alias
        });
        return Ok(data);
    }

    [HttpDelete]
    public async Task<IActionResult> Delete([FromQuery] Guid mediaKey)
    {
        var success = await _service.DeleteMediaAsync(mediaKey);
        if (!success)
            return NotFound($"Media item with key '{mediaKey}' not found.");

        return Ok(new { success = true });
    }
}
