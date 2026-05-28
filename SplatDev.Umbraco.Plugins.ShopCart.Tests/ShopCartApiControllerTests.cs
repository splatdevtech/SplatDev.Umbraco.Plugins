using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using SplatDev.Umbraco.Plugins.ShopCart.Controllers;
using SplatDev.Umbraco.Plugins.ShopCart.Services;

namespace SplatDev.Umbraco.Plugins.ShopCart.Tests;

public class ShopCartApiControllerTests
{
    private readonly Mock<IShopCartService> _svc = new();
    private readonly ShopCartApiController _sut;

    public ShopCartApiControllerTests() => _sut = new(_svc.Object);

    [Fact]
    public async Task GetCart_NoSessionId_ReturnsBadRequest() =>
        Assert.IsType<BadRequestObjectResult>(await _sut.GetCart(""));

    [Fact]
    public async Task GetCart_Valid_ReturnsOk()
    {
        _svc.Setup(s => s.GetCart("abc")).ReturnsAsync([]);
        Assert.IsType<OkObjectResult>(await _sut.GetCart("abc"));
    }

    [Fact]
    public async Task AddItem_NoSessionId_ReturnsBadRequest() =>
        Assert.IsType<BadRequestObjectResult>(await _sut.AddItem(new() { SessionId = "", ProductId = "p1" }));

    [Fact]
    public async Task AddItem_NoProductId_ReturnsBadRequest() =>
        Assert.IsType<BadRequestObjectResult>(await _sut.AddItem(new() { SessionId = "s", ProductId = "" }));

    [Fact]
    public async Task AddItem_Valid_ReturnsOk() =>
        Assert.IsType<OkResult>(await _sut.AddItem(new() { SessionId = "s", ProductId = "p1" }));

    [Fact]
    public async Task UpdateQuantity_Negative_ReturnsBadRequest() =>
        Assert.IsType<BadRequestObjectResult>(await _sut.UpdateQuantity(new(1, -1)));

    [Fact]
    public async Task UpdateQuantity_Valid_ReturnsOk() =>
        Assert.IsType<OkResult>(await _sut.UpdateQuantity(new(1, 2)));
}
