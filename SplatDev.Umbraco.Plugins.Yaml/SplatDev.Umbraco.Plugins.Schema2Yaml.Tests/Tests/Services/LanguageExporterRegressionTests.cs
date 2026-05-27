using Microsoft.Extensions.Logging;
using Moq;
using SplatDev.Umbraco.Plugins.Schema2Yaml.Services;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Services;

namespace SplatDev.Umbraco.Plugins.Schema2Yaml.Tests.Services;

/// <summary>
/// Regression tests for LanguageExporter.
/// Language export has no TFM-specific compile guards; tests run identically on net8.0–net10.0.
/// </summary>
public class LanguageExporterRegressionTests
{
    private readonly Mock<ILanguageService> _mockService = new();
    private readonly Mock<ILogger<LanguageExporter>> _mockLogger = new();
    private readonly LanguageExporter _sut;

    public LanguageExporterRegressionTests()
    {
        _sut = new LanguageExporter(_mockService.Object, _mockLogger.Object);
    }

    // ── Core field mapping regression ────────────────────────────────────────

    [Fact]
    public async Task ExportAsync_MapsAllFourFields()
    {
        var lang = BuildLanguage("en-US", "English (United States)", isDefault: true, isMandatory: true);
        _mockService.Setup(s => s.GetAllAsync()).ReturnsAsync(new[] { lang });

        var result = await _sut.ExportAsync();

        Assert.Single(result);
        Assert.Equal("en-US", result[0].IsoCode);
        Assert.Equal("English (United States)", result[0].CultureName);
        Assert.True(result[0].IsDefault);
        Assert.True(result[0].IsMandatory);
    }

    // ── Multi-language site regression ───────────────────────────────────────

    [Theory]
    [InlineData("en-US", "English", true, false)]
    [InlineData("pt-BR", "Portuguese (Brazil)", false, false)]
    [InlineData("fr-FR", "French (France)", false, true)]
    [InlineData("es-ES", "Spanish (Spain)", false, false)]
    [InlineData("de-DE", "German (Germany)", false, false)]
    public async Task ExportAsync_MapsEachLanguageCorrectly(
        string isoCode, string cultureName, bool isDefault, bool isMandatory)
    {
        var lang = BuildLanguage(isoCode, cultureName, isDefault, isMandatory);
        _mockService.Setup(s => s.GetAllAsync()).ReturnsAsync(new[] { lang });

        var result = await _sut.ExportAsync();

        Assert.Single(result);
        Assert.Equal(isoCode, result[0].IsoCode);
        Assert.Equal(cultureName, result[0].CultureName);
        Assert.Equal(isDefault, result[0].IsDefault);
        Assert.Equal(isMandatory, result[0].IsMandatory);
    }

    [Fact]
    public async Task ExportAsync_PreservesOrderOfLanguagesReturnedByService()
    {
        var languages = new[]
        {
            BuildLanguage("en-US", "English", isDefault: true, isMandatory: false),
            BuildLanguage("pt-BR", "Portuguese", isDefault: false, isMandatory: false),
            BuildLanguage("fr-FR", "French", isDefault: false, isMandatory: true)
        };
        _mockService.Setup(s => s.GetAllAsync()).ReturnsAsync(languages);

        var result = await _sut.ExportAsync();

        Assert.Equal(3, result.Count);
        Assert.Equal("en-US", result[0].IsoCode);
        Assert.Equal("pt-BR", result[1].IsoCode);
        Assert.Equal("fr-FR", result[2].IsoCode);
    }

    // ── Default / mandatory flag permutations ────────────────────────────────

    [Theory]
    [InlineData(true, true)]
    [InlineData(true, false)]
    [InlineData(false, true)]
    [InlineData(false, false)]
    public async Task ExportAsync_MapsDefaultAndMandatoryPermutations(bool isDefault, bool isMandatory)
    {
        var lang = BuildLanguage("en-US", "English", isDefault, isMandatory);
        _mockService.Setup(s => s.GetAllAsync()).ReturnsAsync(new[] { lang });

        var result = await _sut.ExportAsync();

        Assert.Equal(isDefault, result[0].IsDefault);
        Assert.Equal(isMandatory, result[0].IsMandatory);
    }

    // ── Error resilience regression ───────────────────────────────────────────

    [Fact]
    public async Task ExportAsync_SkipsFailingLanguage_AndContinuesExport()
    {
        // Throw on CultureName (inside the try block); IsoCode is used by the catch logger so
        // throwing there would cause the exception to escape the catch.
        var broken = new Mock<ILanguage>();
        broken.Setup(l => l.IsoCode).Returns("xx-XX");
        broken.Setup(l => l.CultureName).Throws(new InvalidOperationException("Service failure"));

        var good = BuildLanguage("en-US", "English", isDefault: true, isMandatory: false);
        _mockService.Setup(s => s.GetAllAsync()).ReturnsAsync(new ILanguage[] { broken.Object, good });

        var result = await _sut.ExportAsync();

        Assert.Single(result);
        Assert.Equal("en-US", result[0].IsoCode);
    }

    [Fact]
    public async Task ExportAsync_ReturnsEmptyList_WhenNoLanguagesExist()
    {
        _mockService.Setup(s => s.GetAllAsync()).ReturnsAsync(Enumerable.Empty<ILanguage>());

        var result = await _sut.ExportAsync();

        Assert.Empty(result);
    }

    // ── Constructor null guard regression ────────────────────────────────────

    [Fact]
    public void Constructor_ThrowsArgumentNullException_WhenLocalizationServiceIsNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new LanguageExporter(null!, _mockLogger.Object));
    }

    [Fact]
    public void Constructor_ThrowsArgumentNullException_WhenLoggerIsNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new LanguageExporter(_mockService.Object, null!));
    }

    private static ILanguage BuildLanguage(string isoCode, string cultureName, bool isDefault, bool isMandatory)
    {
        var mock = new Mock<ILanguage>();
        mock.Setup(l => l.IsoCode).Returns(isoCode);
        mock.Setup(l => l.CultureName).Returns(cultureName);
        mock.Setup(l => l.IsDefault).Returns(isDefault);
        mock.Setup(l => l.IsMandatory).Returns(isMandatory);
        return mock.Object;
    }
}
