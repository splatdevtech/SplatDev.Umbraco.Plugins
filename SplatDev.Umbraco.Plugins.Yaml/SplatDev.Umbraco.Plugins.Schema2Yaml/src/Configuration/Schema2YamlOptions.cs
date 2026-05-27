namespace SplatDev.Umbraco.Plugins.Schema2Yaml.Configuration;

/// <summary>
/// Configuration options for Schema2Yaml export plugin.
/// </summary>
public class Schema2YamlOptions
{
    /// <summary>
    /// Configuration section name in appsettings.json.
    /// </summary>
    public const string SectionName = "UmbracoSchema2Yaml";

    /// <summary>
    /// Default YAML export file path (relative to content root).
    /// </summary>
    public string ExportPath { get; set; } = "exports/umbraco.yaml";

    /// <summary>
    /// Include media items in export.
    /// </summary>
    public bool IncludeMedia { get; set; } = true;

    /// <summary>
    /// Media files download location (relative to content root).
    /// </summary>
    public string MediaPath { get; set; } = "exports/media";

    /// <summary>
    /// Include content nodes in export.
    /// </summary>
    public bool IncludeContent { get; set; } = true;

    /// <summary>
    /// Include back-office users in export (security consideration).
    /// </summary>
    public bool IncludeUsers { get; set; } = false;

    /// <summary>
    /// Include members in export.
    /// </summary>
    public bool IncludeMembers { get; set; } = true;

    /// <summary>
    /// Include dictionary items in export.
    /// </summary>
    public bool IncludeDictionary { get; set; } = true;

    /// <summary>
    /// Include languages in export.
    /// </summary>
    public bool IncludeLanguages { get; set; } = true;

    /// <summary>
    /// Minimize YAML output (removes comments and extra whitespace).
    /// </summary>
    public bool CompressYaml { get; set; } = false;

    /// <summary>
    /// Maximum depth for content/media hierarchies (prevents circular references).
    /// </summary>
    public int MaxHierarchyDepth { get; set; } = 50;
}
