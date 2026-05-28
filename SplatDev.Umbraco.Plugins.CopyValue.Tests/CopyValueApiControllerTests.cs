using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using SplatDev.Umbraco.Plugins.CopyValue.Controllers;
using SplatDev.Umbraco.Plugins.CopyValue.Services;

namespace SplatDev.Umbraco.Plugins.CopyValue.Tests;
public class CopyValueApiControllerTests
{
    private readonly Mock<ICopyValueService> _svc = new();
    private readonly CopyValueApiController _sut;
    public CopyValueApiControllerTests() => _sut = new(_svc.Object);
    [Fact] public async Task GetMappings_ReturnsOk() => Assert.IsType<OkObjectResult>(await _sut.GetMappings());
    [Fact] public async Task GetMapping_NotFound() => Assert.IsType<NotFoundResult>(await _sut.GetMapping(99));
    [Fact] public async Task DeleteMapping_ReturnsOk() => Assert.IsType<OkResult>(await _sut.DeleteMapping(1));
}
