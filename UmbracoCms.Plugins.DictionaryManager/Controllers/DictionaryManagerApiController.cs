using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Web.Common.Controllers;
using UmbracoCms.Plugins.DictionaryManager.Models;
using UmbracoCms.Plugins.DictionaryManager.Services;

namespace UmbracoCms.Plugins.DictionaryManager.Controllers;

[Route("umbraco/api/dictionarymanager/[action]")]
public class DictionaryManagerApiController : UmbracoApiController
{
    private readonly IDictionaryManagerService _service;

    public DictionaryManagerApiController(IDictionaryManagerService service)
    {
        _service = service;
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        var items = _service.GetAll();
        return Ok(items);
    }

    [HttpPost]
    public IActionResult Create([FromBody] DictionaryItemDto item)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (string.IsNullOrWhiteSpace(item.Key))
            return BadRequest("Key is required.");

        var result = _service.Create(item);
        if (result is null)
            return StatusCode(500, new { message = "Failed to create dictionary item." });

        return Ok(result);
    }

    [HttpPut]
    public IActionResult Update([FromBody] DictionaryItemDto item)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (string.IsNullOrWhiteSpace(item.Key))
            return BadRequest("Key is required.");

        var result = _service.Update(item);
        if (result is null)
            return NotFound($"Dictionary item '{item.Key}' not found.");

        return Ok(result);
    }

    [HttpDelete]
    public IActionResult Delete([FromQuery] string key)
    {
        if (string.IsNullOrWhiteSpace(key))
            return BadRequest("Key is required.");

        var deleted = _service.Delete(key);
        if (!deleted)
            return NotFound($"Dictionary item '{key}' not found.");

        return Ok(new { message = $"'{key}' deleted." });
    }

    [HttpPost]
    public async Task<IActionResult> Import(
        [FromBody] List<DictionaryItemDto> items,
        [FromQuery] bool overrideExisting = false)
    {
        if (items is null || items.Count == 0)
            return BadRequest("No items provided.");

        var results = await _service.ImportAsync(items, overrideExisting);
        return Ok(results);
    }

    [HttpGet]
    public async Task<IActionResult> Export()
    {
        var bytes = await _service.ExportAsync();
        return File(bytes, "application/json", "dictionary-export.json");
    }
}
