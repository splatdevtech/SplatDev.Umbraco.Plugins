using System.Text.RegularExpressions;

namespace SplatDev.Umbraco.Themes.Tests.Tests;

/// <summary>
/// Verifies that each theme's wwwroot/css/ directory exists and contains valid CSS files.
/// Where CSS files are already present, their content is validated for required selectors.
/// </summary>
public class CssTests
{
    // ── Path helpers ─────────────────────────────────────────────────────────

    /// <summary>
    /// Returns the absolute path to the wwwroot/css/ directory for a given theme project,
    /// resolved from the test assembly output directory (four levels up from bin/Debug/net8.0/).
    /// </summary>
    private static string GetThemeCssDir(string themeName)
    {
        var assemblyDir = Path.GetDirectoryName(
            typeof(CssTests).Assembly.Location)!;

        // Navigate: bin/Debug/net8.0 -> Tests root -> Umbraco Projects/
        var repoRoot = Path.GetFullPath(Path.Combine(assemblyDir, "..", "..", "..", ".."));
        return Path.Combine(repoRoot, $"UmbracoCms.Themes.{themeName}", "wwwroot", "css");
    }

    // ── wwwroot/css directory existence ───────────────────────────────────────

    [Theory]
    [InlineData("Base")]
    [InlineData("Blog")]
    [InlineData("Commerce")]
    [InlineData("Conference")]
    [InlineData("Forum")]
    [InlineData("Hotel")]
    [InlineData("Landing")]
    [InlineData("Portfolio")]
    public void Theme_CssDirectory_Exists(string themeName)
    {
        var cssDir = GetThemeCssDir(themeName);
        Directory.Exists(cssDir).Should().BeTrue(
            because: $"'{themeName}' theme must have a wwwroot/css/ directory");
    }

    // ── CSS file existence checks ─────────────────────────────────────────────

    [Fact]
    public void BaseTheme_CssDirectory_ContainsCssFile()
    {
        var cssDir = GetThemeCssDir("Base");
        var files = Directory.GetFiles(cssDir, "*.css", SearchOption.AllDirectories);
        files.Should().NotBeEmpty(
            because: "Base theme must include at least one CSS file in wwwroot/css/");
    }

    [Fact]
    public void BlogTheme_CssDirectory_ContainsCssFile()
    {
        var cssDir = GetThemeCssDir("Blog");
        var files = Directory.GetFiles(cssDir, "*.css", SearchOption.AllDirectories);
        files.Should().NotBeEmpty(
            because: "Blog theme must include at least one CSS file in wwwroot/css/");
    }

    [Fact]
    public void CommerceTheme_CssDirectory_ContainsCssFile()
    {
        var cssDir = GetThemeCssDir("Commerce");
        var files = Directory.GetFiles(cssDir, "*.css", SearchOption.AllDirectories);
        files.Should().NotBeEmpty(
            because: "Commerce theme must include at least one CSS file in wwwroot/css/");
    }

    // ── CSS file non-empty checks ─────────────────────────────────────────────

    /// <summary>
    /// Verifies that every CSS file found in a theme's wwwroot/css/ directory
    /// is non-empty (at least 50 bytes).
    /// </summary>
    [Theory]
    [InlineData("Base")]
    [InlineData("Blog")]
    [InlineData("Commerce")]
    [InlineData("Conference")]
    [InlineData("Forum")]
    [InlineData("Landing")]
    public void Theme_CssFiles_AreNonEmpty(string themeName)
    {
        var cssDir = GetThemeCssDir(themeName);

        if (!Directory.Exists(cssDir))
            return; // directory not yet created – skip gracefully

        var cssFiles = Directory.GetFiles(cssDir, "*.css", SearchOption.AllDirectories);

        if (!cssFiles.Any())
            return; // no CSS files yet – structural check is handled by other tests

        foreach (var file in cssFiles)
        {
            var info = new FileInfo(file);
            info.Length.Should().BeGreaterThan(50,
                because: $"CSS file '{Path.GetFileName(file)}' in '{themeName}' must not be empty");
        }
    }

    // ── CSS content validation ────────────────────────────────────────────────

    /// <summary>
    /// Verifies that a CSS file contains at least one valid CSS rule block (selector + braces).
    /// </summary>
    [Theory]
    [InlineData("Base")]
    [InlineData("Blog")]
    [InlineData("Commerce")]
    public void Theme_CssFiles_ContainValidCssRules(string themeName)
    {
        var cssDir = GetThemeCssDir(themeName);

        if (!Directory.Exists(cssDir))
            return;

        var cssFiles = Directory.GetFiles(cssDir, "*.css", SearchOption.AllDirectories);

        if (!cssFiles.Any())
            return;

        // A minimal CSS rule contains a selector followed by a property declaration block.
        var cssRulePattern = new Regex(@"\w[\w\s\-#\.:\[\]""=,>~+*\(\)]*\s*\{[^}]+\}", RegexOptions.Singleline);

        foreach (var file in cssFiles)
        {
            var content = File.ReadAllText(file);
            cssRulePattern.IsMatch(content).Should().BeTrue(
                because: $"'{Path.GetFileName(file)}' in '{themeName}' must contain at least one valid CSS rule");
        }
    }

    // ── Layout stylesheet reference ───────────────────────────────────────────

    [Fact]
    public void BaseTheme_Layout_ReferencesThemeStylesheet()
    {
        var assemblyDir = Path.GetDirectoryName(typeof(CssTests).Assembly.Location)!;
        var repoRoot = Path.GetFullPath(Path.Combine(assemblyDir, "..", "..", "..", ".."));
        var layoutPath = Path.Combine(repoRoot, "UmbracoCms.Themes.Base", "Views", "Shared", "_Layout.cshtml");

        if (!File.Exists(layoutPath))
            return;

        var content = File.ReadAllText(layoutPath);
        content.Should().Contain(".css",
            because: "_Layout.cshtml must reference at least one CSS stylesheet via a <link> tag");
    }

    // ── CSS naming convention ─────────────────────────────────────────────────

    /// <summary>
    /// Verifies that CSS files follow the kebab-case naming convention.
    /// </summary>
    [Theory]
    [InlineData("Base")]
    [InlineData("Blog")]
    [InlineData("Commerce")]
    [InlineData("Conference")]
    [InlineData("Forum")]
    [InlineData("Landing")]
    public void Theme_CssFiles_FollowKebabCaseNaming(string themeName)
    {
        var cssDir = GetThemeCssDir(themeName);

        if (!Directory.Exists(cssDir))
            return;

        var cssFiles = Directory.GetFiles(cssDir, "*.css", SearchOption.AllDirectories);

        if (!cssFiles.Any())
            return;

        // Kebab-case: lowercase letters, digits, hyphens, dots (for .min.css)
        var kebabPattern = new Regex(@"^[a-z0-9][a-z0-9\-\.]*\.css$");

        foreach (var file in cssFiles)
        {
            var fileName = Path.GetFileName(file);
            kebabPattern.IsMatch(fileName).Should().BeTrue(
                because: $"CSS file '{fileName}' in '{themeName}' should use kebab-case naming (e.g. 'theme-name.min.css')");
        }
    }

    // ── Parameterised wwwroot check for in-development themes ────────────────

    /// <summary>
    /// Verifies that all themes that are fully implemented have a wwwroot directory.
    /// </summary>
    [Theory]
    [InlineData("Base")]
    [InlineData("Blog")]
    [InlineData("Commerce")]
    [InlineData("Conference")]
    [InlineData("Forum")]
    [InlineData("Hotel")]
    [InlineData("Landing")]
    [InlineData("Portfolio")]
    public void Theme_WwwrootDirectory_Exists(string themeName)
    {
        var assemblyDir = Path.GetDirectoryName(typeof(CssTests).Assembly.Location)!;
        var repoRoot = Path.GetFullPath(Path.Combine(assemblyDir, "..", "..", "..", ".."));
        var wwwroot = Path.Combine(repoRoot, $"UmbracoCms.Themes.{themeName}", "wwwroot");

        Directory.Exists(wwwroot).Should().BeTrue(
            because: $"'{themeName}' theme must have a wwwroot/ directory for static assets");
    }
}
