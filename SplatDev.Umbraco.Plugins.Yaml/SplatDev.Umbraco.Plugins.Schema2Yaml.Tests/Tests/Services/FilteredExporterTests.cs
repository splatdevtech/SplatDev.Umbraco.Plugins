using Microsoft.Extensions.Logging;
using Moq;
using SplatDev.Umbraco.Plugins.Schema2Yaml.Models;
using SplatDev.Umbraco.Plugins.Schema2Yaml.Services;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Services;

namespace SplatDev.Umbraco.Plugins.Schema2Yaml.Tests.Services;

public class FilteredExporterTests
{
    private readonly Mock<IContentTypeService> _mockContentTypeService;
    private readonly Mock<IDataTypeService> _mockDataTypeService;
    private readonly Mock<ILogger<DocumentTypeExporter>> _mockLogger;
    private readonly DocumentTypeExporter _sut;

    public FilteredExporterTests()
    {
        _mockContentTypeService = new Mock<IContentTypeService>();
        _mockDataTypeService = new Mock<IDataTypeService>();
        _mockLogger = new Mock<ILogger<DocumentTypeExporter>>();

        _sut = new DocumentTypeExporter(
            _mockContentTypeService.Object,
            _mockDataTypeService.Object,
            _mockLogger.Object);

        // Set up two content types: "article" and "home"
        var ct1 = CreateMinimalContentType("article", "Article", null, id: 1);
        var ct2 = CreateMinimalContentType("home", "Home", null, id: 2);

        _mockContentTypeService.Setup(s => s.GetAll())
            .Returns([ct1.Object, ct2.Object]);
    }

    [Fact]
    public async Task Filter_IncludeAll_ReturnsAll()
    {
        var filter = new CategorySelection { IncludeAll = true, Aliases = [] };

        var result = await _sut.ExportAsync(filter);

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task Filter_ExcludeAll_ReturnsEmpty()
    {
        var filter = new CategorySelection { IncludeAll = false, Aliases = [] };

        var result = await _sut.ExportAsync(filter);

        Assert.Empty(result);
    }

    [Fact]
    public async Task Filter_SpecificAlias_ReturnsOnlyMatch()
    {
        var filter = new CategorySelection { IncludeAll = false, Aliases = ["article"] };

        var result = await _sut.ExportAsync(filter);

        Assert.Single(result);
        Assert.Equal("article", result[0].Alias);
    }

    private static Mock<IContentType> CreateMinimalContentType(
        string alias,
        string name,
        string? icon,
        int id = 1)
    {
        var mock = new Mock<IContentType>();
        mock.Setup(ct => ct.Id).Returns(id);
        mock.Setup(ct => ct.Alias).Returns(alias);
        mock.Setup(ct => ct.Name).Returns(name);
        mock.Setup(ct => ct.Icon).Returns(icon);
        mock.Setup(ct => ct.IsElement).Returns(false);
        mock.Setup(ct => ct.AllowedAsRoot).Returns(false);
        mock.Setup(ct => ct.AllowedContentTypes).Returns([]);
        mock.Setup(ct => ct.ContentTypeComposition).Returns([]);
        mock.Setup(ct => ct.AllowedTemplates).Returns([]);
        mock.Setup(ct => ct.DefaultTemplate).Returns((ITemplate?)null);
        mock.Setup(ct => ct.PropertyGroups).Returns(new PropertyGroupCollection([]));
        mock.Setup(ct => ct.NoGroupPropertyTypes).Returns(new PropertyTypeCollection(true, []));
        return mock;
    }
}
