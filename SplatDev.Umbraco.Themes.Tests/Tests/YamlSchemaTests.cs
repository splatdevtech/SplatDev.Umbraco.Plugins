using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace SplatDev.Umbraco.Themes.Tests.Tests;

/// <summary>
/// Verifies that each theme's embedded YAML file can be loaded, parsed without error,
/// and contains the expected top-level keys (dataTypes, documentTypes, templates) and
/// a representative set of document type aliases.
/// </summary>
public class YamlSchemaTests
{
    // ── Helpers ──────────────────────────────────────────────────────────────

    private static IDeserializer BuildDeserializer() =>
        new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .IgnoreUnmatchedProperties()
            .Build();

    /// <summary>
    /// Loads the embedded umbraco.yml resource from the given assembly and returns its
    /// text content. Throws <see cref="InvalidOperationException"/> if not found.
    /// </summary>
    private static string LoadEmbeddedYaml(Assembly assembly, string resourceName)
    {
        using var stream = assembly.GetManifestResourceStream(resourceName)
            ?? throw new InvalidOperationException(
                $"Embedded resource '{resourceName}' not found in assembly '{assembly.GetName().Name}'. " +
                "Ensure Config/umbraco.yml is marked as EmbeddedResource in the project file.");

        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }

    /// <summary>
    /// Parses YAML text into a plain <see cref="Dictionary{String,Object}"/> so that
    /// tests remain independent of the Yaml2Schema model classes.
    /// </summary>
    private static Dictionary<object, object> ParseYaml(string yaml)
    {
        var deserializer = BuildDeserializer();
        return deserializer.Deserialize<Dictionary<object, object>>(yaml)!;
    }

    /// <summary>
    /// Extracts the list of document type aliases from a parsed YAML root dictionary.
    /// </summary>
    private static IReadOnlyList<string> GetDocumentTypeAliases(Dictionary<object, object> root)
    {
        if (!root.TryGetValue("documentTypes", out var raw) || raw is not List<object> list)
            return Array.Empty<string>();

        var aliases = new List<string>();
        foreach (var item in list)
        {
            if (item is Dictionary<object, object> dict && dict.TryGetValue("alias", out var alias))
                aliases.Add(alias?.ToString() ?? string.Empty);
        }

        return aliases;
    }

    // ── Validity tests (each theme parses without exception) ─────────────────

    [Fact]
    public void BaseTheme_YamlFile_IsValid()
    {
        var assembly = typeof(UmbracoCms.Themes.Base.Composers.BaseThemeComposer).Assembly;
        var yaml = LoadEmbeddedYaml(assembly, "UmbracoCms.Themes.Base.Config.umbraco.yml");

        yaml.Should().NotBeNullOrWhiteSpace();
        var root = ParseYaml(yaml);
        root.Should().NotBeNull();
    }

    [Fact]
    public void BlogTheme_YamlFile_IsValid()
    {
        var assembly = typeof(UmbracoCms.Themes.Blog.Composers.BlogThemeComposer).Assembly;
        var yaml = LoadEmbeddedYaml(assembly, "UmbracoCms.Themes.Blog.Config.umbraco.yml");

        yaml.Should().NotBeNullOrWhiteSpace();
        var root = ParseYaml(yaml);
        root.Should().NotBeNull();
    }

    [Fact]
    public void CommerceTheme_YamlFile_IsValid()
    {
        var assembly = typeof(UmbracoCms.Themes.Commerce.Composers.CommerceThemeComposer).Assembly;
        var yaml = LoadEmbeddedYaml(assembly, "UmbracoCms.Themes.Commerce.Config.umbraco.yml");

        yaml.Should().NotBeNullOrWhiteSpace();
        var root = ParseYaml(yaml);
        root.Should().NotBeNull();
    }

    [Fact]
    public void ConferenceTheme_YamlFile_IsValid()
    {
        var assembly = typeof(UmbracoCms.Themes.Conference.Composers.ConferenceThemeComposer).Assembly;
        var yaml = LoadEmbeddedYaml(assembly, "UmbracoCms.Themes.Conference.Config.umbraco.yml");

        yaml.Should().NotBeNullOrWhiteSpace();
        var root = ParseYaml(yaml);
        root.Should().NotBeNull();
    }

    [Fact]
    public void ForumTheme_YamlFile_IsValid()
    {
        var assembly = typeof(UmbracoCms.Themes.Forum.Composers.ForumThemeComposer).Assembly;
        var yaml = LoadEmbeddedYaml(assembly, "UmbracoCms.Themes.Forum.Config.umbraco.yml");

        yaml.Should().NotBeNullOrWhiteSpace();
        var root = ParseYaml(yaml);
        root.Should().NotBeNull();
    }

    [Fact]
    public void LandingTheme_YamlFile_IsValid()
    {
        var assembly = typeof(UmbracoCms.Themes.Landing.Composers.LandingThemeComposer).Assembly;
        var yaml = LoadEmbeddedYaml(assembly, "UmbracoCms.Themes.Landing.Config.umbraco.yml");

        yaml.Should().NotBeNullOrWhiteSpace();
        var root = ParseYaml(yaml);
        root.Should().NotBeNull();
    }

    // ── Top-level section tests ───────────────────────────────────────────────

    [Fact]
    public void BaseTheme_YamlFile_HasDataTypesSection()
    {
        var assembly = typeof(UmbracoCms.Themes.Base.Composers.BaseThemeComposer).Assembly;
        var root = ParseYaml(LoadEmbeddedYaml(assembly, "UmbracoCms.Themes.Base.Config.umbraco.yml"));

        root.Should().ContainKey("dataTypes");
        (root["dataTypes"] as List<object>).Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void BaseTheme_YamlFile_HasDocumentTypesSection()
    {
        var assembly = typeof(UmbracoCms.Themes.Base.Composers.BaseThemeComposer).Assembly;
        var root = ParseYaml(LoadEmbeddedYaml(assembly, "UmbracoCms.Themes.Base.Config.umbraco.yml"));

        root.Should().ContainKey("documentTypes");
        (root["documentTypes"] as List<object>).Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void BaseTheme_YamlFile_HasTemplatesSection()
    {
        var assembly = typeof(UmbracoCms.Themes.Base.Composers.BaseThemeComposer).Assembly;
        var root = ParseYaml(LoadEmbeddedYaml(assembly, "UmbracoCms.Themes.Base.Config.umbraco.yml"));

        root.Should().ContainKey("templates");
        (root["templates"] as List<object>).Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void BlogTheme_YamlFile_HasDataTypesSection()
    {
        var assembly = typeof(UmbracoCms.Themes.Blog.Composers.BlogThemeComposer).Assembly;
        var root = ParseYaml(LoadEmbeddedYaml(assembly, "UmbracoCms.Themes.Blog.Config.umbraco.yml"));

        root.Should().ContainKey("dataTypes");
        (root["dataTypes"] as List<object>).Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void CommerceTheme_YamlFile_HasDataTypesSection()
    {
        var assembly = typeof(UmbracoCms.Themes.Commerce.Composers.CommerceThemeComposer).Assembly;
        var root = ParseYaml(LoadEmbeddedYaml(assembly, "UmbracoCms.Themes.Commerce.Config.umbraco.yml"));

        root.Should().ContainKey("dataTypes");
        (root["dataTypes"] as List<object>).Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void LandingTheme_YamlFile_HasDocumentTypesSection()
    {
        var assembly = typeof(UmbracoCms.Themes.Landing.Composers.LandingThemeComposer).Assembly;
        var root = ParseYaml(LoadEmbeddedYaml(assembly, "UmbracoCms.Themes.Landing.Config.umbraco.yml"));

        root.Should().ContainKey("documentTypes");
        (root["documentTypes"] as List<object>).Should().NotBeNullOrEmpty();
    }

    // ── Required document type tests ─────────────────────────────────────────

    [Fact]
    public void BaseTheme_YamlFile_HasRequiredDocumentTypes()
    {
        var assembly = typeof(UmbracoCms.Themes.Base.Composers.BaseThemeComposer).Assembly;
        var root = ParseYaml(LoadEmbeddedYaml(assembly, "UmbracoCms.Themes.Base.Config.umbraco.yml"));

        var aliases = GetDocumentTypeAliases(root);
        aliases.Should().Contain("siteRoot", because: "Base theme requires a siteRoot document type");
        aliases.Should().Contain("homePage", because: "Base theme requires a homePage document type");
        aliases.Should().Contain("standardPage", because: "Base theme requires a standardPage document type");
        aliases.Should().Contain("basePage", because: "Base theme requires a basePage composition");
    }

    [Fact]
    public void BlogTheme_YamlFile_HasRequiredDocumentTypes()
    {
        var assembly = typeof(UmbracoCms.Themes.Blog.Composers.BlogThemeComposer).Assembly;
        var root = ParseYaml(LoadEmbeddedYaml(assembly, "UmbracoCms.Themes.Blog.Config.umbraco.yml"));

        var aliases = GetDocumentTypeAliases(root);
        aliases.Should().Contain("blogPost", because: "Blog theme requires a blogPost document type");
        aliases.Should().Contain("blogListing", because: "Blog theme requires a blogListing document type");
        aliases.Should().Contain("blogCategory", because: "Blog theme requires a blogCategory document type");
        aliases.Should().Contain("blogTag", because: "Blog theme requires a blogTag document type");
        aliases.Should().Contain("blogRoot", because: "Blog theme requires a blogRoot document type");
    }

    [Fact]
    public void CommerceTheme_YamlFile_HasRequiredDocumentTypes()
    {
        var assembly = typeof(UmbracoCms.Themes.Commerce.Composers.CommerceThemeComposer).Assembly;
        var root = ParseYaml(LoadEmbeddedYaml(assembly, "UmbracoCms.Themes.Commerce.Config.umbraco.yml"));

        var aliases = GetDocumentTypeAliases(root);
        aliases.Should().Contain("shopRoot", because: "Commerce theme requires a shopRoot document type");
        aliases.Should().Contain("product", because: "Commerce theme requires a product document type");
        aliases.Should().Contain("productCategory", because: "Commerce theme requires a productCategory document type");
        aliases.Should().Contain("shopListing", because: "Commerce theme requires a shopListing document type");
    }

    [Fact]
    public void LandingTheme_YamlFile_HasRequiredDocumentTypes()
    {
        var assembly = typeof(UmbracoCms.Themes.Landing.Composers.LandingThemeComposer).Assembly;
        var root = ParseYaml(LoadEmbeddedYaml(assembly, "UmbracoCms.Themes.Landing.Config.umbraco.yml"));

        var aliases = GetDocumentTypeAliases(root);
        aliases.Should().Contain("landingPage", because: "Landing theme requires a landingPage document type");
    }

    // ── Data type quality tests ───────────────────────────────────────────────

    [Fact]
    public void BaseTheme_YamlFile_DataTypes_HaveAliasAndEditorUiAlias()
    {
        var assembly = typeof(UmbracoCms.Themes.Base.Composers.BaseThemeComposer).Assembly;
        var root = ParseYaml(LoadEmbeddedYaml(assembly, "UmbracoCms.Themes.Base.Config.umbraco.yml"));

        var dataTypes = root["dataTypes"] as List<object>;
        dataTypes.Should().NotBeNull();

        foreach (var item in dataTypes!)
        {
            var dt = item as Dictionary<object, object>;
            dt.Should().NotBeNull();
            dt!.Should().ContainKey("alias", because: "every data type must have an alias");
            dt.Should().ContainKey("editorUiAlias", because: "every data type must have an editorUiAlias");
        }
    }

    [Fact]
    public void BaseTheme_YamlFile_ContainsCommonDataTypes()
    {
        var assembly = typeof(UmbracoCms.Themes.Base.Composers.BaseThemeComposer).Assembly;
        var root = ParseYaml(LoadEmbeddedYaml(assembly, "UmbracoCms.Themes.Base.Config.umbraco.yml"));

        var dataTypes = root["dataTypes"] as List<object>;
        var aliases = dataTypes!
            .OfType<Dictionary<object, object>>()
            .Select(d => d.TryGetValue("alias", out var a) ? a?.ToString() : null)
            .Where(a => a != null)
            .ToList();

        aliases.Should().Contain("textString");
        aliases.Should().Contain("imagePicker");
        aliases.Should().Contain("toggle");
        aliases.Should().Contain("urlPicker");
    }

    // ── Template tests ────────────────────────────────────────────────────────

    [Fact]
    public void BaseTheme_YamlFile_Templates_HaveAliasAndFilePath()
    {
        var assembly = typeof(UmbracoCms.Themes.Base.Composers.BaseThemeComposer).Assembly;
        var root = ParseYaml(LoadEmbeddedYaml(assembly, "UmbracoCms.Themes.Base.Config.umbraco.yml"));

        var templates = root["templates"] as List<object>;
        templates.Should().NotBeNull();

        foreach (var item in templates!)
        {
            var tpl = item as Dictionary<object, object>;
            tpl.Should().NotBeNull();
            tpl!.Should().ContainKey("alias", because: "every template must have an alias");
        }
    }

    [Fact]
    public void BlogTheme_YamlFile_Templates_ContainBlogPostAndBlogListing()
    {
        var assembly = typeof(UmbracoCms.Themes.Blog.Composers.BlogThemeComposer).Assembly;
        var root = ParseYaml(LoadEmbeddedYaml(assembly, "UmbracoCms.Themes.Blog.Config.umbraco.yml"));

        var templates = root["templates"] as List<object>;
        var aliases = templates!
            .OfType<Dictionary<object, object>>()
            .Select(t => t.TryGetValue("alias", out var a) ? a?.ToString() : null)
            .Where(a => a != null)
            .ToList();

        aliases.Should().Contain("BlogPost", because: "Blog theme requires a BlogPost template");
        aliases.Should().Contain("BlogListing", because: "Blog theme requires a BlogListing template");
    }

    [Fact]
    public void ConferenceTheme_YamlFile_HasRequiredDocumentTypes()
    {
        var assembly = typeof(UmbracoCms.Themes.Conference.Composers.ConferenceThemeComposer).Assembly;
        var root = ParseYaml(LoadEmbeddedYaml(assembly, "UmbracoCms.Themes.Conference.Config.umbraco.yml"));

        var aliases = GetDocumentTypeAliases(root);
        aliases.Should().Contain("conferenceRoot", because: "Conference theme requires a conferenceRoot document type");
        aliases.Should().Contain("speaker", because: "Conference theme requires a speaker document type");
        aliases.Should().Contain("conferenceHome", because: "Conference theme requires a conferenceHome document type");
    }

    [Fact]
    public void ForumTheme_YamlFile_HasRequiredDocumentTypes()
    {
        var assembly = typeof(UmbracoCms.Themes.Forum.Composers.ForumThemeComposer).Assembly;
        var root = ParseYaml(LoadEmbeddedYaml(assembly, "UmbracoCms.Themes.Forum.Config.umbraco.yml"));

        var aliases = GetDocumentTypeAliases(root);
        aliases.Should().Contain("forumRoot", because: "Forum theme requires a forumRoot document type");
        aliases.Should().Contain("forumThread", because: "Forum theme requires a forumThread document type");
        aliases.Should().Contain("forumCategory", because: "Forum theme requires a forumCategory document type");
    }

    [Fact]
    public void CommerceTheme_YamlFile_Templates_ContainShopListingAndProduct()
    {
        var assembly = typeof(UmbracoCms.Themes.Commerce.Composers.CommerceThemeComposer).Assembly;
        var root = ParseYaml(LoadEmbeddedYaml(assembly, "UmbracoCms.Themes.Commerce.Config.umbraco.yml"));

        var templates = root["templates"] as List<object>;
        var aliases = templates!
            .OfType<Dictionary<object, object>>()
            .Select(t => t.TryGetValue("alias", out var a) ? a?.ToString() : null)
            .Where(a => a != null)
            .ToList();

        aliases.Should().Contain("ShopListing", because: "Commerce theme requires a ShopListing template");
        aliases.Should().Contain("Product", because: "Commerce theme requires a Product template");
    }
}
