using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using Umbraco.Cms.Core.Models;
using SplatDev.Umbraco.Plugins.MemberTypes.Controllers;
using SplatDev.Umbraco.Plugins.MemberTypes.Services;

namespace SplatDev.Umbraco.Plugins.MemberTypes.Tests;

public class MemberTypesApiControllerTests
{
    private readonly Mock<IMemberTypesService> _service;
    private readonly MemberTypesApiController _sut;

    public MemberTypesApiControllerTests()
    {
        _service = new Mock<IMemberTypesService>();
        _sut = new MemberTypesApiController(_service.Object);
    }

    private static IMemberType MakeType(int id, string alias, string name) =>
        Mock.Of<IMemberType>(t =>
            t.Id == id &&
            t.Alias == alias &&
            t.Name == name &&
            t.Description == "desc" &&
            t.CreateDate == DateTime.UtcNow &&
            t.UpdateDate == DateTime.UtcNow);

    [Fact]
    public async Task GetAll_ReturnsOkWithList()
    {
        var types = new[] { MakeType(1, "staff", "Staff") };
        _service.Setup(s => s.GetAllAsync()).ReturnsAsync(types);

        var result = await _sut.GetAll();

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task GetByAlias_Valid_ReturnsOk()
    {
        _service.Setup(s => s.GetByAliasAsync("staff")).ReturnsAsync(MakeType(1, "staff", "Staff"));

        var result = await _sut.GetByAlias("staff");

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task GetByAlias_Empty_ReturnsBadRequest()
    {
        var result = await _sut.GetByAlias("");

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task GetByAlias_NotFound_ReturnsNotFound()
    {
        _service.Setup(s => s.GetByAliasAsync("nope")).ReturnsAsync((IMemberType?)null);

        var result = await _sut.GetByAlias("nope");

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Create_Valid_ReturnsOk()
    {
        _service.Setup(s => s.CreateAsync("vip", "VIP", string.Empty)).ReturnsAsync(MakeType(2, "vip", "VIP"));

        var result = await _sut.Create(new CreateMemberTypeRequest("vip", "VIP", null));

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task Update_Valid_ReturnsOk()
    {
        _service.Setup(s => s.UpdateAsync("staff", "Staff Updated", string.Empty))
            .ReturnsAsync(MakeType(1, "staff", "Staff Updated"));

        var result = await _sut.Update("staff", new UpdateMemberTypeRequest("Staff Updated", null));

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task Update_EmptyAlias_ReturnsBadRequest()
    {
        var result = await _sut.Update("", new UpdateMemberTypeRequest("X", null));

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task Update_NotFound_ReturnsNotFound()
    {
        _service.Setup(s => s.UpdateAsync("nope", "X", string.Empty))
            .ThrowsAsync(new InvalidOperationException("Not found"));

        var result = await _sut.Update("nope", new UpdateMemberTypeRequest("X", null));

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task Delete_Valid_ReturnsOk()
    {
        _service.Setup(s => s.DeleteAsync("staff")).Returns(Task.CompletedTask);

        var result = await _sut.Delete("staff");

        Assert.IsType<OkResult>(result);
    }

    [Fact]
    public async Task Delete_EmptyAlias_ReturnsBadRequest()
    {
        var result = await _sut.Delete("");

        Assert.IsType<BadRequestObjectResult>(result);
    }
}
