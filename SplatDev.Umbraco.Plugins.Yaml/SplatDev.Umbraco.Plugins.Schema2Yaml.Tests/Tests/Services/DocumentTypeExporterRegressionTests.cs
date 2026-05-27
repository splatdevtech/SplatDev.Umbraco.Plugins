using Microsoft.Extensions.Logging;
using Moq;
using SplatDev.Umbraco.Plugins.Schema2Yaml.Services;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Services;

namespace SplatDev.Umbraco.Plugins.Schema2Yaml.Tests.Services;

/// <summary>
/// Regression tests for DocumentTypeExporter.
/// DocumentType export has no TFM-specific compile guards, so these tests run identically
/// across net8.0, net9.0, and net10.0 (Umbraco 14–17).
/// </summary>
public class DocumentTypeExporterRegressionTests
{
    private readonly Mock<IContentTypeService> _mockContentTypeService = new();
    private readonly Mock<IDataTypeService> _mockDataTypeService = new();
    private readonly Mock<ILogger<DocumentTypeExporter>> _mockLogger = new();
    private readonly DocumentTypeExporter _sut;

    public DocumentTypeExporterRegressionTests()
    {
        _sut = new DocumentTypeExporter(
            _mockContentTypeService.Object,
            _mockDataTypeService.Object,
            _mockLogger.Object);
    }

    // ── AllowedChildTypes regression ─────────────────────────────────────────

    [Fact]
    public async Task ExportAsync_MapsAllowedChildTypes_WhenPresent()
    {
var allowedChild = new ContentTypeSort(Guid.NewGuid(), 0, "article");
        var ct = CreateMinimalContentType("page", "Page");
        ct.Setup(c => c.AllowedContentTypes).Returns([allowedChild]);
        _mockContentTypeService.Setup(s => s.GetAll()).Returns([ct.Object]);

        var result = await _sut.ExportAsync();

        Assert.Single(result[0].AllowedChildTypes);
        Assert.Equal("article", result[0].AllowedChildTypes[0]);
    }

    [Fact]
    public async Task ExportAsync_ReturnsEmptyAllowedChildTypes_WhenNoneConfigured()
    {
        var ct = CreateMinimalContentType("page", "Page");
        _mockContentTypeService.Setup(s => s.GetAll()).Returns([ct.Object]);

        var result = await _sut.ExportAsync();

        Assert.Empty(result[0].AllowedChildTypes);
    }

    // ── AllowedTemplates / DefaultTemplate regression ────────────────────────

    [Fact]
    public async Task ExportAsync_MapsAllowedTemplates()
    {
        var tpl = new Mock<ITemplate>();
        tpl.Setup(t => t.Alias).Returns("homePage");

        var ct = CreateMinimalContentType("home", "Home");
        ct.Setup(c => c.AllowedTemplates).Returns([tpl.Object]);
        _mockContentTypeService.Setup(s => s.GetAll()).Returns([ct.Object]);

        var result = await _sut.ExportAsync();

        Assert.Single(result[0].AllowedTemplates);
        Assert.Equal("homePage", result[0].AllowedTemplates[0]);
    }

    [Fact]
    public async Task ExportAsync_MapsDefaultTemplate_WhenSet()
    {
        var tpl = new Mock<ITemplate>();
        tpl.Setup(t => t.Alias).Returns("homePage");

        var ct = CreateMinimalContentType("home", "Home");
        ct.Setup(c => c.DefaultTemplate).Returns(tpl.Object);
        _mockContentTypeService.Setup(s => s.GetAll()).Returns([ct.Object]);

        var result = await _sut.ExportAsync();

        Assert.Equal("homePage", result[0].DefaultTemplate);
    }

    [Fact]
    public async Task ExportAsync_LeavesDefaultTemplateNull_WhenNotSet()
    {
        var ct = CreateMinimalContentType("element", "Element");
        ct.Setup(c => c.DefaultTemplate).Returns((ITemplate?)null);
        _mockContentTypeService.Setup(s => s.GetAll()).Returns([ct.Object]);

        var result = await _sut.ExportAsync();

        Assert.Null(result[0].DefaultTemplate);
    }

    // ── Composition / self-exclusion regression ──────────────────────────────

    [Fact]
    public async Task ExportAsync_ExcludesSelf_FromCompositions()
    {
        var selfId = 10;
        var ct = CreateMinimalContentType("page", "Page", id: selfId);

        var selfComposition = new Mock<IContentTypeComposition>();
        selfComposition.Setup(c => c.Alias).Returns("page");
        selfComposition.Setup(c => c.Id).Returns(selfId);

        var otherComposition = new Mock<IContentTypeComposition>();
        otherComposition.Setup(c => c.Alias).Returns("seoMixin");
        otherComposition.Setup(c => c.Id).Returns(99);

        ct.Setup(c => c.ContentTypeComposition)
            .Returns([selfComposition.Object, otherComposition.Object]);
        _mockContentTypeService.Setup(s => s.GetAll()).Returns([ct.Object]);

        var result = await _sut.ExportAsync();

        Assert.Single(result[0].Compositions);
        Assert.Equal("seoMixin", result[0].Compositions[0]);
    }

    [Fact]
    public async Task ExportAsync_ReturnsEmptyCompositions_WhenNoCompositionsSet()
    {
        var ct = CreateMinimalContentType("page", "Page");
        _mockContentTypeService.Setup(s => s.GetAll()).Returns([ct.Object]);

        var result = await _sut.ExportAsync();

        Assert.Empty(result[0].Compositions);
    }

    // ── Tab / property export regression ─────────────────────────────────────

    [Fact]
    public async Task ExportAsync_ExportsTabs_WithProperties()
    {
        var dataTypeKey = Guid.NewGuid();
        var dataType = new Mock<IDataType>();
        dataType.Setup(d => d.Name).Returns("Textstring");
        _mockDataTypeService.Setup(s => s.GetAsync(It.IsAny<Guid>()))
            .ReturnsAsync(dataType.Object);

        var prop = new Mock<IPropertyType>();
        prop.Setup(p => p.Alias).Returns("title");
        prop.Setup(p => p.Name).Returns("Title");
        prop.Setup(p => p.DataTypeId).Returns(1);
        prop.Setup(p => p.DataTypeKey).Returns(dataTypeKey);
        prop.Setup(p => p.Mandatory).Returns(true);
        prop.Setup(p => p.Description).Returns("Page title");
        prop.Setup(p => p.SortOrder).Returns(0);

        var group = new PropertyGroup(new PropertyTypeCollection(true, [prop.Object]))
        {
            Alias = "content",
            Name = "Content",
            SortOrder = 0
        };

        var ct = CreateMinimalContentType("page", "Page");
        ct.Setup(c => c.PropertyGroups).Returns(new PropertyGroupCollection([group]));
        _mockContentTypeService.Setup(s => s.GetAll()).Returns([ct.Object]);

        var result = await _sut.ExportAsync();

        Assert.Single(result[0].Tabs);
        Assert.Equal("Content", result[0].Tabs[0].Name);
        Assert.Single(result[0].Tabs[0].Properties);
        Assert.Equal("title", result[0].Tabs[0].Properties[0].Alias);
        Assert.Equal("Textstring", result[0].Tabs[0].Properties[0].DataType);
        Assert.True(result[0].Tabs[0].Properties[0].Required);
    }

    [Fact]
    public async Task ExportAsync_TabsOrderedBySortOrder()
    {
        var ct = CreateMinimalContentType("page", "Page");
        var group1 = new PropertyGroup(new PropertyTypeCollection(true, [])) { Alias = "seo",     Name = "SEO",     SortOrder = 10 };
        var group2 = new PropertyGroup(new PropertyTypeCollection(true, [])) { Alias = "content", Name = "Content", SortOrder = 0 };
        // The exporter orders by SortOrder; pass groups in reverse order to verify sorting.
        ct.Setup(c => c.PropertyGroups).Returns(new PropertyGroupCollection([group1, group2]));
        _mockContentTypeService.Setup(s => s.GetAll()).Returns([ct.Object]);

        var result = await _sut.ExportAsync();

        Assert.Equal("Content", result[0].Tabs[0].Name);
        Assert.Equal("SEO",     result[0].Tabs[1].Name);
    }

    // ── Error resilience regression ───────────────────────────────────────────

    [Fact]
    public async Task ExportAsync_SkipsFailingContentType_AndContinuesExport()
    {
        // Throw on Icon (inside the try block); Name is used by the catch logger so
        // throwing there would cause the exception to escape the catch.
        var broken = new Mock<IContentType>();
        broken.Setup(c => c.Name).Returns("Broken");
        broken.Setup(c => c.Icon).Throws(new InvalidOperationException("DB timeout"));

        var good = CreateMinimalContentType("article", "Article");
        _mockContentTypeService.Setup(s => s.GetAll()).Returns([broken.Object, good.Object]);

        var result = await _sut.ExportAsync();

        Assert.Single(result);
        Assert.Equal("article", result[0].Alias);
    }

    // ── Constructor null guard regression ────────────────────────────────────

    [Fact]
    public void Constructor_ThrowsArgumentNullException_WhenContentTypeServiceIsNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new DocumentTypeExporter(null!, _mockDataTypeService.Object, _mockLogger.Object));
    }

    [Fact]
    public void Constructor_ThrowsArgumentNullException_WhenDataTypeServiceIsNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new DocumentTypeExporter(_mockContentTypeService.Object, null!, _mockLogger.Object));
    }

    private static Mock<IContentType> CreateMinimalContentType(string alias, string name, int id = 1)
    {
        var mock = new Mock<IContentType>();
        mock.Setup(ct => ct.Id).Returns(id);
        mock.Setup(ct => ct.Alias).Returns(alias);
        mock.Setup(ct => ct.Name).Returns(name);
        mock.Setup(ct => ct.Icon).Returns("icon-document");
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
