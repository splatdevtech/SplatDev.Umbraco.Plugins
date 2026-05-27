using Microsoft.Extensions.Logging;
using Moq;
using SplatDev.Umbraco.Plugins.Schema2Yaml.Services;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Services;

namespace SplatDev.Umbraco.Plugins.Schema2Yaml.Tests.Services;

public class DataTypeExporterTests
{
    private readonly Mock<IDataTypeService> _mockDataTypeService;
    private readonly Mock<IUmbracoVersion> _mockUmbracoVersion;
    private readonly Mock<ILogger<UmbracoVersionDetector>> _mockVersionLogger;
    private readonly Mock<ILogger<DataTypeExporter>> _mockLogger;
    private readonly UmbracoVersionDetector _versionDetector;
    private readonly DataTypeExporter _sut;

    public DataTypeExporterTests()
    {
        _mockDataTypeService = new Mock<IDataTypeService>();
        _mockUmbracoVersion = new Mock<IUmbracoVersion>();
        _mockVersionLogger = new Mock<ILogger<UmbracoVersionDetector>>();
        _mockLogger = new Mock<ILogger<DataTypeExporter>>();

        _mockUmbracoVersion.Setup(v => v.Version).Returns(new Version(17, 0, 0));
        _versionDetector = new UmbracoVersionDetector(_mockUmbracoVersion.Object, _mockVersionLogger.Object);

        _sut = new DataTypeExporter(_mockDataTypeService.Object, _versionDetector, _mockLogger.Object);
    }

    [Fact]
    public async Task ExportAsync_WhenNoDataTypes_ReturnsEmptyList()
    {
        _mockDataTypeService.Setup(s => s.GetAllAsync())
            .ReturnsAsync(Enumerable.Empty<IDataType>());

        var result = await _sut.ExportAsync();

        Assert.Empty(result);
    }

    [Fact]
    public async Task ExportAsync_MapsNameAndEditorAlias()
    {
        var mockDataType = new Mock<IDataType>();
        mockDataType.Setup(dt => dt.Name).Returns("Textstring");
        mockDataType.Setup(dt => dt.EditorUiAlias).Returns("Umb.PropertyEditorUi.TextBox");
        mockDataType.Setup(dt => dt.ConfigurationObject).Returns(null as object);
        mockDataType.Setup(dt => dt.DatabaseType).Returns(ValueStorageType.Ntext);

        _mockDataTypeService.Setup(s => s.GetAllAsync())
            .ReturnsAsync(new[] { mockDataType.Object });

        var result = await _sut.ExportAsync();

        Assert.Single(result);
        Assert.Equal("Textstring", result[0].Name);
        Assert.Equal("Umb.PropertyEditorUi.TextBox", result[0].EditorUiAlias);
    }

    [Fact]
    public async Task ExportAsync_GeneratesAlias_AsCamelCase()
    {
        var mockDataType = new Mock<IDataType>();
        mockDataType.Setup(dt => dt.Name).Returns("Rich Text Editor");
        mockDataType.Setup(dt => dt.EditorUiAlias).Returns("Umb.PropertyEditorUi.TinyMce");
        mockDataType.Setup(dt => dt.ConfigurationObject).Returns(null as object);
        mockDataType.Setup(dt => dt.DatabaseType).Returns(ValueStorageType.Ntext);

        _mockDataTypeService.Setup(s => s.GetAllAsync())
            .ReturnsAsync(new[] { mockDataType.Object });

        var result = await _sut.ExportAsync();

        Assert.Single(result);
        Assert.Equal("richTextEditor", result[0].Alias);
    }

    [Fact]
    public async Task ExportAsync_ExportsMultipleDataTypes()
    {
        var mockDt1 = new Mock<IDataType>();
        mockDt1.Setup(dt => dt.Name).Returns("Textstring");
        mockDt1.Setup(dt => dt.EditorUiAlias).Returns("Umb.PropertyEditorUi.TextBox");
        mockDt1.Setup(dt => dt.ConfigurationObject).Returns(null as object);
        mockDt1.Setup(dt => dt.DatabaseType).Returns(ValueStorageType.Nvarchar);

        var mockDt2 = new Mock<IDataType>();
        mockDt2.Setup(dt => dt.Name).Returns("Numeric");
        mockDt2.Setup(dt => dt.EditorUiAlias).Returns("Umb.PropertyEditorUi.Integer");
        mockDt2.Setup(dt => dt.ConfigurationObject).Returns(null as object);
        mockDt2.Setup(dt => dt.DatabaseType).Returns(ValueStorageType.Integer);

        _mockDataTypeService.Setup(s => s.GetAllAsync())
            .ReturnsAsync(new[] { mockDt1.Object, mockDt2.Object });

        var result = await _sut.ExportAsync();

        Assert.Equal(2, result.Count);
        Assert.Equal("Textstring", result[0].Name);
        Assert.Equal("Numeric", result[1].Name);
    }

    [Fact]
    public async Task ExportAsync_SetsValueType_FromDatabaseType()
    {
        var mockDataType = new Mock<IDataType>();
        mockDataType.Setup(dt => dt.Name).Returns("Number");
        mockDataType.Setup(dt => dt.EditorUiAlias).Returns("Umb.PropertyEditorUi.Integer");
        mockDataType.Setup(dt => dt.ConfigurationObject).Returns(null as object);
        mockDataType.Setup(dt => dt.DatabaseType).Returns(ValueStorageType.Integer);

        _mockDataTypeService.Setup(s => s.GetAllAsync())
            .ReturnsAsync(new[] { mockDataType.Object });

        var result = await _sut.ExportAsync();

        Assert.Single(result);
        Assert.Equal("Integer", result[0].ValueType);
    }
}
