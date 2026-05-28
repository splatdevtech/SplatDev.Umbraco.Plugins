using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using SplatDev.Umbraco.Plugins.PropertiesReport.Controllers;
using SplatDev.Umbraco.Plugins.PropertiesReport.Models;
using SplatDev.Umbraco.Plugins.PropertiesReport.Services;

namespace SplatDev.Umbraco.Plugins.PropertiesReport.Tests;

public class PropertiesReportApiControllerTests
{
    private readonly Mock<IPropertiesReportService> _service;
    private readonly PropertiesReportApiController _sut;

    public PropertiesReportApiControllerTests()
    {
        _service = new Mock<IPropertiesReportService>();
        _sut = new PropertiesReportApiController(_service.Object);
    }

    [Fact]
    public void GetReport_ReturnsOk()
    {
        _service.Setup(s => s.GetReport()).Returns(new PropertiesReportResult
        {
            Items = [new PropertyReportItem { ContentTypeAlias = "home", PropertyAlias = "title" }]
        });

        var result = _sut.GetReport();

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public void GetByContentType_Valid_ReturnsOk()
    {
        _service.Setup(s => s.GetByContentType("home")).Returns(new PropertiesReportResult
        {
            Items = [new PropertyReportItem { ContentTypeAlias = "home", PropertyAlias = "title" }]
        });

        var result = _sut.GetByContentType("home");

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public void GetByContentType_Empty_ReturnsBadRequest()
    {
        var result = _sut.GetByContentType("");

        Assert.IsType<BadRequestObjectResult>(result);
    }
}
