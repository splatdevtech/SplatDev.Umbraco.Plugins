using Microsoft.Extensions.Logging;
using Moq;
using SplatDev.Umbraco.Plugins.Schema2Yaml.Services;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Services;

namespace SplatDev.Umbraco.Plugins.Schema2Yaml.Tests.Services;

public class DocumentTypeExporterTests
{
    private readonly Mock<IContentTypeService> _mockContentTypeService;
    private readonly Mock<IDataTypeService> _mockDataTypeService;
    private readonly Mock<ILogger<DocumentTypeExporter>> _mockLogger;
    private readonly DocumentTypeExporter _sut;

    public DocumentTypeExporterTests()
    {
        _mockContentTypeService = new Mock<IContentTypeService>();
        _mockDataTypeService = new Mock<IDataTypeService>();
        _mockLogger = new Mock<ILogger<DocumentTypeExporter>>();

        _sut = new DocumentTypeExporter(
            _mockContentTypeService.Object,
            _mockDataTypeService.Object,
            _mockLogger.Object);
    }

    [Fact]
    public async Task ExportAsync_WhenNoContentTypes_ReturnsEmptyList()
    {
        _mockContentTypeService.Setup(s => s.GetAll()).Returns([]);

        var result = await _sut.ExportAsync();

        Assert.Empty(result);
    }

    [Fact]
    public async Task ExportAsync_MapsAliasNameAndIcon()
    {
        var mockContentType = CreateMinimalContentType("blogPost", "Blog Post", "icon-article");

        _mockContentTypeService.Setup(s => s.GetAll()).Returns([mockContentType.Object]);

        var result = await _sut.ExportAsync();

        Assert.Single(result);
        Assert.Equal("blogPost", result[0].Alias);
        Assert.Equal("Blog Post", result[0].Name);
        Assert.Equal("icon-article", result[0].Icon);
    }

    [Fact]
    public async Task ExportAsync_MapsIsElementFlag()
    {
        var mockContentType = CreateMinimalContentType("myElement", "My Element", null);
        mockContentType.Setup(ct => ct.IsElement).Returns(true);

        _mockContentTypeService.Setup(s => s.GetAll()).Returns([mockContentType.Object]);

        var result = await _sut.ExportAsync();

        Assert.Single(result);
        Assert.True(result[0].IsElement);
    }

    [Fact]
    public async Task ExportAsync_MapsAllowAsRoot()
    {
        var mockContentType = CreateMinimalContentType("home", "Home", null);
        mockContentType.Setup(ct => ct.AllowedAsRoot).Returns(true);

        _mockContentTypeService.Setup(s => s.GetAll()).Returns([mockContentType.Object]);

        var result = await _sut.ExportAsync();

        Assert.Single(result);
        Assert.True(result[0].AllowAsRoot);
    }

    [Fact]
    public async Task ExportAsync_MapsCompositions_ExcludesSelf()
    {
        var selfId = 42;
        var mockContentType = CreateMinimalContentType("page", "Page", null, id: selfId);

        var mockComposition = new Mock<IContentTypeComposition>();
        mockComposition.Setup(c => c.Alias).Returns("seo");
        mockComposition.Setup(c => c.Id).Returns(99);

        mockContentType.Setup(ct => ct.ContentTypeComposition)
            .Returns([mockComposition.Object]);

        _mockContentTypeService.Setup(s => s.GetAll()).Returns([mockContentType.Object]);

        var result = await _sut.ExportAsync();

        Assert.Single(result);
        Assert.Single(result[0].Compositions);
        Assert.Equal("seo", result[0].Compositions[0]);
    }

    [Fact]
    public async Task ExportAsync_ExportsMultipleContentTypes()
    {
        var mockCt1 = CreateMinimalContentType("home", "Home", null);
        var mockCt2 = CreateMinimalContentType("blogPost", "Blog Post", null);

        _mockContentTypeService.Setup(s => s.GetAll())
            .Returns([mockCt1.Object, mockCt2.Object]);

        var result = await _sut.ExportAsync();

        Assert.Equal(2, result.Count);
        Assert.Equal("home", result[0].Alias);
        Assert.Equal("blogPost", result[1].Alias);
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
