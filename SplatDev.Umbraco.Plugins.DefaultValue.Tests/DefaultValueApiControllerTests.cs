using Microsoft.AspNetCore.Mvc;using Moq;using Xunit;
using SplatDev.Umbraco.Plugins.DefaultValue.Controllers;using SplatDev.Umbraco.Plugins.DefaultValue.Services;
namespace SplatDev.Umbraco.Plugins.DefaultValue.Tests;
public class DefaultValueApiControllerTests{private readonly Mock<IDefaultValueService>_s=new();private readonly DefaultValueApiController _c;
public DefaultValueApiControllerTests()=>_c=new(_s.Object);
[Fact]public async Task GetRules_Ok()=>Assert.IsType<OkObjectResult>(await _c.GetRules());
[Fact]public async Task GetRulesForType_Missing_ReturnsBadRequest()=>Assert.IsType<BadRequestObjectResult>(await _c.GetRulesForType(""));
}
