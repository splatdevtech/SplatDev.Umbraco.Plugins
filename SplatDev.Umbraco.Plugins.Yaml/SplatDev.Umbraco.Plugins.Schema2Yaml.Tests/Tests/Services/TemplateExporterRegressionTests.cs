using Microsoft.Extensions.Logging;
using Moq;
using SplatDev.Umbraco.Plugins.Schema2Yaml.Services;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Services;

namespace SplatDev.Umbraco.Plugins.Schema2Yaml.Tests.Services;

/// <summary>
/// Regression tests for TemplateExporter.
/// Template export has no TFM-specific compile guards; tests run identically on net8.0–net10.0,
/// covering Umbraco 14–17.
/// </summary>
public class TemplateExporterRegressionTests
{
    private readonly Mock<ITemplateService> _mockTemplateService = new();
    private readonly Mock<ILogger<TemplateExporter>> _mockLogger = new();
    private readonly TemplateExporter _sut;

    public TemplateExporterRegressionTests()
    {
        _sut = new TemplateExporter(_mockTemplateService.Object, _mockLogger.Object);
    }

    // ── Core field mapping regression ────────────────────────────────────────

    [Fact]
    public async Task ExportAsync_MapsAllFourFields()
    {
        var tpl = BuildTemplate("homePage", "Home Page", masterAlias: "_Layout", content: "@inherits UmbracoViewPage\n<h1>Home</h1>");
        _mockTemplateService.Setup(s => s.GetAllAsync(It.IsAny<string[]>()))
            .ReturnsAsync(new[] { tpl });

        var result = await _sut.ExportAsync();

        Assert.Single(result);
        Assert.Equal("homePage", result[0].Alias);
        Assert.Equal("Home Page", result[0].Name);
        Assert.Equal("_Layout", result[0].MasterTemplate);
        Assert.Equal("@inherits UmbracoViewPage\n<h1>Home</h1>", result[0].Content);
    }

    // ── MasterTemplate null/set regression ───────────────────────────────────

    [Fact]
    public async Task ExportAsync_LeavesMasterTemplateNull_WhenNotSet()
    {
        var tpl = BuildTemplate("master", "Master", masterAlias: null, content: "@inherits UmbracoViewPage");
        _mockTemplateService.Setup(s => s.GetAllAsync(It.IsAny<string[]>()))
            .ReturnsAsync(new[] { tpl });

        var result = await _sut.ExportAsync();

        Assert.Single(result);
        Assert.Null(result[0].MasterTemplate);
    }

    [Fact]
    public async Task ExportAsync_MapsMasterTemplateAlias_WhenSet()
    {
        var tpl = BuildTemplate("page", "Page", masterAlias: "master", content: "");
        _mockTemplateService.Setup(s => s.GetAllAsync(It.IsAny<string[]>()))
            .ReturnsAsync(new[] { tpl });

        var result = await _sut.ExportAsync();

        Assert.Equal("master", result[0].MasterTemplate);
    }

    // ── Empty content regression ──────────────────────────────────────────────

    [Fact]
    public async Task ExportAsync_MapsEmptyContent_WhenTemplateHasNoRazor()
    {
        var tpl = BuildTemplate("blank", "Blank", masterAlias: null, content: null);
        _mockTemplateService.Setup(s => s.GetAllAsync(It.IsAny<string[]>()))
            .ReturnsAsync(new[] { tpl });

        var result = await _sut.ExportAsync();

        Assert.Single(result);
        Assert.Null(result[0].Content);
    }

    // ── Multiple templates regression ────────────────────────────────────────

    [Theory]
    [InlineData("homePage", "Home Page", "_Layout")]
    [InlineData("contentPage", "Content Page", "_Layout")]
    [InlineData("landingPage", "Landing Page", null)]
    [InlineData("blogPost", "Blog Post", "contentPage")]
    [InlineData("_Layout", "Master Layout", null)]
    public async Task ExportAsync_MapsEachTemplate(string alias, string name, string? masterAlias)
    {
        var tpl = BuildTemplate(alias, name, masterAlias, content: "");
        _mockTemplateService.Setup(s => s.GetAllAsync(It.IsAny<string[]>()))
            .ReturnsAsync(new[] { tpl });

        var result = await _sut.ExportAsync();

        Assert.Single(result);
        Assert.Equal(alias, result[0].Alias);
        Assert.Equal(name, result[0].Name);
        Assert.Equal(masterAlias, result[0].MasterTemplate);
    }

    [Fact]
    public async Task ExportAsync_PreservesOrderReturnedByFileService()
    {
        var templates = new[]
        {
            BuildTemplate("_Layout",     "Master",       null,      ""),
            BuildTemplate("homePage",    "Home Page",    "_Layout", ""),
            BuildTemplate("contentPage", "Content Page", "_Layout", ""),
        };
        _mockTemplateService.Setup(s => s.GetAllAsync(It.IsAny<string[]>()))
            .ReturnsAsync(templates);

        var result = await _sut.ExportAsync();

        Assert.Equal(3, result.Count);
        Assert.Equal("_Layout",     result[0].Alias);
        Assert.Equal("homePage",    result[1].Alias);
        Assert.Equal("contentPage", result[2].Alias);
    }

    // ── Error resilience regression ───────────────────────────────────────────

    [Fact]
    public async Task ExportAsync_SkipsFailingTemplate_AndContinuesExport()
    {
        // Throw on Content (inside the try block); Name is used by the catch logger so
        // throwing there would cause the exception to escape the catch.
        var broken = new Mock<ITemplate>();
        broken.Setup(t => t.Alias).Returns("broken");
        broken.Setup(t => t.Name).Returns("Broken Template");
        broken.Setup(t => t.MasterTemplateAlias).Returns((string?)null);
        broken.Setup(t => t.Content).Throws(new InvalidOperationException("Disk read error"));

        var good = BuildTemplate("homePage", "Home Page", null, "");
        _mockTemplateService.Setup(s => s.GetAllAsync(It.IsAny<string[]>()))
            .ReturnsAsync(new ITemplate[] { broken.Object, good });

        var result = await _sut.ExportAsync();

        Assert.Single(result);
        Assert.Equal("homePage", result[0].Alias);
    }

    [Fact]
    public async Task ExportAsync_ReturnsEmptyList_WhenNoTemplatesExist()
    {
        _mockTemplateService.Setup(s => s.GetAllAsync(It.IsAny<string[]>()))
            .ReturnsAsync(Enumerable.Empty<ITemplate>());

        var result = await _sut.ExportAsync();

        Assert.Empty(result);
    }

    // ── Constructor null guard regression ────────────────────────────────────

    [Fact]
    public void Constructor_ThrowsArgumentNullException_WhenFileServiceIsNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new TemplateExporter(null!, _mockLogger.Object));
    }

    [Fact]
    public void Constructor_ThrowsArgumentNullException_WhenLoggerIsNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new TemplateExporter(_mockTemplateService.Object, null!));
    }

    private static ITemplate BuildTemplate(string alias, string name, string? masterAlias, string? content)
    {
        var mock = new Mock<ITemplate>();
        mock.Setup(t => t.Alias).Returns(alias);
        mock.Setup(t => t.Name).Returns(name);
        mock.Setup(t => t.MasterTemplateAlias).Returns(masterAlias);
        mock.Setup(t => t.Content).Returns(content);
        return mock.Object;
    }
}
