using System;
using System.Collections.Generic;
using YamlDotNet.Serialization;

namespace SplatDev.Umbraco.Plugins.Yaml2Schema.Models
{
    public class YamlRoot
    {
        [YamlMember(Alias = "umbraco")]
        public UmbracoConfig Umbraco { get; set; }
    }

    public class UmbracoConfig
    {
        [YamlMember(Alias = "dataTypes")]
        public List<YamlDataType> DataTypes { get; set; } = new();

        [YamlMember(Alias = "documentTypes")]
        public List<YamlDocumentType> DocumentTypes { get; set; } = new();

        [YamlMember(Alias = "mediaTypes")]
        public List<YamlMediaType> MediaTypes { get; set; } = new();

        [YamlMember(Alias = "memberTypes")]
        public List<YamlMemberType> MemberTypes { get; set; } = new();

        [YamlMember(Alias = "memberGroups")]
        public List<YamlMemberGroup> MemberGroups { get; set; } = new();

        [YamlMember(Alias = "templates")]
        public List<YamlTemplate> Templates { get; set; } = new();

        [YamlMember(Alias = "content")]
        public List<YamlContent> Content { get; set; } = new();

        [YamlMember(Alias = "media")]
        public List<YamlMedia> Media { get; set; } = new();

        [YamlMember(Alias = "scripts")]
        public List<YamlScript> Scripts { get; set; } = new();

        [YamlMember(Alias = "stylesheets")]
        public List<YamlStylesheet> Stylesheets { get; set; } = new();

        [YamlMember(Alias = "languages")]
        public List<YamlLanguage> Languages { get; set; } = new();

        [YamlMember(Alias = "dictionaryItems")]
        public List<YamlDictionaryItem> DictionaryItems { get; set; } = new();

        [YamlMember(Alias = "members")]
        public List<YamlMember> Members { get; set; } = new();

        [YamlMember(Alias = "users")]
        public List<YamlUser> Users { get; set; } = new();

        [YamlMember(Alias = "packages")]
        public List<YamlPackage> Packages { get; set; } = new();

        [YamlMember(Alias = "propertyEditors")]
        public List<YamlPropertyEditor> PropertyEditors { get; set; } = new();

        /// <summary>
        /// Default folder applied to all <c>media:</c> items that do not specify their own <c>folder:</c>.
        /// Supports nested paths (e.g. "Images/Uploads"). Folders are created automatically.
        /// </summary>
        [YamlMember(Alias = "mediaDefaultFolder")]
        public string? MediaDefaultFolder { get; set; }

        /// <summary>
        /// Umbraco Models Builder configuration. When present, the plugin writes the specified
        /// settings into <c>appsettings.json</c> under <c>Umbraco:CMS:ModelsBuilder</c> at startup.
        /// </summary>
        [YamlMember(Alias = "modelsBuilder")]
        public YamlModelsBuilder? ModelsBuilder { get; set; }
    }

    public class YamlDataType
    {
        [YamlMember(Alias = "alias")]
        public string Alias { get; set; }

        [YamlMember(Alias = "name")]
        public string Name { get; set; }

        [YamlMember(Alias = "editorUiAlias")]
        public string Editor { get; set; }

        [YamlMember(Alias = "config")]
        public Dictionary<string, object> Config { get; set; } = new();

        /// <summary>
        /// Optional storage type override for custom (frontend-only) property editors that have
        /// no server-side <c>IDataEditor</c> registration. Accepted values: NVARCHAR (default),
        /// NTEXT, TEXT, INT, INTEGER, BIGINT, DECIMAL, DATE.
        /// </summary>
        [YamlMember(Alias = "valueType")]
        public string? ValueType { get; set; }

        [YamlMember(Alias = "remove")]
        public bool Remove { get; set; } = false;

        [YamlMember(Alias = "update")]
        public bool Update { get; set; } = false;
    }

    public class YamlDocumentType
    {
        [YamlMember(Alias = "alias")]
        public string Alias { get; set; }

        [YamlMember(Alias = "name")]
        public string Name { get; set; }

        [YamlMember(Alias = "icon")]
        public string Icon { get; set; }

        [YamlMember(Alias = "description")]
        public string? Description { get; set; }

        [YamlMember(Alias = "isElement")]
        public bool IsElement { get; set; } = false;

        [YamlMember(Alias = "allowAsRoot")]
        public bool AllowAsRoot { get; set; } = true;

        [YamlMember(Alias = "allowedChildTypes")]
        public List<string> AllowedChildTypes { get; set; } = new();

        /// <summary>
        /// Aliases of other document/element types to use as compositions (mixins).
        /// Compositions add their property groups and properties to this type without inheritance.
        /// </summary>
        [YamlMember(Alias = "compositions")]
        public List<string> Compositions { get; set; } = new();

        [YamlMember(Alias = "tabs")]
        public List<YamlTab> Tabs { get; set; } = new();

        [YamlMember(Alias = "allowedTemplates")]
        public List<string> AllowedTemplates { get; set; } = new();

        [YamlMember(Alias = "defaultTemplate")]
        public string DefaultTemplate { get; set; }

        [YamlMember(Alias = "remove")]
        public bool Remove { get; set; } = false;

        [YamlMember(Alias = "update")]
        public bool Update { get; set; } = false;
    }

    public class YamlTab
    {
        [YamlMember(Alias = "name")]
        public string Name { get; set; }

        [YamlMember(Alias = "sortOrder")]
        public int SortOrder { get; set; } = 0;

        [YamlMember(Alias = "properties")]
        public List<YamlProperty> Properties { get; set; } = new();
    }

    public class YamlProperty
    {
        [YamlMember(Alias = "alias")]
        public string Alias { get; set; }

        [YamlMember(Alias = "name")]
        public string Name { get; set; }

        [YamlMember(Alias = "dataType")]
        public string DataType { get; set; }

        [YamlMember(Alias = "required")]
        public bool Required { get; set; } = false;

        [YamlMember(Alias = "description")]
        public string Description { get; set; }

        [YamlMember(Alias = "sortOrder")]
        public int SortOrder { get; set; } = 0;

        [YamlMember(Alias = "validationRegExp")]
        public string? ValidationRegExp { get; set; }
    }

    public class YamlTemplate
    {
        [YamlMember(Alias = "alias")]
        public string Alias { get; set; }

        [YamlMember(Alias = "name")]
        public string Name { get; set; }

        [YamlMember(Alias = "path")]
        public string Path { get; set; }

        [YamlMember(Alias = "masterTemplate")]
        public string MasterTemplate { get; set; }

        /// <summary>
        /// Explicit Razor content for this template. When provided, overrides the auto-generated
        /// scaffold. Use a YAML block scalar (|) for multi-line Razor views.
        /// </summary>
        [YamlMember(Alias = "content")]
        public string? RazorContent { get; set; }

        /// <summary>Paths to JavaScript files (relative to wwwroot) to inject into the generated template.</summary>
        [YamlMember(Alias = "scripts")]
        public List<string> Scripts { get; set; } = new();

        /// <summary>Paths to CSS stylesheets (relative to wwwroot) to inject into the generated template.</summary>
        [YamlMember(Alias = "stylesheets")]
        public List<string> Stylesheets { get; set; } = new();

        [YamlMember(Alias = "remove")]
        public bool Remove { get; set; } = false;

        [YamlMember(Alias = "update")]
        public bool Update { get; set; } = false;
    }

    public class YamlScript
    {
        [YamlMember(Alias = "alias")]
        public string Alias { get; set; }

        [YamlMember(Alias = "name")]
        public string Name { get; set; }

        /// <summary>Output path relative to wwwroot (e.g. "js/site.js").</summary>
        [YamlMember(Alias = "path")]
        public string Path { get; set; }

        [YamlMember(Alias = "content")]
        public string Content { get; set; }

        [YamlMember(Alias = "remove")]
        public bool Remove { get; set; } = false;

        [YamlMember(Alias = "update")]
        public bool Update { get; set; } = false;
    }

    public class YamlStylesheet
    {
        [YamlMember(Alias = "alias")]
        public string Alias { get; set; }

        [YamlMember(Alias = "name")]
        public string Name { get; set; }

        /// <summary>Output path relative to wwwroot (e.g. "css/site.css").</summary>
        [YamlMember(Alias = "path")]
        public string Path { get; set; }

        [YamlMember(Alias = "content")]
        public string Content { get; set; }

        [YamlMember(Alias = "remove")]
        public bool Remove { get; set; } = false;

        [YamlMember(Alias = "update")]
        public bool Update { get; set; } = false;
    }

    public class YamlContent
    {
        [YamlMember(Alias = "alias")]
        public string Alias { get; set; }

        [YamlMember(Alias = "name")]
        public string Name { get; set; }

        [YamlMember(Alias = "documentType")]
        public string Type { get; set; }

        [YamlMember(Alias = "isPublished")]
        public bool Published { get; set; } = false;

        [YamlMember(Alias = "sortOrder")]
        public int SortOrder { get; set; } = 0;

        [YamlMember(Alias = "properties")]
        public Dictionary<string, object> Values { get; set; } = new();

        [YamlMember(Alias = "children")]
        public List<YamlContent> Children { get; set; } = new();

        [YamlMember(Alias = "remove")]
        public bool Remove { get; set; } = false;

        [YamlMember(Alias = "update")]
        public bool Update { get; set; } = false;
    }

    // ── Media Type ────────────────────────────────────────────────────────────

    public class YamlMediaType
    {
        [YamlMember(Alias = "alias")]
        public string Alias { get; set; }

        [YamlMember(Alias = "name")]
        public string Name { get; set; }

        [YamlMember(Alias = "icon")]
        public string? Icon { get; set; }

        [YamlMember(Alias = "allowedAtRoot")]
        public bool AllowedAtRoot { get; set; } = false;

        [YamlMember(Alias = "tabs")]
        public List<YamlTab> Tabs { get; set; } = new();

        [YamlMember(Alias = "remove")]
        public bool Remove { get; set; } = false;

        [YamlMember(Alias = "update")]
        public bool Update { get; set; } = false;
    }

    // ── Media ─────────────────────────────────────────────────────────────────

    public class YamlMedia
    {
        [YamlMember(Alias = "alias")]
        public string Alias { get; set; }

        [YamlMember(Alias = "name")]
        public string Name { get; set; }

        /// <summary>Alias of the Media Type (e.g. "Image", "File").</summary>
        [YamlMember(Alias = "mediaType")]
        public string MediaType { get; set; }

        /// <summary>
        /// URL to download file content from. When provided, the file is downloaded
        /// and attached as the umbracoFile property value.
        /// </summary>
        [YamlMember(Alias = "url")]
        public string? Url { get; set; }

        /// <summary>
        /// Optional folder path where this media item will be placed (e.g. "Images" or "Images/Partners").
        /// Folders are created automatically if they do not exist.
        /// </summary>
        [YamlMember(Alias = "folder")]
        public string? Folder { get; set; }

        [YamlMember(Alias = "properties")]
        public Dictionary<string, object> Properties { get; set; } = new();

        [YamlMember(Alias = "children")]
        public List<YamlMedia> Children { get; set; } = new();

        [YamlMember(Alias = "remove")]
        public bool Remove { get; set; } = false;

        [YamlMember(Alias = "update")]
        public bool Update { get; set; } = false;
    }

    // ── Language ──────────────────────────────────────────────────────────────

    public class YamlLanguage
    {
        /// <summary>ISO culture code, e.g. "en-US", "fr-FR".</summary>
        [YamlMember(Alias = "isoCode")]
        public string IsoCode { get; set; }

        [YamlMember(Alias = "cultureName")]
        public string? CultureName { get; set; }

        [YamlMember(Alias = "isDefault")]
        public bool IsDefault { get; set; } = false;

        [YamlMember(Alias = "isMandatory")]
        public bool IsMandatory { get; set; } = false;

        [YamlMember(Alias = "remove")]
        public bool Remove { get; set; } = false;

        [YamlMember(Alias = "update")]
        public bool Update { get; set; } = false;
    }

    // ── Dictionary Item ───────────────────────────────────────────────────────

    public class YamlDictionaryItem
    {
        [YamlMember(Alias = "key")]
        public string Key { get; set; }

        /// <summary>Translation values keyed by ISO culture code.</summary>
        [YamlMember(Alias = "translations")]
        public Dictionary<string, string> Translations { get; set; } = new();

        [YamlMember(Alias = "remove")]
        public bool Remove { get; set; } = false;

        [YamlMember(Alias = "update")]
        public bool Update { get; set; } = false;
    }

    // ── Member ────────────────────────────────────────────────────────────────

    public class YamlMember
    {
        [YamlMember(Alias = "alias")]
        public string Alias { get; set; }

        [YamlMember(Alias = "name")]
        public string Name { get; set; }

        [YamlMember(Alias = "email")]
        public string Email { get; set; }

        [YamlMember(Alias = "username")]
        public string Username { get; set; }

        [YamlMember(Alias = "password")]
        public string Password { get; set; }

        /// <summary>Alias of the Member Type (defaults to "Member").</summary>
        [YamlMember(Alias = "memberType")]
        public string MemberType { get; set; } = "Member";

        [YamlMember(Alias = "isApproved")]
        public bool IsApproved { get; set; } = true;

        [YamlMember(Alias = "properties")]
        public Dictionary<string, object> Properties { get; set; } = new();

        [YamlMember(Alias = "remove")]
        public bool Remove { get; set; } = false;

        [YamlMember(Alias = "update")]
        public bool Update { get; set; } = false;
    }

    // ── User ──────────────────────────────────────────────────────────────────

    public class YamlUser
    {
        [YamlMember(Alias = "alias")]
        public string Alias { get; set; }

        [YamlMember(Alias = "name")]
        public string Name { get; set; }

        [YamlMember(Alias = "email")]
        public string Email { get; set; }

        [YamlMember(Alias = "username")]
        public string Username { get; set; }

        /// <summary>User group aliases to assign (e.g. "admin", "editor").</summary>
        [YamlMember(Alias = "userGroups")]
        public List<string> UserGroups { get; set; } = new();

        [YamlMember(Alias = "remove")]
        public bool Remove { get; set; } = false;

        [YamlMember(Alias = "update")]
        public bool Update { get; set; } = false;
    }

    // ── Package ───────────────────────────────────────────────────────────────

    /// <summary>
    /// Declares a NuGet package that the site depends on. At startup the plugin checks whether
    /// the package assembly is loaded in the current AppDomain and logs a warning (or error if
    /// <c>required: true</c>) if it is missing.
    /// </summary>
    public class YamlPackage
    {
        /// <summary>NuGet package ID (e.g. "Our.Umbraco.Community.SomePlugin").</summary>
        [YamlMember(Alias = "id")]
        public string Id { get; set; }

        /// <summary>Expected version string (informational, e.g. "2.0.0"). Logged if mismatch found.</summary>
        [YamlMember(Alias = "version")]
        public string? Version { get; set; }

        /// <summary>When true, a missing assembly is logged as an error rather than a warning.</summary>
        [YamlMember(Alias = "required")]
        public bool Required { get; set; } = false;

        /// <summary>
        /// Override the assembly name to check when it differs from the NuGet package ID
        /// (e.g. "Our.Umbraco.Community.SomePlugin.Core").
        /// </summary>
        [YamlMember(Alias = "assemblyName")]
        public string? AssemblyName { get; set; }

        /// <summary>
        /// When <c>true</c>, the plugin downloads the package from NuGet (or <c>source</c>) and
        /// loads its assemblies into the current AppDomain if they are not already present.
        /// The package is cached in the <c>packages/</c> directory under the content root.
        ///
        /// Note: assemblies loaded at runtime will not have their DI / IComposer registrations
        /// executed. A full Umbraco package install (with DI wiring) requires a restart.
        /// </summary>
        [YamlMember(Alias = "install")]
        public bool Install { get; set; } = false;

        /// <summary>
        /// NuGet feed URL to download from. Defaults to <c>https://api.nuget.org/v3/index.json</c>.
        /// The flat-container endpoint is derived automatically from this base URL.
        /// </summary>
        [YamlMember(Alias = "source")]
        public string? Source { get; set; }
    }

    // ── ModelsBuilder ─────────────────────────────────────────────────────────

    /// <summary>
    /// Controls Umbraco Models Builder settings written to <c>appsettings.json</c>.
    /// </summary>
    public class YamlModelsBuilder
    {
        /// <summary>
        /// Absolute or content-root-relative path where generated model files are written
        /// (maps to <c>Umbraco:CMS:ModelsBuilder:ModelsDirectoryAbsolute</c>).
        /// </summary>
        [YamlMember(Alias = "outputPath")]
        public string? OutputPath { get; set; }

        /// <summary>
        /// Models Builder mode: <c>InMemoryAuto</c>, <c>SourceCodeAuto</c>,
        /// <c>SourceCodeManual</c>, or <c>Nothing</c>
        /// (maps to <c>Umbraco:CMS:ModelsBuilder:ModelsMode</c>).
        /// </summary>
        [YamlMember(Alias = "mode")]
        public string? Mode { get; set; }
    }

    // ── Member Type ───────────────────────────────────────────────────────────

    /// <summary>
    /// Defines a custom Umbraco member type (used to extend the built-in Member schema with
    /// site-specific properties). Member types group properties into tabs exactly like document types.
    /// </summary>
    public class YamlMemberType
    {
        [YamlMember(Alias = "alias")]
        public string Alias { get; set; }

        [YamlMember(Alias = "name")]
        public string Name { get; set; }

        [YamlMember(Alias = "description")]
        public string? Description { get; set; }

        [YamlMember(Alias = "icon")]
        public string? Icon { get; set; }

        [YamlMember(Alias = "tabs")]
        public List<YamlMemberTypeTab> Tabs { get; set; } = new();

        [YamlMember(Alias = "remove")]
        public bool Remove { get; set; } = false;

        [YamlMember(Alias = "update")]
        public bool Update { get; set; } = false;
    }

    public class YamlMemberTypeTab
    {
        [YamlMember(Alias = "name")]
        public string Name { get; set; }

        [YamlMember(Alias = "sortOrder")]
        public int SortOrder { get; set; } = 0;

        [YamlMember(Alias = "properties")]
        public List<YamlMemberTypeProperty> Properties { get; set; } = new();
    }

    public class YamlMemberTypeProperty
    {
        [YamlMember(Alias = "alias")]
        public string Alias { get; set; }

        [YamlMember(Alias = "name")]
        public string Name { get; set; }

        [YamlMember(Alias = "dataType")]
        public string DataType { get; set; }

        [YamlMember(Alias = "required")]
        public bool Required { get; set; } = false;

        [YamlMember(Alias = "description")]
        public string? Description { get; set; }

        [YamlMember(Alias = "sortOrder")]
        public int SortOrder { get; set; } = 0;

        [YamlMember(Alias = "validationRegExp")]
        public string? ValidationRegExp { get; set; }

        /// <summary>When true the member can edit this property from their profile.</summary>
        [YamlMember(Alias = "memberCanEdit")]
        public bool MemberCanEdit { get; set; } = false;

        /// <summary>When true the property is visible on the member's public profile.</summary>
        [YamlMember(Alias = "showOnProfile")]
        public bool ShowOnProfile { get; set; } = false;

        /// <summary>When true the property value is treated as sensitive (hidden in the back-office unless the current user has access).</summary>
        [YamlMember(Alias = "isSensitive")]
        public bool IsSensitive { get; set; } = false;
    }

    // ── Member Group ──────────────────────────────────────────────────────────

    /// <summary>
    /// Declares an Umbraco member group. Member groups are used for public access rules
    /// (restricting content to logged-in members belonging to specific groups).
    /// </summary>
    public class YamlMemberGroup
    {
        [YamlMember(Alias = "name")]
        public string Name { get; set; }

        [YamlMember(Alias = "remove")]
        public bool Remove { get; set; } = false;
    }

    // ── PropertyEditor ────────────────────────────────────────────────────────

    /// <summary>
    /// Defines a custom (frontend-only) Umbraco property editor. The plugin writes an
    /// <c>App_Plugins/[folderName]/umbraco-package.json</c> manifest and, when
    /// <c>jsContent</c> is provided, the corresponding JavaScript file.
    /// </summary>
    public class YamlPropertyEditor
    {
        /// <summary>
        /// Schema alias used both as the <c>propertyEditorSchema</c> alias in the manifest
        /// and as the editor alias in DataType definitions (e.g. "My.CustomTextEditor").
        /// </summary>
        [YamlMember(Alias = "alias")]
        public string Alias { get; set; }

        [YamlMember(Alias = "name")]
        public string Name { get; set; }

        /// <summary>Backoffice icon alias (e.g. "icon-code"). Defaults to "icon-code".</summary>
        [YamlMember(Alias = "icon")]
        public string? Icon { get; set; }

        /// <summary>Backoffice group (e.g. "common", "lists"). Defaults to "common".</summary>
        [YamlMember(Alias = "group")]
        public string? Group { get; set; }

        /// <summary>
        /// UI component alias (e.g. "My.PropertyEditorUi.CustomEditor").
        /// Auto-derived as <c>{Alias}.Ui</c> when not specified.
        /// </summary>
        [YamlMember(Alias = "uiAlias")]
        public string? UiAlias { get; set; }

        /// <summary>
        /// App_Plugins sub-folder name. Auto-derived from <c>alias</c> (dots replaced with dashes,
        /// lower-cased) when not specified.
        /// </summary>
        [YamlMember(Alias = "folderName")]
        public string? FolderName { get; set; }

        /// <summary>
        /// URL path to the JavaScript file served from wwwroot
        /// (e.g. "/App_Plugins/my-editor/index.js"). Auto-derived when not specified.
        /// </summary>
        [YamlMember(Alias = "jsPath")]
        public string? JsPath { get; set; }

        /// <summary>Inline JavaScript content written to the JS file (use YAML block scalar |).</summary>
        [YamlMember(Alias = "jsContent")]
        public string? JsContent { get; set; }

        [YamlMember(Alias = "remove")]
        public bool Remove { get; set; } = false;

        [YamlMember(Alias = "update")]
        public bool Update { get; set; } = false;
    }
}
