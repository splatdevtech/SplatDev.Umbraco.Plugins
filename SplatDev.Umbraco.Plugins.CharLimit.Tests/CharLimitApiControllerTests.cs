using Microsoft.AspNetCore.Mvc;
using Xunit;
using SplatDev.Umbraco.Plugins.CharLimit.Controllers;

namespace SplatDev.Umbraco.Plugins.CharLimit.Tests;

public class CharLimitApiControllerTests
{
    [Fact]
    public void GetConfig_ReturnsOk()
    {
        var sut = new CharLimitApiController();
        Assert.IsType<OkObjectResult>(sut.GetConfig());
    }
}
