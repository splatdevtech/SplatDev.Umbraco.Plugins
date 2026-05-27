using Microsoft.Extensions.Logging;
using Moq;
using SplatDev.Umbraco.Plugins.Schema2Yaml.Services;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Services;

namespace SplatDev.Umbraco.Plugins.Schema2Yaml.Tests.Services;

public class DictionaryExporterTests
{
    private readonly Mock<IDictionaryItemService> _mockDictionaryItemService;
    private readonly Mock<ILogger<DictionaryExporter>> _mockLogger;
    private readonly DictionaryExporter _sut;

    public DictionaryExporterTests()
    {
        _mockDictionaryItemService = new Mock<IDictionaryItemService>();
        _mockLogger = new Mock<ILogger<DictionaryExporter>>();

        _sut = new DictionaryExporter(_mockDictionaryItemService.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task ExportAsync_WhenNoRootItems_ReturnsEmptyList()
    {
        _mockDictionaryItemService.Setup(s => s.GetAtRootAsync())
            .ReturnsAsync(Enumerable.Empty<IDictionaryItem>());

        var result = await _sut.ExportAsync();

        Assert.Empty(result);
    }

    [Fact]
    public async Task ExportAsync_ExportsRootItemWithTranslations()
    {
        var mockTranslation = new Mock<IDictionaryTranslation>();
        mockTranslation.Setup(t => t.LanguageIsoCode).Returns("en-US");
        mockTranslation.Setup(t => t.Value).Returns("Welcome");

        var itemKey = Guid.NewGuid();
        var mockItem = new Mock<IDictionaryItem>();
        mockItem.Setup(i => i.ItemKey).Returns("general.welcome");
        mockItem.Setup(i => i.Key).Returns(itemKey);
        mockItem.Setup(i => i.Translations).Returns([mockTranslation.Object]);

        _mockDictionaryItemService.Setup(s => s.GetAtRootAsync())
            .ReturnsAsync(new[] { mockItem.Object });
        _mockDictionaryItemService.Setup(s => s.GetChildrenAsync(It.IsAny<Guid>()))
            .ReturnsAsync(Enumerable.Empty<IDictionaryItem>());

        var result = await _sut.ExportAsync();

        Assert.Single(result);
        Assert.Equal("general.welcome", result[0].Key);
        Assert.True(result[0].Translations.ContainsKey("en-US"));
        Assert.Equal("Welcome", result[0].Translations["en-US"]);
    }

    [Fact]
    public async Task ExportAsync_ExportsChildItems_FlattenedIntoList()
    {
        var parentKey = Guid.NewGuid();

        var mockParent = new Mock<IDictionaryItem>();
        mockParent.Setup(i => i.ItemKey).Returns("nav");
        mockParent.Setup(i => i.Key).Returns(parentKey);
        mockParent.Setup(i => i.Translations).Returns([]);

        var mockChild = new Mock<IDictionaryItem>();
        mockChild.Setup(i => i.ItemKey).Returns("nav.home");
        mockChild.Setup(i => i.Key).Returns(Guid.NewGuid());
        mockChild.Setup(i => i.Translations).Returns([]);

        _mockDictionaryItemService.Setup(s => s.GetAtRootAsync())
            .ReturnsAsync(new[] { mockParent.Object });
        _mockDictionaryItemService.Setup(s => s.GetChildrenAsync(parentKey))
            .ReturnsAsync(new[] { mockChild.Object });
        _mockDictionaryItemService.Setup(s => s.GetChildrenAsync(It.Is<Guid>(g => g != parentKey)))
            .ReturnsAsync(Enumerable.Empty<IDictionaryItem>());

        var result = await _sut.ExportAsync();

        Assert.Equal(2, result.Count);
        Assert.Equal("nav", result[0].Key);
        Assert.Equal("nav.home", result[1].Key);
    }

    [Fact]
    public async Task ExportAsync_SkipsTranslation_WhenLanguageIsNull()
    {
        var mockTranslation = new Mock<IDictionaryTranslation>();
        mockTranslation.Setup(t => t.LanguageIsoCode).Returns(string.Empty);
        mockTranslation.Setup(t => t.Value).Returns("Welcome");

        var mockItem = new Mock<IDictionaryItem>();
        mockItem.Setup(i => i.ItemKey).Returns("general.welcome");
        mockItem.Setup(i => i.Key).Returns(Guid.NewGuid());
        mockItem.Setup(i => i.Translations).Returns([mockTranslation.Object]);

        _mockDictionaryItemService.Setup(s => s.GetAtRootAsync())
            .ReturnsAsync(new[] { mockItem.Object });
        _mockDictionaryItemService.Setup(s => s.GetChildrenAsync(It.IsAny<Guid>()))
            .ReturnsAsync(Enumerable.Empty<IDictionaryItem>());

        var result = await _sut.ExportAsync();

        Assert.Single(result);
        Assert.Empty(result[0].Translations);
    }
}
