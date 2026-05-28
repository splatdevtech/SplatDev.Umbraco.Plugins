using Microsoft.AspNetCore.Mvc;using Moq;using Xunit;
using SplatDev.Umbraco.Plugins.Restricted.Controllers;using SplatDev.Umbraco.Plugins.Restricted.Services;
namespace SplatDev.Umbraco.Plugins.Restricted.Tests;
public class RestrictedApiControllerTests{readonly Mock<IRestrictedContentService>_s=new();readonly RestrictedApiController _c;
public RestrictedApiControllerTests()=>_c=new(_s.Object);
[Fact]public async Task GetRestrictedNodes_Ok()=>Assert.IsType<OkObjectResult>(await _c.GetRestrictedNodes());
}
