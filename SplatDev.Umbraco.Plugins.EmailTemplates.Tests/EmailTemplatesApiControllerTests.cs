using Microsoft.AspNetCore.Mvc;using Moq;using Xunit;using Microsoft.Extensions.Logging;
using SplatDev.Umbraco.Plugins.EmailTemplates.Controllers;using SplatDev.Umbraco.Plugins.EmailTemplates.Services;
namespace SplatDev.Umbraco.Plugins.EmailTemplates.Tests;
public class EmailTemplatesApiControllerTests{
Mock<IEmailTemplateService>_t=new();Mock<IEmailStyleService>_s=new();Mock<ILogger<EmailTemplatesApiController>>_l=new();EmailTemplatesApiController _c;
public EmailTemplatesApiControllerTests()=>_c=new(_t.Object,_s.Object,_l.Object);
[Fact]public void GetAll_Ok(){_t.Setup(t=>t.GetAll()).Returns([]);Assert.IsType<OkObjectResult>(_c.GetAll());}
}
