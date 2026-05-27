using YamlDotNet.Serialization;

namespace SplatDev.Umbraco.Plugins.Schema2Yaml.Models;

/// <summary>
/// Root export model containing all Umbraco structure.
/// </summary>
public class ExportRoot
{
    [YamlMember(Alias = "umbraco")]
    public UmbracoExport Umbraco { get; set; } = new();
}

/// <summary>
/// Main export container for all Umbraco entities.
/// </summary>
public class UmbracoExport
{
    [YamlMember(Alias = "languages")]
    public List<ExportLanguage> Languages { get; set; } = [];

    [YamlMember(Alias = "dataTypes")]
    public List<ExportDataType> DataTypes { get; set; } = [];

    [YamlMember(Alias = "documentTypes")]
    public List<ExportDocumentType> DocumentTypes { get; set; } = [];

    [YamlMember(Alias = "mediaTypes")]
    public List<ExportMediaType> MediaTypes { get; set; } = [];

    [YamlMember(Alias = "templates")]
    public List<ExportTemplate> Templates { get; set; } = [];

    [YamlMember(Alias = "media")]
    public List<ExportMedia> Media { get; set; } = [];

    [YamlMember(Alias = "content")]
    public List<ExportContent> Content { get; set; } = [];

    [YamlMember(Alias = "dictionaryItems")]
    public List<ExportDictionaryItem> DictionaryItems { get; set; } = [];

    [YamlMember(Alias = "members")]
    public List<ExportMember> Members { get; set; } = [];

    [YamlMember(Alias = "users")]
    public List<ExportUser> Users { get; set; } = [];
}

/// <summary>
/// Export model for DataTypes (property editors).
/// </summary>
public class ExportDataType
{
    [YamlMember(Alias = "alias")]
    public string Alias { get; set; } = string.Empty;

    [YamlMember(Alias = "name")]
    public string Name { get; set; } = string.Empty;

    [YamlMember(Alias = "editorUiAlias")]
    public string EditorUiAlias { get; set; } = string.Empty;

    [YamlMember(Alias = "config")]
    public Dictionary<string, object> Config { get; set; } = new();

    [YamlMember(Alias = "valueType")]
    public string? ValueType { get; set; }
}

/// <summary>
/// Export model for DocumentTypes.
/// </summary>
public class ExportDocumentType
{
    [YamlMember(Alias = "alias")]
    public string Alias { get; set; } = string.Empty;

    [YamlMember(Alias = "name")]
    public string Name { get; set; } = string.Empty;

    [YamlMember(Alias = "icon")]
    public string? Icon { get; set; }

    [YamlMember(Alias = "isElement")]
    public bool IsElement { get; set; }

    [YamlMember(Alias = "allowAsRoot")]
    public bool AllowAsRoot { get; set; }

    [YamlMember(Alias = "allowedChildTypes")]
    public List<string> AllowedChildTypes { get; set; } = [];

    [YamlMember(Alias = "compositions")]
    public List<string> Compositions { get; set; } = [];

    [YamlMember(Alias = "allowedTemplates")]
    public List<string> AllowedTemplates { get; set; } = [];

    [YamlMember(Alias = "defaultTemplate")]
    public string? DefaultTemplate { get; set; }

    [YamlMember(Alias = "tabs")]
    public List<ExportTab> Tabs { get; set; } = [];
}

/// <summary>
/// Export model for property tabs.
/// </summary>
public class ExportTab
{
    [YamlMember(Alias = "name")]
    public string Name { get; set; } = string.Empty;

    [YamlMember(Alias = "sortOrder")]
    public int SortOrder { get; set; }

    [YamlMember(Alias = "properties")]
    public List<ExportProperty> Properties { get; set; } = [];
}

/// <summary>
/// Export model for properties.
/// </summary>
public class ExportProperty
{
    [YamlMember(Alias = "alias")]
    public string Alias { get; set; } = string.Empty;

    [YamlMember(Alias = "name")]
    public string Name { get; set; } = string.Empty;

    [YamlMember(Alias = "dataType")]
    public string DataType { get; set; } = string.Empty;

    [YamlMember(Alias = "required")]
    public bool Required { get; set; }

    [YamlMember(Alias = "description")]
    public string? Description { get; set; }

    [YamlMember(Alias = "sortOrder")]
    public int SortOrder { get; set; }
}

/// <summary>
/// Export model for MediaTypes.
/// </summary>
public class ExportMediaType
{
    [YamlMember(Alias = "alias")]
    public string Alias { get; set; } = string.Empty;

    [YamlMember(Alias = "name")]
    public string Name { get; set; } = string.Empty;

    [YamlMember(Alias = "icon")]
    public string? Icon { get; set; }

    [YamlMember(Alias = "allowedAtRoot")]
    public bool AllowedAtRoot { get; set; }

    [YamlMember(Alias = "tabs")]
    public List<ExportTab> Tabs { get; set; } = [];
}

/// <summary>
/// Export model for Templates.
/// </summary>
public class ExportTemplate
{
    [YamlMember(Alias = "alias")]
    public string Alias { get; set; } = string.Empty;

    [YamlMember(Alias = "name")]
    public string Name { get; set; } = string.Empty;

    [YamlMember(Alias = "masterTemplate")]
    public string? MasterTemplate { get; set; }

    [YamlMember(Alias = "content")]
    public string? Content { get; set; }
}

/// <summary>
/// Export model for Content nodes.
/// </summary>
public class ExportContent
{
    [YamlMember(Alias = "name")]
    public string Name { get; set; } = string.Empty;

    [YamlMember(Alias = "documentType")]
    public string DocumentType { get; set; } = string.Empty;

    [YamlMember(Alias = "template")]
    public string? Template { get; set; }

    [YamlMember(Alias = "sortOrder")]
    public int SortOrder { get; set; }

    [YamlMember(Alias = "isPublished")]
    public bool IsPublished { get; set; }

    [YamlMember(Alias = "properties")]
    public Dictionary<string, object> Properties { get; set; } = new();

    [YamlMember(Alias = "children")]
    public List<ExportContent> Children { get; set; } = [];
}

/// <summary>
/// Export model for Media items.
/// </summary>
public class ExportMedia
{
    [YamlMember(Alias = "name")]
    public string Name { get; set; } = string.Empty;

    [YamlMember(Alias = "mediaType")]
    public string MediaType { get; set; } = string.Empty;

    [YamlMember(Alias = "folder")]
    public string? Folder { get; set; }

    [YamlMember(Alias = "url")]
    public string? Url { get; set; }

    [YamlMember(Alias = "properties")]
    public Dictionary<string, object> Properties { get; set; } = new();

    [YamlMember(Alias = "children")]
    public List<ExportMedia> Children { get; set; } = [];
}

/// <summary>
/// Export model for Languages.
/// </summary>
public class ExportLanguage
{
    [YamlMember(Alias = "isoCode")]
    public string IsoCode { get; set; } = string.Empty;

    [YamlMember(Alias = "cultureName")]
    public string? CultureName { get; set; }

    [YamlMember(Alias = "isDefault")]
    public bool IsDefault { get; set; }

    [YamlMember(Alias = "isMandatory")]
    public bool IsMandatory { get; set; }
}

/// <summary>
/// Export model for Dictionary Items.
/// </summary>
public class ExportDictionaryItem
{
    [YamlMember(Alias = "key")]
    public string Key { get; set; } = string.Empty;

    [YamlMember(Alias = "translations")]
    public Dictionary<string, string> Translations { get; set; } = new();
}

/// <summary>
/// Export model for Members (passwords excluded for security).
/// </summary>
public class ExportMember
{
    [YamlMember(Alias = "name")]
    public string Name { get; set; } = string.Empty;

    [YamlMember(Alias = "email")]
    public string Email { get; set; } = string.Empty;

    [YamlMember(Alias = "username")]
    public string Username { get; set; } = string.Empty;

    [YamlMember(Alias = "memberType")]
    public string MemberType { get; set; } = "Member";

    [YamlMember(Alias = "isApproved")]
    public bool IsApproved { get; set; }

    [YamlMember(Alias = "properties")]
    public Dictionary<string, object> Properties { get; set; } = new();
}

/// <summary>
/// Export model for Users (passwords excluded for security).
/// </summary>
public class ExportUser
{
    [YamlMember(Alias = "name")]
    public string Name { get; set; } = string.Empty;

    [YamlMember(Alias = "email")]
    public string Email { get; set; } = string.Empty;

    [YamlMember(Alias = "username")]
    public string Username { get; set; } = string.Empty;

    [YamlMember(Alias = "userGroups")]
    public List<string> UserGroups { get; set; } = [];
}
