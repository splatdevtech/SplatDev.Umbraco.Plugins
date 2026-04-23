using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Umbraco.Cms.Web.Common.Controllers;
using UmbracoCms.Plugins.NewsTicker.Models;
using UmbracoCms.Plugins.NewsTicker.Services;

namespace UmbracoCms.Plugins.NewsTicker.Controllers;

[Route("umbraco/api/newsticker")]
public class NewsTickerApiController : UmbracoApiController
{
    private readonly INewsTickerService _service;
    private readonly NewsTickerSettings _settings;

    public NewsTickerApiController(
        INewsTickerService service,
        IOptions<NewsTickerSettings> settings)
    {
        _service = service;
        _settings = settings.Value;
    }

    [HttpGet("items")]
    public async Task<IActionResult> GetItems()
    {
        var items = await _service.GetActiveItemsAsync();
        return Ok(items);
    }

    [HttpGet("settings")]
    public IActionResult GetSettings()
    {
        return Ok(_settings);
    }

    [HttpPost("items")]
    public async Task<IActionResult> AddItem([FromBody] NewsTickerItem item)
    {
        if (string.IsNullOrWhiteSpace(item.Text))
            return BadRequest("Text is required.");

        var created = await _service.AddItemAsync(item);
        return Ok(created);
    }

    [HttpPut("items/{id:int}")]
    public async Task<IActionResult> UpdateItem(int id, [FromBody] NewsTickerItem item)
    {
        if (id != item.Id)
            return BadRequest("ID mismatch.");

        try
        {
            await _service.UpdateItemAsync(item);
            return Ok();
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpDelete("items/{id:int}")]
    public async Task<IActionResult> DeleteItem(int id)
    {
        try
        {
            await _service.DeleteItemAsync(id);
            return Ok();
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(ex.Message);
        }
    }
}
