using Microsoft.Extensions.Logging;
using Moq;
using SplatDev.Umbraco.Plugins.Schema2Yaml.Services;
using Umbraco.Cms.Core.Configuration;
using UmbracoVersion = SplatDev.Umbraco.Plugins.Schema2Yaml.Services.UmbracoVersion;

namespace SplatDev.Umbraco.Plugins.Schema2Yaml.Tests.Services;

public class UmbracoVersionDetectorTests
{
    private static UmbracoVersionDetector CreateDetector(int major, int minor = 0, int patch = 0)
    {
        var mockVersion = new Mock<IUmbracoVersion>();
        mockVersion.Setup(v => v.Version).Returns(new Version(major, minor, patch));
        var mockLogger = new Mock<ILogger<UmbracoVersionDetector>>();
        return new UmbracoVersionDetector(mockVersion.Object, mockLogger.Object);
    }

    // ── GetVersion ──────────────────────────────────────────────────────────

    [Theory]
    [InlineData(14, UmbracoVersion.V14)]
    [InlineData(15, UmbracoVersion.V15)]
    [InlineData(16, UmbracoVersion.V16)]
    [InlineData(17, UmbracoVersion.V17)]
    public void GetVersion_ReturnsCorrectEnum_ForKnownMajors(int major, UmbracoVersion expected)
    {
        var sut = CreateDetector(major);

        var result = sut.GetVersion();

        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(12)]
    [InlineData(13)]
    [InlineData(18)]
    [InlineData(99)]
    public void GetVersion_ReturnsUnknown_ForUnsupportedMajors(int major)
    {
        var sut = CreateDetector(major);

        var result = sut.GetVersion();

        Assert.Equal(UmbracoVersion.Unknown, result);
    }

    [Fact]
    public void GetVersion_ReturnsCachedResult_OnSecondCall()
    {
        var mockVersion = new Mock<IUmbracoVersion>();
        mockVersion.Setup(v => v.Version).Returns(new Version(17, 0, 0));
        var mockLogger = new Mock<ILogger<UmbracoVersionDetector>>();
        var sut = new UmbracoVersionDetector(mockVersion.Object, mockLogger.Object);

        sut.GetVersion();
        sut.GetVersion();

        // Version property should be read exactly once due to caching
        mockVersion.Verify(v => v.Version, Times.Once);
    }

    // ── GetVersionString ─────────────────────────────────────────────────────

    [Fact]
    public void GetVersionString_ReturnsVersionAsDotSeparatedString()
    {
        var sut = CreateDetector(17, 2, 1);

        var result = sut.GetVersionString();

        Assert.Equal("17.2.1", result);
    }

    // ── SupportsEditorUiAlias ────────────────────────────────────────────────

    [Theory]
    [InlineData(14, true)]
    [InlineData(15, true)]
    [InlineData(16, true)]
    [InlineData(17, true)]
    public void SupportsEditorUiAlias_ReturnsTrue_ForV14AndLater(int major, bool expected)
    {
        var sut = CreateDetector(major);

        Assert.Equal(expected, sut.SupportsEditorUiAlias());
    }

    [Theory]
    [InlineData(12)]
    [InlineData(13)]
    [InlineData(18)]
    public void SupportsEditorUiAlias_ReturnsTrue_ForAllVersions_BecausePackageTargets14Plus(int major)
    {
        var sut = CreateDetector(major);

        Assert.True(sut.SupportsEditorUiAlias());
    }

    // ── UsesLegacyEditorAlias ────────────────────────────────────────────────

    [Fact]
    public void UsesLegacyEditorAlias_ReturnsFalse_ForAllVersions_BecausePackageTargets14Plus()
    {
        var sut = CreateDetector(13);

        Assert.False(sut.UsesLegacyEditorAlias());
    }

    [Theory]
    [InlineData(14)]
    [InlineData(15)]
    [InlineData(16)]
    [InlineData(17)]
    public void UsesLegacyEditorAlias_ReturnsFalse_ForV14AndLater(int major)
    {
        var sut = CreateDetector(major);

        Assert.False(sut.UsesLegacyEditorAlias());
    }

    // ── Constructor guard ────────────────────────────────────────────────────

    [Fact]
    public void Constructor_ThrowsArgumentNullException_WhenUmbracoVersionIsNull()
    {
        var mockLogger = new Mock<ILogger<UmbracoVersionDetector>>();

        Assert.Throws<ArgumentNullException>(() =>
            new UmbracoVersionDetector(null!, mockLogger.Object));
    }

    [Fact]
    public void Constructor_ThrowsArgumentNullException_WhenLoggerIsNull()
    {
        var mockVersion = new Mock<IUmbracoVersion>();
        mockVersion.Setup(v => v.Version).Returns(new Version(17, 0, 0));

        Assert.Throws<ArgumentNullException>(() =>
            new UmbracoVersionDetector(mockVersion.Object, null!));
    }
}
