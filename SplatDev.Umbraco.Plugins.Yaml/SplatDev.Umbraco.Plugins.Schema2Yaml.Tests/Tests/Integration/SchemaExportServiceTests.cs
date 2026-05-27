using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using SplatDev.Umbraco.Plugins.Schema2Yaml.Configuration;
using SplatDev.Umbraco.Plugins.Schema2Yaml.Services;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Services;
using System.IO.Compression;
using ITemplate = Umbraco.Cms.Core.Models.ITemplate;
using IDictionaryItem = Umbraco.Cms.Core.Models.IDictionaryItem;
using ILanguage = Umbraco.Cms.Core.Models.ILanguage;
using IDataType = Umbraco.Cms.Core.Models.IDataType;

namespace SplatDev.Umbraco.Plugins.Schema2Yaml.Tests.Integration;

/// <summary>
/// Integration tests for SchemaExportService orchestrating all exporters.
/// All Umbraco service dependencies return empty sets to isolate orchestration logic.
/// </summary>
public class SchemaExportServiceTests : IDisposable
{
    private readonly SchemaExportService _sut;
    private readonly IOptions<Schema2YamlOptions> _options;

    public SchemaExportServiceTests()
    {
        _options = Options.Create(new Schema2YamlOptions
        {
            IncludeContent = false,
            IncludeMedia = false,
            IncludeMembers = false,
            IncludeUsers = false,
            IncludeDictionary = true,
            IncludeLanguages = true
        });

        _sut = CreateService(_options);
    }

    [Fact]
    public async Task ExportToYamlAsync_ReturnsValidYaml()
    {
        var yaml = await _sut.ExportToYamlAsync();

        Assert.NotEmpty(yaml);
        Assert.Contains("umbraco:", yaml);
    }

    [Fact]
    public async Task ExportToYamlAsync_ContainsAllSectionKeys()
    {
        var yaml = await _sut.ExportToYamlAsync();

        Assert.Contains("languages:", yaml);
        Assert.Contains("dataTypes:", yaml);
        Assert.Contains("documentTypes:", yaml);
        Assert.Contains("mediaTypes:", yaml);
        Assert.Contains("templates:", yaml);
        Assert.Contains("media:", yaml);
        Assert.Contains("content:", yaml);
        Assert.Contains("dictionaryItems:", yaml);
        Assert.Contains("members:", yaml);
        Assert.Contains("users:", yaml);
    }

    [Fact]
    public async Task ExportToYamlAsync_UpdatesStatistics()
    {
        await _sut.ExportToYamlAsync();

        var stats = _sut.GetLastExportStatistics();

        Assert.NotNull(stats);
        Assert.NotEqual(default, stats.ExportDate);
        Assert.NotEmpty(stats.UmbracoVersion);
    }

    [Fact]
    public async Task ExportToYamlAsync_StatisticsReflectEmptyExport()
    {
        await _sut.ExportToYamlAsync();

        var stats = _sut.GetLastExportStatistics();

        Assert.Equal(0, stats.DataTypeCount);
        Assert.Equal(0, stats.DocumentTypeCount);
        Assert.Equal(0, stats.LanguageCount);
        Assert.Equal(0, stats.TemplateCount);
    }

    [Fact]
    public void GetLastExportStatistics_BeforeExport_ReturnsDefaultStatistics()
    {
        var service = CreateService(_options);
        var stats = service.GetLastExportStatistics();

        Assert.NotNull(stats);
        Assert.Equal(0, stats.DataTypeCount);
    }

    [Fact]
    public async Task ExportToFileAsync_WritesFileToPath()
    {
        var filePath = Path.Combine(Path.GetTempPath(), $"schema2yaml_test_{Guid.NewGuid():N}.yml");

        try
        {
            await _sut.ExportToFileAsync(filePath);

            Assert.True(File.Exists(filePath));
            var content = await File.ReadAllTextAsync(filePath);
            Assert.Contains("umbraco:", content);
        }
        finally
        {
            if (File.Exists(filePath))
                File.Delete(filePath);
        }
    }

    [Fact]
    public async Task ExportToFileAsync_CreatesDirectoryIfNotExists()
    {
        var dir = Path.Combine(Path.GetTempPath(), $"schema2yaml_{Guid.NewGuid():N}");
        var filePath = Path.Combine(dir, "export.yml");

        try
        {
            await _sut.ExportToFileAsync(filePath);

            Assert.True(File.Exists(filePath));
        }
        finally
        {
            if (Directory.Exists(dir))
                Directory.Delete(dir, recursive: true);
        }
    }

    [Fact]
    public async Task ExportToZipAsync_ReturnsNonEmptyBytes()
    {
        var zipBytes = await _sut.ExportToZipAsync();

        Assert.NotEmpty(zipBytes);
    }

    [Fact]
    public async Task ExportToZipAsync_ContainsYamlEntry()
    {
        var zipBytes = await _sut.ExportToZipAsync();

        using var stream = new MemoryStream(zipBytes);
        using var archive = new ZipArchive(stream, ZipArchiveMode.Read);

        Assert.Contains(archive.Entries, e => e.Name == "umbraco.yml");
    }

    [Fact]
    public async Task ExportToZipAsync_YamlEntryContainsValidContent()
    {
        var zipBytes = await _sut.ExportToZipAsync();

        using var stream = new MemoryStream(zipBytes);
        using var archive = new ZipArchive(stream, ZipArchiveMode.Read);

        var entry = archive.Entries.First(e => e.Name == "umbraco.yml");
        using var reader = new StreamReader(entry.Open());
        var content = await reader.ReadToEndAsync();

        Assert.Contains("umbraco:", content);
    }

    [Fact]
    public async Task ExportToYamlAsync_StatisticsDuration_IsPositive()
    {
        await _sut.ExportToYamlAsync();

        var stats = _sut.GetLastExportStatistics();

        Assert.True(stats.Duration >= TimeSpan.Zero);
    }

    private static SchemaExportService CreateService(IOptions<Schema2YamlOptions> options)
    {
        var mockLanguageService = new Mock<ILanguageService>();
        mockLanguageService.Setup(s => s.GetAllAsync())
            .ReturnsAsync(Enumerable.Empty<ILanguage>());

        var mockDictionaryItemService = new Mock<IDictionaryItemService>();
        mockDictionaryItemService.Setup(s => s.GetAtRootAsync())
            .ReturnsAsync(Enumerable.Empty<IDictionaryItem>());

        var mockDataTypeService = new Mock<IDataTypeService>();
        mockDataTypeService.Setup(s => s.GetAllAsync())
            .ReturnsAsync(Enumerable.Empty<IDataType>());

        var mockContentTypeService = new Mock<IContentTypeService>();
        mockContentTypeService.Setup(s => s.GetAll()).Returns([]);

        var mockMediaTypeService = new Mock<IMediaTypeService>();
        mockMediaTypeService.Setup(s => s.GetAll()).Returns([]);

        var mockTemplateService = new Mock<ITemplateService>();
        mockTemplateService.Setup(s => s.GetAllAsync(It.IsAny<string[]>()))
            .ReturnsAsync(Enumerable.Empty<ITemplate>());

        var mockMediaService = new Mock<IMediaService>();
        mockMediaService.Setup(s => s.GetRootMedia()).Returns([]);

        var mockContentService = new Mock<IContentService>();
        mockContentService.Setup(s => s.GetRootContent()).Returns([]);

        var mockMemberService = new Mock<IMemberService>();
        var total = 0L;
        mockMemberService.Setup(s => s.GetAll(0, int.MaxValue, out total)).Returns([]);

        var mockUserService = new Mock<IUserService>();
        mockUserService.Setup(s => s.GetAll(0, int.MaxValue, out total)).Returns([]);

        var mockWebHostEnv = new Mock<IWebHostEnvironment>();
        mockWebHostEnv.Setup(h => h.WebRootPath).Returns(string.Empty);

        var mockUmbracoVersion = new Mock<IUmbracoVersion>();
        mockUmbracoVersion.Setup(v => v.Version).Returns(new Version(17, 0, 0));

        var versionDetector = new UmbracoVersionDetector(
            mockUmbracoVersion.Object,
            Mock.Of<ILogger<UmbracoVersionDetector>>());

        var languageExporter = new LanguageExporter(
            mockLanguageService.Object,
            Mock.Of<ILogger<LanguageExporter>>());

        var dataTypeExporter = new DataTypeExporter(
            mockDataTypeService.Object,
            versionDetector,
            Mock.Of<ILogger<DataTypeExporter>>());

        var documentTypeExporter = new DocumentTypeExporter(
            mockContentTypeService.Object,
            mockDataTypeService.Object,
            Mock.Of<ILogger<DocumentTypeExporter>>());

        var mediaTypeExporter = new MediaTypeExporter(
            mockMediaTypeService.Object,
            mockDataTypeService.Object,
            Mock.Of<ILogger<MediaTypeExporter>>());

        var templateExporter = new TemplateExporter(
            mockTemplateService.Object,
            Mock.Of<ILogger<TemplateExporter>>());

        var mediaExporter = new MediaExporter(
            mockMediaService.Object,
            mockWebHostEnv.Object,
            options,
            Mock.Of<ILogger<MediaExporter>>());

        var contentExporter = new ContentExporter(
            mockContentService.Object,
            mockTemplateService.Object,
            options,
            Mock.Of<ILogger<ContentExporter>>());

        var dictionaryExporter = new DictionaryExporter(
            mockDictionaryItemService.Object,
            Mock.Of<ILogger<DictionaryExporter>>());

        var memberExporter = new MemberExporter(
            mockMemberService.Object,
            options,
            Mock.Of<ILogger<MemberExporter>>());

        var userExporter = new UserExporter(
            mockUserService.Object,
            options,
            Mock.Of<ILogger<UserExporter>>());

        return new SchemaExportService(
            languageExporter,
            dataTypeExporter,
            documentTypeExporter,
            mediaTypeExporter,
            templateExporter,
            mediaExporter,
            contentExporter,
            dictionaryExporter,
            memberExporter,
            userExporter,
            versionDetector,
            options,
            Mock.Of<ILogger<SchemaExportService>>());
    }

    public void Dispose() { }
}
