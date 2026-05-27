using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using SplatDev.Umbraco.Plugins.Schema2Yaml.Configuration;
using SplatDev.Umbraco.Plugins.Schema2Yaml.Models;
using SplatDev.Umbraco.Plugins.Schema2Yaml.Services;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Services;
using System.IO.Compression;

namespace SplatDev.Umbraco.Plugins.Schema2Yaml.Tests.Integration;

/// <summary>
/// Integration tests for the filtered ExportToYamlAsync(ExportSelection) and
/// ExportToZipAsync(ExportSelection) overloads on SchemaExportService.
/// Uses NSubstitute to substitute concrete exporter classes (which have virtual
/// ExportAsync(CategorySelection) methods from Tasks 7 and 8).
/// </summary>
public class FilteredSchemaExportServiceTests
{
    private readonly SchemaExportService _sut;
    private readonly DocumentTypeExporter _documentTypeExporter;
    private readonly LanguageExporter _languageExporter;
    private readonly DataTypeExporter _dataTypeExporter;
    private readonly MediaTypeExporter _mediaTypeExporter;
    private readonly TemplateExporter _templateExporter;
    private readonly MediaExporter _mediaExporter;
    private readonly ContentExporter _contentExporter;
    private readonly DictionaryExporter _dictionaryExporter;
    private readonly MemberExporter _memberExporter;
    private readonly UserExporter _userExporter;
    private readonly UmbracoVersionDetector _versionDetector;

    public FilteredSchemaExportServiceTests()
    {
        var options = Options.Create(new Schema2YamlOptions
        {
            IncludeContent = true,
            IncludeMedia = true,
            IncludeMembers = true,
            IncludeUsers = true,
            IncludeDictionary = true,
            IncludeLanguages = true
        });

        // Substitute concrete exporter classes (virtual methods allow this).
        // Constructor args for NSubstitute.For<ConcreteClass>(args) must match a real constructor.
        // We pass Substitute.For<IInterface>() for interface dependencies.
        _versionDetector = Substitute.For<UmbracoVersionDetector>(
            Substitute.For<IUmbracoVersion>(),
            Substitute.For<ILogger<UmbracoVersionDetector>>());

        _languageExporter = Substitute.For<LanguageExporter>(
            Substitute.For<ILanguageService>(),
            Substitute.For<ILogger<LanguageExporter>>());

        _dataTypeExporter = Substitute.For<DataTypeExporter>(
            Substitute.For<IDataTypeService>(),
            _versionDetector,
            Substitute.For<ILogger<DataTypeExporter>>());

        _documentTypeExporter = Substitute.For<DocumentTypeExporter>(
            Substitute.For<IContentTypeService>(),
            Substitute.For<IDataTypeService>(),
            Substitute.For<ILogger<DocumentTypeExporter>>());

        _mediaTypeExporter = Substitute.For<MediaTypeExporter>(
            Substitute.For<IMediaTypeService>(),
            Substitute.For<IDataTypeService>(),
            Substitute.For<ILogger<MediaTypeExporter>>());

        _templateExporter = Substitute.For<TemplateExporter>(
            Substitute.For<ITemplateService>(),
            Substitute.For<ILogger<TemplateExporter>>());

        _mediaExporter = Substitute.For<MediaExporter>(
            Substitute.For<IMediaService>(),
            Substitute.For<IWebHostEnvironment>(),
            options,
            Substitute.For<ILogger<MediaExporter>>());

        _contentExporter = Substitute.For<ContentExporter>(
            Substitute.For<IContentService>(),
            Substitute.For<ITemplateService>(),
            options,
            Substitute.For<ILogger<ContentExporter>>());

        _dictionaryExporter = Substitute.For<DictionaryExporter>(
            Substitute.For<IDictionaryItemService>(),
            Substitute.For<ILogger<DictionaryExporter>>());

        _memberExporter = Substitute.For<MemberExporter>(
            Substitute.For<IMemberService>(),
            options,
            Substitute.For<ILogger<MemberExporter>>());

        _userExporter = Substitute.For<UserExporter>(
            Substitute.For<IUserService>(),
            options,
            Substitute.For<ILogger<UserExporter>>());

        // Set up default returns for all filtered ExportAsync(CategorySelection) calls
        _languageExporter
            .ExportAsync(Arg.Any<CategorySelection>())
            .Returns(Task.FromResult(new List<ExportLanguage>()));

        _dataTypeExporter
            .ExportAsync(Arg.Any<CategorySelection>())
            .Returns(Task.FromResult(new List<ExportDataType>()));

        _documentTypeExporter
            .ExportAsync(Arg.Any<CategorySelection>())
            .Returns(Task.FromResult(new List<ExportDocumentType>()));

        _mediaTypeExporter
            .ExportAsync(Arg.Any<CategorySelection>())
            .Returns(Task.FromResult(new List<ExportMediaType>()));

        _templateExporter
            .ExportAsync(Arg.Any<CategorySelection>())
            .Returns(Task.FromResult(new List<ExportTemplate>()));

        _mediaExporter
            .ExportAsync(Arg.Any<CategorySelection>())
            .Returns(Task.FromResult<(List<ExportMedia>, Dictionary<string, byte[]>)>(([], [])));

        _contentExporter
            .ExportAsync(Arg.Any<CategorySelection>())
            .Returns(Task.FromResult(new List<ExportContent>()));

        _dictionaryExporter
            .ExportAsync(Arg.Any<CategorySelection>())
            .Returns(Task.FromResult(new List<ExportDictionaryItem>()));

        _memberExporter
            .ExportAsync(Arg.Any<CategorySelection>())
            .Returns(Task.FromResult(new List<ExportMember>()));

        _userExporter
            .ExportAsync(Arg.Any<CategorySelection>())
            .Returns(Task.FromResult(new List<ExportUser>()));

        _versionDetector
            .GetVersionString()
            .Returns("14.0.0");

        _sut = new SchemaExportService(
            _languageExporter,
            _dataTypeExporter,
            _documentTypeExporter,
            _mediaTypeExporter,
            _templateExporter,
            _mediaExporter,
            _contentExporter,
            _dictionaryExporter,
            _memberExporter,
            _userExporter,
            _versionDetector,
            options,
            Substitute.For<ILogger<SchemaExportService>>());
    }

    [Fact]
    public async Task ExportToYamlAsync_WithSelection_PassesFilterToExporters()
    {
        var selection = new ExportSelection();
        selection.DocumentTypes.IncludeAll = false;
        selection.DocumentTypes.Aliases    = ["article"];

        await _sut.ExportToYamlAsync(selection);

        await _documentTypeExporter.Received(1).ExportAsync(
            Arg.Is<CategorySelection>(f => !f.IncludeAll && f.Aliases.Contains("article")));
    }

    [Fact]
    public async Task ExportToYamlAsync_WithSelection_CallsAllExporters()
    {
        var selection = new ExportSelection();

        await _sut.ExportToYamlAsync(selection);

        await _languageExporter.Received(1).ExportAsync(Arg.Any<CategorySelection>());
        await _dataTypeExporter.Received(1).ExportAsync(Arg.Any<CategorySelection>());
        await _documentTypeExporter.Received(1).ExportAsync(Arg.Any<CategorySelection>());
        await _mediaTypeExporter.Received(1).ExportAsync(Arg.Any<CategorySelection>());
        await _templateExporter.Received(1).ExportAsync(Arg.Any<CategorySelection>());
        await _contentExporter.Received(1).ExportAsync(Arg.Any<CategorySelection>());
        await _dictionaryExporter.Received(1).ExportAsync(Arg.Any<CategorySelection>());
        await _memberExporter.Received(1).ExportAsync(Arg.Any<CategorySelection>());
        await _userExporter.Received(1).ExportAsync(Arg.Any<CategorySelection>());
    }

    [Fact]
    public async Task ExportToYamlAsync_WithSelection_ReturnsValidYaml()
    {
        var selection = new ExportSelection();

        var yaml = await _sut.ExportToYamlAsync(selection);

        Assert.NotEmpty(yaml);
        Assert.Contains("umbraco:", yaml);
    }

    [Fact]
    public async Task ExportToYamlAsync_WithSelection_PassesLanguageSelectionCorrectly()
    {
        var selection = new ExportSelection();
        selection.Languages.IncludeAll = false;
        selection.Languages.Aliases    = ["en-US", "fr-FR"];

        await _sut.ExportToYamlAsync(selection);

        await _languageExporter.Received(1).ExportAsync(
            Arg.Is<CategorySelection>(f =>
                !f.IncludeAll &&
                f.Aliases.Contains("en-US") &&
                f.Aliases.Contains("fr-FR")));
    }

    [Fact]
    public async Task ExportToYamlAsync_WithSelection_UpdatesStatistics()
    {
        var selection = new ExportSelection();

        await _sut.ExportToYamlAsync(selection);

        var stats = _sut.GetLastExportStatistics();
        Assert.NotNull(stats);
        Assert.NotEqual(default, stats.ExportDate);
        Assert.Equal("14.0.0", stats.UmbracoVersion);
    }

    [Fact]
    public async Task ExportToZipAsync_WithSelection_PassesMediaSelectionToMediaExporter()
    {
        var selection = new ExportSelection();
        selection.Media.IncludeAll = false;

        await _sut.ExportToZipAsync(selection);

        await _mediaExporter.Received().ExportAsync(
            Arg.Is<CategorySelection>(f => !f.IncludeAll));
    }

    [Fact]
    public async Task ExportToZipAsync_WithSelection_ReturnsNonEmptyBytes()
    {
        var selection = new ExportSelection();

        var bytes = await _sut.ExportToZipAsync(selection);

        Assert.NotEmpty(bytes);
    }

    [Fact]
    public async Task ExportToZipAsync_WithSelection_ContainsYamlEntry()
    {
        var selection = new ExportSelection();

        var zipBytes = await _sut.ExportToZipAsync(selection);

        using var stream  = new MemoryStream(zipBytes);
        using var archive = new ZipArchive(stream, ZipArchiveMode.Read);

        Assert.Contains(archive.Entries, e => e.Name == "umbraco.yml");
    }
}
