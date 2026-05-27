using Microsoft.Extensions.Logging;
using Moq;
using SplatDev.Umbraco.Plugins.Schema2Yaml.Services;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Services;

namespace SplatDev.Umbraco.Plugins.Schema2Yaml.Tests.Services;

public class LanguageExporterTests
{
    private readonly Mock<ILanguageService> _mockLanguageService;
    private readonly Mock<ILogger<LanguageExporter>> _mockLogger;
    private readonly LanguageExporter _sut;

    public LanguageExporterTests()
    {
        _mockLanguageService = new Mock<ILanguageService>();
        _mockLogger = new Mock<ILogger<LanguageExporter>>();

        _sut = new LanguageExporter(_mockLanguageService.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task ExportAsync_WhenNoLanguages_ReturnsEmptyList()
    {
        _mockLanguageService.Setup(s => s.GetAllAsync())
            .ReturnsAsync(Enumerable.Empty<ILanguage>());

        var result = await _sut.ExportAsync();

        Assert.Empty(result);
    }

    [Fact]
    public async Task ExportAsync_MapsIsoCodeAndCultureName()
    {
        var mockLanguage = new Mock<ILanguage>();
        mockLanguage.Setup(l => l.IsoCode).Returns("en-US");
        mockLanguage.Setup(l => l.CultureName).Returns("English (United States)");
        mockLanguage.Setup(l => l.IsDefault).Returns(true);
        mockLanguage.Setup(l => l.IsMandatory).Returns(false);

        _mockLanguageService.Setup(s => s.GetAllAsync())
            .ReturnsAsync(new[] { mockLanguage.Object });

        var result = await _sut.ExportAsync();

        Assert.Single(result);
        Assert.Equal("en-US", result[0].IsoCode);
        Assert.Equal("English (United States)", result[0].CultureName);
    }

    [Fact]
    public async Task ExportAsync_MapsDefaultAndMandatoryFlags()
    {
        var mockLanguage = new Mock<ILanguage>();
        mockLanguage.Setup(l => l.IsoCode).Returns("pt-BR");
        mockLanguage.Setup(l => l.CultureName).Returns("Portuguese (Brazil)");
        mockLanguage.Setup(l => l.IsDefault).Returns(false);
        mockLanguage.Setup(l => l.IsMandatory).Returns(true);

        _mockLanguageService.Setup(s => s.GetAllAsync())
            .ReturnsAsync(new[] { mockLanguage.Object });

        var result = await _sut.ExportAsync();

        Assert.Single(result);
        Assert.False(result[0].IsDefault);
        Assert.True(result[0].IsMandatory);
    }

    [Fact]
    public async Task ExportAsync_ExportsMultipleLanguages()
    {
        var mockEn = new Mock<ILanguage>();
        mockEn.Setup(l => l.IsoCode).Returns("en-US");
        mockEn.Setup(l => l.CultureName).Returns("English");
        mockEn.Setup(l => l.IsDefault).Returns(true);
        mockEn.Setup(l => l.IsMandatory).Returns(false);

        var mockPt = new Mock<ILanguage>();
        mockPt.Setup(l => l.IsoCode).Returns("pt-BR");
        mockPt.Setup(l => l.CultureName).Returns("Portuguese");
        mockPt.Setup(l => l.IsDefault).Returns(false);
        mockPt.Setup(l => l.IsMandatory).Returns(false);

        _mockLanguageService.Setup(s => s.GetAllAsync())
            .ReturnsAsync(new[] { mockEn.Object, mockPt.Object });

        var result = await _sut.ExportAsync();

        Assert.Equal(2, result.Count);
        Assert.Equal("en-US", result[0].IsoCode);
        Assert.Equal("pt-BR", result[1].IsoCode);
    }
}
