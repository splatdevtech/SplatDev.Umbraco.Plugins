using Microsoft.Extensions.Logging;
using Moq;
using SplatDev.Umbraco.Plugins.Schema2Yaml.Services;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Services;

namespace SplatDev.Umbraco.Plugins.Schema2Yaml.Tests.Services;

public class TemplateExporterTests
{
    private readonly Mock<ITemplateService> _mockTemplateService;
    private readonly Mock<ILogger<TemplateExporter>> _mockLogger;
    private readonly TemplateExporter _sut;

    public TemplateExporterTests()
    {
        _mockTemplateService = new Mock<ITemplateService>();
        _mockLogger = new Mock<ILogger<TemplateExporter>>();

        _sut = new TemplateExporter(_mockTemplateService.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task ExportAsync_WhenNoTemplates_ReturnsEmptyList()
    {
        _mockTemplateService.Setup(s => s.GetAllAsync(It.IsAny<string[]>()))
            .ReturnsAsync(Enumerable.Empty<ITemplate>());

        var result = await _sut.ExportAsync();

        Assert.Empty(result);
    }

    [Fact]
    public async Task ExportAsync_MapsAliasNameAndMasterTemplate()
    {
        var mockTemplate = new Mock<ITemplate>();
        mockTemplate.Setup(t => t.Alias).Returns("home");
        mockTemplate.Setup(t => t.Name).Returns("Home");
        mockTemplate.Setup(t => t.MasterTemplateAlias).Returns("master");
        mockTemplate.Setup(t => t.Content).Returns("@inherits Umbraco.Web.Mvc.UmbracoViewPage");

        _mockTemplateService.Setup(s => s.GetAllAsync(It.IsAny<string[]>()))
            .ReturnsAsync(new[] { mockTemplate.Object });

        var result = await _sut.ExportAsync();

        Assert.Single(result);
        Assert.Equal("home", result[0].Alias);
        Assert.Equal("Home", result[0].Name);
        Assert.Equal("master", result[0].MasterTemplate);
    }

    [Fact]
    public async Task ExportAsync_WhenNoMasterTemplate_SetsNull()
    {
        var mockTemplate = new Mock<ITemplate>();
        mockTemplate.Setup(t => t.Alias).Returns("base");
        mockTemplate.Setup(t => t.Name).Returns("Base");
        mockTemplate.Setup(t => t.MasterTemplateAlias).Returns((string?)null);
        mockTemplate.Setup(t => t.Content).Returns(string.Empty);

        _mockTemplateService.Setup(s => s.GetAllAsync(It.IsAny<string[]>()))
            .ReturnsAsync(new[] { mockTemplate.Object });

        var result = await _sut.ExportAsync();

        Assert.Single(result);
        Assert.Null(result[0].MasterTemplate);
    }

    [Fact]
    public async Task ExportAsync_ExportsMultipleTemplates()
    {
        var mockT1 = new Mock<ITemplate>();
        mockT1.Setup(t => t.Alias).Returns("master");
        mockT1.Setup(t => t.Name).Returns("Master");
        mockT1.Setup(t => t.MasterTemplateAlias).Returns((string?)null);
        mockT1.Setup(t => t.Content).Returns(string.Empty);

        var mockT2 = new Mock<ITemplate>();
        mockT2.Setup(t => t.Alias).Returns("home");
        mockT2.Setup(t => t.Name).Returns("Home");
        mockT2.Setup(t => t.MasterTemplateAlias).Returns("master");
        mockT2.Setup(t => t.Content).Returns(string.Empty);

        _mockTemplateService.Setup(s => s.GetAllAsync(It.IsAny<string[]>()))
            .ReturnsAsync(new[] { mockT1.Object, mockT2.Object });

        var result = await _sut.ExportAsync();

        Assert.Equal(2, result.Count);
        Assert.Equal("master", result[0].Alias);
        Assert.Equal("home", result[1].Alias);
    }
}
