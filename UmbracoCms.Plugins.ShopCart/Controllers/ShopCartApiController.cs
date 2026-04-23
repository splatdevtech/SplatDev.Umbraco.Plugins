using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Web.Common.Controllers;
using UmbracoCms.Plugins.ShopCart.Models;
using UmbracoCms.Plugins.ShopCart.Services;

namespace UmbracoCms.Plugins.ShopCart.Controllers;

[Route("umbraco/api/shopcart/[action]")]
public class ShopCartApiController : UmbracoApiController
{
    private readonly IShopCartService _service;

    public ShopCartApiController(IShopCartService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetCart(string sessionId)
    {
        if (string.IsNullOrWhiteSpace(sessionId))
            return BadRequest("sessionId is required.");

        var items = await _service.GetCart(sessionId);
        return Ok(items);
    }

    [HttpPost]
    public async Task<IActionResult> AddItem([FromBody] CartItem item)
    {
        if (string.IsNullOrWhiteSpace(item.SessionId))
            return BadRequest("SessionId is required.");

        if (string.IsNullOrWhiteSpace(item.ProductId))
            return BadRequest("ProductId is required.");

        await _service.AddItem(item);
        return Ok();
    }

    [HttpPost]
    public async Task<IActionResult> UpdateQuantity([FromBody] UpdateQuantityRequest request)
    {
        if (request.Qty < 0)
            return BadRequest("Qty must be zero or greater.");

        await _service.UpdateQuantity(request.Id, request.Qty);
        return Ok();
    }

    [HttpDelete]
    public async Task<IActionResult> RemoveItem(int id)
    {
        await _service.RemoveItem(id);
        return Ok();
    }

    [HttpDelete]
    public async Task<IActionResult> ClearCart(string sessionId)
    {
        if (string.IsNullOrWhiteSpace(sessionId))
            return BadRequest("sessionId is required.");

        await _service.ClearCart(sessionId);
        return Ok();
    }

    [HttpGet]
    public async Task<IActionResult> GetTotal(string sessionId)
    {
        if (string.IsNullOrWhiteSpace(sessionId))
            return BadRequest("sessionId is required.");

        var total = await _service.GetTotal(sessionId);
        return Ok(new { sessionId, total });
    }
}

public record UpdateQuantityRequest(int Id, int Qty);
