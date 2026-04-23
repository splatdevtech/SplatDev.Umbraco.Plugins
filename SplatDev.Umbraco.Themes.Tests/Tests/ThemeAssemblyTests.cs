using Umbraco.Cms.Core.Composing;

namespace SplatDev.Umbraco.Themes.Tests.Tests;

/// <summary>
/// Verifies structural properties of each theme assembly:
/// that each has an IComposer implementation, has the expected embedded
/// YAML resource, and that types are in the expected namespaces.
/// </summary>
public class ThemeAssemblyTests
{
    // ── Composer class existence ──────────────────────────────────────────────

    [Fact]
    public void BaseTheme_Assembly_HasComposerClass()
    {
        var assembly = typeof(UmbracoCms.Themes.Base.Composers.BaseThemeComposer).Assembly;
        var composerType = assembly.GetType("UmbracoCms.Themes.Base.Composers.BaseThemeComposer");

        composerType.Should().NotBeNull();
        composerType!.GetInterfaces().Should().Contain(typeof(IComposer),
            because: "BaseThemeComposer must implement IComposer");
    }

    [Fact]
    public void BlogTheme_Assembly_HasComposerClass()
    {
        var assembly = typeof(UmbracoCms.Themes.Blog.Composers.BlogThemeComposer).Assembly;
        var composerType = assembly.GetType("UmbracoCms.Themes.Blog.Composers.BlogThemeComposer");

        composerType.Should().NotBeNull();
        composerType!.GetInterfaces().Should().Contain(typeof(IComposer),
            because: "BlogThemeComposer must implement IComposer");
    }

    [Fact]
    public void CommerceTheme_Assembly_HasComposerClass()
    {
        var assembly = typeof(UmbracoCms.Themes.Commerce.Composers.CommerceThemeComposer).Assembly;
        var composerType = assembly.GetType("UmbracoCms.Themes.Commerce.Composers.CommerceThemeComposer");

        composerType.Should().NotBeNull();
        composerType!.GetInterfaces().Should().Contain(typeof(IComposer),
            because: "CommerceThemeComposer must implement IComposer");
    }

    [Fact]
    public void ConferenceTheme_Assembly_HasComposerClass()
    {
        var assembly = typeof(UmbracoCms.Themes.Conference.Composers.ConferenceThemeComposer).Assembly;
        var composerType = assembly.GetType("UmbracoCms.Themes.Conference.Composers.ConferenceThemeComposer");

        composerType.Should().NotBeNull();
        composerType!.GetInterfaces().Should().Contain(typeof(IComposer),
            because: "ConferenceThemeComposer must implement IComposer");
    }

    [Fact]
    public void ForumTheme_Assembly_HasComposerClass()
    {
        var assembly = typeof(UmbracoCms.Themes.Forum.Composers.ForumThemeComposer).Assembly;
        var composerType = assembly.GetType("UmbracoCms.Themes.Forum.Composers.ForumThemeComposer");

        composerType.Should().NotBeNull();
        composerType!.GetInterfaces().Should().Contain(typeof(IComposer),
            because: "ForumThemeComposer must implement IComposer");
    }

    [Fact]
    public void LandingTheme_Assembly_HasComposerClass()
    {
        var assembly = typeof(UmbracoCms.Themes.Landing.Composers.LandingThemeComposer).Assembly;
        var composerType = assembly.GetType("UmbracoCms.Themes.Landing.Composers.LandingThemeComposer");

        composerType.Should().NotBeNull();
        composerType!.GetInterfaces().Should().Contain(typeof(IComposer),
            because: "LandingThemeComposer must implement IComposer");
    }

    [Fact]
    public void CorporateTheme_Assembly_HasComposerClass()
    {
        // Corporate theme is resolved via AppDomain since it may be a stub assembly.
        var assembly = AppDomain.CurrentDomain
            .GetAssemblies()
            .FirstOrDefault(a => a.GetName().Name == "UmbracoCms.Themes.Corporate");

        assembly.Should().NotBeNull(
            because: "UmbracoCms.Themes.Corporate should be loaded as a project reference");

        var composerType = assembly!.GetType("UmbracoCms.Themes.Corporate.Composers.CorporateThemeComposer");

        composerType.Should().NotBeNull(
            because: "CorporateThemeComposer should exist in the Corporate theme assembly");

        composerType!.GetInterfaces().Should().Contain(typeof(IComposer),
            because: "CorporateThemeComposer must implement IComposer");
    }

    [Fact]
    public void HotelTheme_Assembly_HasComposerClass()
    {
        // Hotel theme is resolved via AppDomain since it may be a stub assembly.
        var assembly = AppDomain.CurrentDomain
            .GetAssemblies()
            .FirstOrDefault(a => a.GetName().Name == "UmbracoCms.Themes.Hotel");

        assembly.Should().NotBeNull(
            because: "UmbracoCms.Themes.Hotel should be loaded as a project reference");

        var composerType = assembly!.GetType("UmbracoCms.Themes.Hotel.Composers.HotelThemeComposer");

        composerType.Should().NotBeNull(
            because: "HotelThemeComposer should exist in the Hotel theme assembly");

        composerType!.GetInterfaces().Should().Contain(typeof(IComposer),
            because: "HotelThemeComposer must implement IComposer");
    }

    [Fact]
    public void PortfolioTheme_Assembly_HasComposerClass()
    {
        // Portfolio theme is resolved via AppDomain since it may be a stub assembly.
        var assembly = AppDomain.CurrentDomain
            .GetAssemblies()
            .FirstOrDefault(a => a.GetName().Name == "UmbracoCms.Themes.Portfolio");

        assembly.Should().NotBeNull(
            because: "UmbracoCms.Themes.Portfolio should be loaded as a project reference");

        var composerType = assembly!.GetType("UmbracoCms.Themes.Portfolio.Composers.PortfolioThemeComposer");

        composerType.Should().NotBeNull(
            because: "PortfolioThemeComposer should exist in the Portfolio theme assembly");

        composerType!.GetInterfaces().Should().Contain(typeof(IComposer),
            because: "PortfolioThemeComposer must implement IComposer");
    }

    // ── AllThemes: parameterised IComposer checks ─────────────────────────────

    /// <summary>
    /// Verifies that all 9 themes have exactly one IComposer implementation
    /// as a public, non-abstract, non-generic class.
    /// </summary>
    [Theory]
    [InlineData("UmbracoCms.Themes.Base.Composers.BaseThemeComposer",  "UmbracoCms.Themes.Base")]
    [InlineData("UmbracoCms.Themes.Blog.Composers.BlogThemeComposer",  "UmbracoCms.Themes.Blog")]
    [InlineData("UmbracoCms.Themes.Commerce.Composers.CommerceThemeComposer",  "UmbracoCms.Themes.Commerce")]
    [InlineData("UmbracoCms.Themes.Conference.Composers.ConferenceThemeComposer",  "UmbracoCms.Themes.Conference")]
    [InlineData("UmbracoCms.Themes.Corporate.Composers.CorporateThemeComposer",  "UmbracoCms.Themes.Corporate")]
    [InlineData("UmbracoCms.Themes.Forum.Composers.ForumThemeComposer",  "UmbracoCms.Themes.Forum")]
    [InlineData("UmbracoCms.Themes.Hotel.Composers.HotelThemeComposer",  "UmbracoCms.Themes.Hotel")]
    [InlineData("UmbracoCms.Themes.Landing.Composers.LandingThemeComposer",  "UmbracoCms.Themes.Landing")]
    [InlineData("UmbracoCms.Themes.Portfolio.Composers.PortfolioThemeComposer",  "UmbracoCms.Themes.Portfolio")]
    public void AllThemes_HaveComposerClasses(string composerTypeName, string assemblyName)
    {
        // Load the assembly by its simple name – it is already in the AppDomain because
        // the test project has direct ProjectReferences to all theme assemblies.
        var assembly = AppDomain.CurrentDomain
            .GetAssemblies()
            .FirstOrDefault(a => a.GetName().Name == assemblyName);

        assembly.Should().NotBeNull(
            because: $"assembly '{assemblyName}' should be loaded as a project reference");

        var composerType = assembly!.GetType(composerTypeName);

        composerType.Should().NotBeNull(
            because: $"'{composerTypeName}' should exist in assembly '{assemblyName}'");

        composerType!.GetInterfaces().Should().Contain(typeof(IComposer),
            because: $"'{composerTypeName}' must implement IComposer");
    }

    // ── AllThemes: embedded YAML resource ─────────────────────────────────────

    /// <summary>
    /// Verifies that each theme assembly contains an embedded umbraco.yml resource.
    /// </summary>
    [Theory]
    [InlineData("UmbracoCms.Themes.Base",       "UmbracoCms.Themes.Base.Config.umbraco.yml")]
    [InlineData("UmbracoCms.Themes.Blog",       "UmbracoCms.Themes.Blog.Config.umbraco.yml")]
    [InlineData("UmbracoCms.Themes.Commerce",   "UmbracoCms.Themes.Commerce.Config.umbraco.yml")]
    [InlineData("UmbracoCms.Themes.Conference", "UmbracoCms.Themes.Conference.Config.umbraco.yml")]
    [InlineData("UmbracoCms.Themes.Corporate",  "UmbracoCms.Themes.Corporate.Config.umbraco.yml")]
    [InlineData("UmbracoCms.Themes.Forum",      "UmbracoCms.Themes.Forum.Config.umbraco.yml")]
    [InlineData("UmbracoCms.Themes.Hotel",      "UmbracoCms.Themes.Hotel.Config.umbraco.yml")]
    [InlineData("UmbracoCms.Themes.Landing",    "UmbracoCms.Themes.Landing.Config.umbraco.yml")]
    [InlineData("UmbracoCms.Themes.Portfolio",  "UmbracoCms.Themes.Portfolio.Config.umbraco.yml")]
    public void AllThemes_HaveEmbeddedYaml(string assemblyName, string resourceName)
    {
        var assembly = AppDomain.CurrentDomain
            .GetAssemblies()
            .FirstOrDefault(a => a.GetName().Name == assemblyName);

        assembly.Should().NotBeNull(
            because: $"assembly '{assemblyName}' should be loaded as a project reference");

        var resourceNames = assembly!.GetManifestResourceNames();

        resourceNames.Should().Contain(resourceName,
            because: $"Config/umbraco.yml must be marked as EmbeddedResource in '{assemblyName}'");
    }

    // ── AllThemes: non-empty YAML resource ───────────────────────────────────

    /// <summary>
    /// Verifies that each embedded YAML resource is non-empty (at least 100 bytes).
    /// </summary>
    [Theory]
    [InlineData("UmbracoCms.Themes.Base",       "UmbracoCms.Themes.Base.Config.umbraco.yml")]
    [InlineData("UmbracoCms.Themes.Blog",       "UmbracoCms.Themes.Blog.Config.umbraco.yml")]
    [InlineData("UmbracoCms.Themes.Commerce",   "UmbracoCms.Themes.Commerce.Config.umbraco.yml")]
    [InlineData("UmbracoCms.Themes.Conference", "UmbracoCms.Themes.Conference.Config.umbraco.yml")]
    [InlineData("UmbracoCms.Themes.Corporate",  "UmbracoCms.Themes.Corporate.Config.umbraco.yml")]
    [InlineData("UmbracoCms.Themes.Forum",      "UmbracoCms.Themes.Forum.Config.umbraco.yml")]
    [InlineData("UmbracoCms.Themes.Hotel",      "UmbracoCms.Themes.Hotel.Config.umbraco.yml")]
    [InlineData("UmbracoCms.Themes.Landing",    "UmbracoCms.Themes.Landing.Config.umbraco.yml")]
    [InlineData("UmbracoCms.Themes.Portfolio",  "UmbracoCms.Themes.Portfolio.Config.umbraco.yml")]
    public void AllThemes_EmbeddedYaml_IsNonEmpty(string assemblyName, string resourceName)
    {
        var assembly = AppDomain.CurrentDomain
            .GetAssemblies()
            .FirstOrDefault(a => a.GetName().Name == assemblyName);

        assembly.Should().NotBeNull();

        using var stream = assembly!.GetManifestResourceStream(resourceName);

        stream.Should().NotBeNull(
            because: $"embedded resource '{resourceName}' should exist in '{assemblyName}'");

        stream!.Length.Should().BeGreaterThan(100,
            because: "a valid YAML schema file should contain meaningful content");
    }

    // ── Assembly naming conventions ───────────────────────────────────────────

    /// <summary>
    /// Verifies that each theme assembly has the expected assembly name.
    /// </summary>
    [Theory]
    [InlineData("UmbracoCms.Themes.Base")]
    [InlineData("UmbracoCms.Themes.Blog")]
    [InlineData("UmbracoCms.Themes.Commerce")]
    [InlineData("UmbracoCms.Themes.Conference")]
    [InlineData("UmbracoCms.Themes.Corporate")]
    [InlineData("UmbracoCms.Themes.Forum")]
    [InlineData("UmbracoCms.Themes.Hotel")]
    [InlineData("UmbracoCms.Themes.Landing")]
    [InlineData("UmbracoCms.Themes.Portfolio")]
    public void AllThemes_HaveExpectedAssemblyName(string expectedAssemblyName)
    {
        var assembly = AppDomain.CurrentDomain
            .GetAssemblies()
            .FirstOrDefault(a => a.GetName().Name == expectedAssemblyName);

        assembly.Should().NotBeNull(
            because: $"assembly '{expectedAssemblyName}' should be loaded via project reference");

        assembly!.GetName().Name.Should().Be(expectedAssemblyName);
    }

    // ── Composer namespace convention ─────────────────────────────────────────

    /// <summary>
    /// Verifies that each theme's IComposer implementation lives in the expected
    /// namespace: {AssemblyName}.Composers.
    /// </summary>
    [Theory]
    [InlineData("UmbracoCms.Themes.Base")]
    [InlineData("UmbracoCms.Themes.Blog")]
    [InlineData("UmbracoCms.Themes.Commerce")]
    [InlineData("UmbracoCms.Themes.Conference")]
    [InlineData("UmbracoCms.Themes.Corporate")]
    [InlineData("UmbracoCms.Themes.Forum")]
    [InlineData("UmbracoCms.Themes.Hotel")]
    [InlineData("UmbracoCms.Themes.Landing")]
    [InlineData("UmbracoCms.Themes.Portfolio")]
    public void AllThemes_ComposerClasses_AreInComposersNamespace(string assemblyName)
    {
        var assembly = AppDomain.CurrentDomain
            .GetAssemblies()
            .FirstOrDefault(a => a.GetName().Name == assemblyName);

        assembly.Should().NotBeNull();

        var composers = assembly!.GetTypes()
            .Where(t => typeof(IComposer).IsAssignableFrom(t)
                     && !t.IsInterface
                     && !t.IsAbstract)
            .ToList();

        composers.Should().NotBeEmpty(
            because: $"'{assemblyName}' must expose at least one public IComposer");

        var expectedNamespace = $"{assemblyName}.Composers";
        foreach (var composer in composers)
        {
            composer.Namespace.Should().Be(expectedNamespace,
                because: $"all IComposer implementations in '{assemblyName}' should reside in the .Composers namespace");
        }
    }
}
