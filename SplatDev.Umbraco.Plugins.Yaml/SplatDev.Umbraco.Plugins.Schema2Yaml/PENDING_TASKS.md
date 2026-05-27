# Schema2Yaml Implementation Tasks

## Baseline Reference
- **Structure baseline**: `Umbraco.Plugins.Yaml2Schema` project
- **Coding patterns**: Service-based architecture, Model-driven YAML, Composer DI registration
- **Inverse logic**: Each Creator service → Exporter service counterpart

---

## Phase 1: Core Implementation

### 1. Models & Data Structures

#### 1.1 Create Export Models
**File**: `src/Models/ExportModels.cs`

Create mirror models from Yaml2Schema for export:
- `ExportRoot` (contains all export sections)
- `ExportDataType`, `ExportDocumentType`, `ExportMediaType`
- `ExportTemplate`, `ExportContent`, `ExportMedia`
- `ExportLanguage`, `ExportDictionaryItem`
- `ExportMember`, `ExportUser`

**Reference**: `Umbraco.Plugins.Yaml2Schema\src\Models\YamlModels.cs`

**Key considerations**:
- Use YamlDotNet attributes for serialization
- Support optional properties (nullables where appropriate)
- Include metadata fields (created date, modified date, version info)
- Add Umbraco version detection property

---

### 2. Core Export Services

#### 2.1 DataType Exporter
**File**: `src/Services/DataTypeExporter.cs`

**Inverse of**: `Umbraco.Plugins.Yaml2Schema\src\Services\DataTypeCreator.cs`

**Logic**:
```csharp
public class DataTypeExporter
{
    private readonly IDataTypeService _dataTypeService;

    public async Task<List<ExportDataType>> ExportAsync()
    {
        // Get all data types
        var dataTypes = _dataTypeService.GetAll();

        // Convert to export models
        return dataTypes.Select(dt => new ExportDataType
        {
            Alias = dt.Key.ToString(), // or custom alias
            Name = dt.Name,
            EditorUiAlias = dt.EditorUiAlias,
            Config = ExtractConfiguration(dt),
            ValueType = dt.DatabaseType.ToString()
        }).ToList();
    }

    private Dictionary<string, object> ExtractConfiguration(IDataType dataType)
    {
        // Extract configuration from dataType.Configuration
        // Handle special cases: Block List, Block Grid, Image Cropper, etc.
    }
}
```

**Special handling**:
- Block List/Grid: Export block configuration with element type aliases
- Dropdown/CheckBox: Export items as string list
- Image Cropper: Export crop definitions
- Generic configs: Serialize as key-value dictionary

---

#### 2.2 DocumentType Exporter
**File**: `src/Services/DocumentTypeExporter.cs`

**Inverse of**: `Umbraco.Plugins.Yaml2Schema\src\Services\DocumentTypeCreator.cs`

**Logic**:
```csharp
public class DocumentTypeExporter
{
    private readonly IContentTypeService _contentTypeService;

    public async Task<List<ExportDocumentType>> ExportAsync()
    {
        var contentTypes = _contentTypeService.GetAll();

        return contentTypes.Select(ct => new ExportDocumentType
        {
            Alias = ct.Alias,
            Name = ct.Name,
            Icon = ct.Icon,
            AllowedAsRoot = ct.AllowedAsRoot,
            IsElement = ct.IsElement,
            Compositions = ct.ContentTypeComposition.Select(c => c.Alias).ToList(),
            AllowedTemplates = ct.AllowedTemplates.Select(t => t.Alias).ToList(),
            DefaultTemplate = ct.DefaultTemplate?.Alias,
            Tabs = ExportTabs(ct),
            AllowedChildTypes = ct.AllowedContentTypes.Select(a => a.Alias).ToList()
        }).ToList();
    }

    private List<ExportTab> ExportTabs(IContentType contentType)
    {
        // Group properties by tab
        // Export tab name, sort order, properties
    }
}
```

**Special handling**:
- Compositions: Export as alias list (resolved on import)
- Generic properties: Include inherited properties flag
- Element types: Set `IsElement: true`
- Property validators: Export validation rules

---

#### 2.3 MediaType Exporter
**File**: `src/Services/MediaTypeExporter.cs`

**Inverse of**: `Umbraco.Plugins.Yaml2Schema\src\Services\MediaTypeCreator.cs`

Similar to DocumentTypeExporter but for media types.

---

#### 2.4 Template Exporter
**File**: `src/Services/TemplateExporter.cs`

**Inverse of**: `Umbraco.Plugins.Yaml2Schema\src\Services\TemplateCreator.cs`

**Logic**:
```csharp
public class TemplateExporter
{
    private readonly IFileService _fileService;

    public async Task<List<ExportTemplate>> ExportAsync()
    {
        var templates = _fileService.GetTemplates();

        return templates.Select(t => new ExportTemplate
        {
            Alias = t.Alias,
            Name = t.Name,
            Content = t.Content, // Full Razor content
            MasterTemplateAlias = t.MasterTemplateAlias
        }).ToList();
    }
}
```

---

#### 2.5 Content Exporter
**File**: `src/Services/ContentExporter.cs`

**Inverse of**: `Umbraco.Plugins.Yaml2Schema\src\Services\ContentCreator.cs`

**Logic**:
```csharp
public class ContentExporter
{
    private readonly IContentService _contentService;

    public async Task<List<ExportContent>> ExportAsync()
    {
        // Get all content at root
        var roots = _contentService.GetRootContent();
        var allContent = new List<ExportContent>();

        foreach (var root in roots)
        {
            allContent.Add(ExportNode(root));
        }

        return allContent;
    }

    private ExportContent ExportNode(IContent content)
    {
        return new ExportContent
        {
            Name = content.Name,
            DocumentType = content.ContentType.Alias,
            Template = content.TemplateId.HasValue 
                ? _fileService.GetTemplate(content.TemplateId.Value)?.Alias 
                : null,
            SortOrder = content.SortOrder,
            Published = content.Published,
            Properties = ExportProperties(content),
            Children = ExportChildren(content) // Recursive
        };
    }

    private Dictionary<string, object> ExportProperties(IContent content)
    {
        // Export all property values
        // Handle special types: Media Picker (resolve to names), Content Picker, etc.
    }
}
```

**Special handling**:
- Hierarchical export (children nested)
- Property value resolution (IDs → Names/Aliases)
- Multi-language variants
- Published vs unpublished state

---

#### 2.6 Media Exporter
**File**: `src/Services/MediaExporter.cs`

**Inverse of**: `Umbraco.Plugins.Yaml2Schema\src\Services\MediaCreator.cs`

**Logic**:
```csharp
public class MediaExporter
{
    private readonly IMediaService _mediaService;
    private readonly IWebHostEnvironment _env;

    public async Task<(List<ExportMedia>, Dictionary<string, byte[]>)> ExportAsync()
    {
        var mediaItems = _mediaService.GetRootMedia();
        var files = new Dictionary<string, byte[]>();
        var exported = new List<ExportMedia>();

        foreach (var item in mediaItems)
        {
            ExportMediaItem(item, exported, files, "");
        }

        return (exported, files);
    }

    private void ExportMediaItem(IMedia media, List<ExportMedia> exported, 
        Dictionary<string, byte[]> files, string folder)
    {
        var export = new ExportMedia
        {
            Name = media.Name,
            MediaType = media.ContentType.Alias,
            Folder = folder,
            Properties = ExportProperties(media)
        };

        // Download file if exists
        if (media.HasProperty("umbracoFile"))
        {
            var filePath = media.GetValue<string>("umbracoFile");
            var physicalPath = _env.MapPathWebRoot(filePath);

            if (File.Exists(physicalPath))
            {
                files[$"{folder}/{media.Name}"] = await File.ReadAllBytesAsync(physicalPath);
                export.Url = filePath;
            }
        }

        exported.Add(export);

        // Recursive for folders
        if (media.ContentType.Alias == "Folder")
        {
            var children = _mediaService.GetPagedChildren(media.Id, 0, int.MaxValue, out _);
            foreach (var child in children)
            {
                ExportMediaItem(child, exported, files, $"{folder}/{media.Name}");
            }
        }
    }
}
```

**Special handling**:
- Maintain folder hierarchy
- Download all physical files
- Store in same structure as Umbraco media folder
- Handle Image Cropper data

---

#### 2.7 Language Exporter
**File**: `src/Services/LanguageExporter.cs`

**Inverse of**: `Umbraco.Plugins.Yaml2Schema\src\Services\LanguageCreator.cs`

Simple export of language configuration.

---

#### 2.8 Dictionary Exporter
**File**: `src/Services/DictionaryExporter.cs`

**Inverse of**: `Umbraco.Plugins.Yaml2Schema\src\Services\DictionaryCreator.cs`

**Logic**:
- Export all dictionary items
- Include translations for all languages
- Maintain parent-child hierarchy

---

#### 2.9 Member Exporter
**File**: `src/Services/MemberExporter.cs`

**Inverse of**: `Umbraco.Plugins.Yaml2Schema\src\Services\MemberCreator.cs`

**Security note**: Exclude passwords (require reset on import)

---

#### 2.10 User Exporter
**File**: `src/Services/UserExporter.cs`

**Inverse of**: `Umbraco.Plugins.Yaml2Schema\src\Services\UserCreator.cs`

**Security note**: Exclude passwords, make optional via config

---

### 3. Orchestration Service

#### 3.1 Schema Export Service
**File**: `src/Services/SchemaExportService.cs`

**Reference**: `Umbraco.Plugins.Yaml2Schema\src\Handlers\YamlInitializationHandler.cs` (but reverse direction)

```csharp
public class SchemaExportService
{
    private readonly DataTypeExporter _dataTypeExporter;
    private readonly DocumentTypeExporter _documentTypeExporter;
    // ... all other exporters

    public async Task<string> ExportToYamlAsync()
    {
        var root = new ExportRoot
        {
            Umbraco = new UmbracoExport
            {
                Languages = await _languageExporter.ExportAsync(),
                DataTypes = await _dataTypeExporter.ExportAsync(),
                DocumentTypes = await _documentTypeExporter.ExportAsync(),
                MediaTypes = await _mediaTypeExporter.ExportAsync(),
                Templates = await _templateExporter.ExportAsync(),
                Media = (await _mediaExporter.ExportAsync()).Item1,
                Content = await _contentExporter.ExportAsync(),
                DictionaryItems = await _dictionaryExporter.ExportAsync(),
                Members = await _memberExporter.ExportAsync(),
                Users = await _userExporter.ExportAsync()
            }
        };

        // Serialize to YAML
        var serializer = new SerializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();

        return serializer.Serialize(root);
    }

    public async Task<byte[]> ExportToZipAsync()
    {
        var yaml = await ExportToYamlAsync();
        var (mediaList, mediaFiles) = await _mediaExporter.ExportAsync();

        using var memStream = new MemoryStream();
        using (var archive = new ZipArchive(memStream, ZipArchiveMode.Create, true))
        {
            // Add YAML
            var yamlEntry = archive.CreateEntry("umbraco.yaml");
            using (var entryStream = yamlEntry.Open())
            using (var writer = new StreamWriter(entryStream))
            {
                await writer.WriteAsync(yaml);
            }

            // Add media files
            foreach (var (path, bytes) in mediaFiles)
            {
                var mediaEntry = archive.CreateEntry($"media/{path}");
                using (var entryStream = mediaEntry.Open())
                {
                    await entryStream.WriteAsync(bytes);
                }
            }
        }

        memStream.Position = 0;
        return memStream.ToArray();
    }
}
```

---

### 4. Dependency Injection

#### 4.1 Composer
**File**: `src/Composers/Schema2YamlComposer.cs`

**Reference**: `Umbraco.Plugins.Yaml2Schema\src\Composers\YamlStartupComposer.cs`

```csharp
public class Schema2YamlComposer : IComposer
{
    public void Compose(IUmbracoBuilder builder)
    {
        // Register all exporters
        builder.Services.AddScoped<DataTypeExporter>();
        builder.Services.AddScoped<DocumentTypeExporter>();
        builder.Services.AddScoped<MediaTypeExporter>();
        builder.Services.AddScoped<TemplateExporter>();
        builder.Services.AddScoped<ContentExporter>();
        builder.Services.AddScoped<MediaExporter>();
        builder.Services.AddScoped<LanguageExporter>();
        builder.Services.AddScoped<DictionaryExporter>();
        builder.Services.AddScoped<MemberExporter>();
        builder.Services.AddScoped<UserExporter>();

        // Register orchestration service
        builder.Services.AddScoped<ISchemaExportService, SchemaExportService>();
    }
}
```

---

### 5. Dashboard UI

#### 5.1 Lit Dashboard Component
**File**: `App_Plugins/Schema2Yaml/dashboard.js`

**Features**:
- Export button with progress indicator
- YAML preview panel (syntax highlighted)
- Download YAML button
- Download ZIP button
- Export statistics (counts by type)
- Version info display

**UI Structure**:
```html
<schema-export-dashboard>
  <header>
    <h1>Schema Export</h1>
    <p>Umbraco Version: 17.0.0</p>
  </header>

  <section class="actions">
    <button @click="exportYaml">Export to YAML</button>
    <button @click="downloadYaml" ?disabled="!hasExport">Download YAML</button>
    <button @click="downloadZip" ?disabled="!hasExport">Download ZIP</button>
  </section>

  <section class="stats" ?hidden="!stats">
    <h2>Export Summary</h2>
    <ul>
      <li>DataTypes: ${stats.dataTypes}</li>
      <li>DocumentTypes: ${stats.documentTypes}</li>
      <li>Content Nodes: ${stats.content}</li>
      <li>Media Items: ${stats.media}</li>
      <!-- ... -->
    </ul>
  </section>

  <section class="preview" ?hidden="!yaml">
    <h2>Generated YAML</h2>
    <pre><code class="language-yaml">${yaml}</code></pre>
  </section>
</schema-export-dashboard>
```

#### 5.2 Dashboard Controller
**File**: `src/Controllers/SchemaExportController.cs`

```csharp
[Authorize(Policy = AuthorizationPolicies.SectionAccessSettings)]
public class SchemaExportController : UmbracoApiController
{
    private readonly ISchemaExportService _exportService;

    [HttpGet]
    public async Task<IActionResult> ExportYaml()
    {
        var yaml = await _exportService.ExportToYamlAsync();
        return Ok(new { yaml, stats = _exportService.GetStats() });
    }

    [HttpGet]
    public async Task<IActionResult> DownloadYaml()
    {
        var yaml = await _exportService.ExportToYamlAsync();
        return File(
            Encoding.UTF8.GetBytes(yaml),
            "application/x-yaml",
            "umbraco.yaml"
        );
    }

    [HttpGet]
    public async Task<IActionResult> DownloadZip()
    {
        var zip = await _exportService.ExportToZipAsync();
        return File(zip, "application/zip", "umbraco-export.zip");
    }
}
```

#### 5.3 Dashboard Registration
**File**: `src/Composers/DashboardComposer.cs`

```csharp
public class DashboardComposer : IComposer
{
    public void Compose(IUmbracoBuilder builder)
    {
        builder.Dashboards().Add<SchemaExportDashboard>();
    }
}

public class SchemaExportDashboard : IDashboard
{
    public string Alias => "schemaExport";
    public string View => "/App_Plugins/Schema2Yaml/dashboard.html";
    public string[] Sections => new[] { Constants.Applications.Settings };
    public IAccessRule[] AccessRules => Array.Empty<IAccessRule>();
}
```

---

### 6. Version Compatibility

#### 6.1 Version Detector
**File**: `src/Services/UmbracoVersionDetector.cs`

```csharp
public class UmbracoVersionDetector
{
    private readonly IUmbracoVersion _umbracoVersion;

    public UmbracoVersion GetVersion()
    {
        var version = _umbracoVersion.Version;

        return version.Major switch
        {
            13 => UmbracoVersion.V13,
            14 => UmbracoVersion.V14,
            15 => UmbracoVersion.V15,
            16 => UmbracoVersion.V16,
            17 => UmbracoVersion.V17,
            _ => throw new NotSupportedException($"Umbraco {version} is not supported")
        };
    }
}

public enum UmbracoVersion
{
    V13, V14, V15, V16, V17
}
```

#### 6.2 Version-Specific Export Logic

Each exporter implements version-specific handling:

**Example** (DataTypeExporter):
```csharp
private string GetEditorAlias(IDataType dataType)
{
    if (_versionDetector.GetVersion() == UmbracoVersion.V13)
    {
        // Use legacy editor alias
        return dataType.EditorAlias;
    }
    else
    {
        // Use new editorUiAlias (V14+)
        return dataType.EditorUiAlias;
    }
}
```

**Version differences to handle**:
- V13: Legacy `PropertyEditorAlias`, old Block List format
- V14-17: New `EditorUiAlias`, Block List v2, new property editors

---

### 7. Testing

#### 7.1 Unit Tests
**Project**: `SplatDev.Umbraco.Plugins.Schema2Yaml.Tests`

**Test structure**:
```
Tests/
  Services/
    DataTypeExporterTests.cs
    DocumentTypeExporterTests.cs
    ContentExporterTests.cs
    MediaExporterTests.cs
    SchemaExportServiceTests.cs
  Integration/
    FullExportTests.cs
    ZipCreationTests.cs
  Version/
    Umbraco17ExportTests.cs
```

**Key test scenarios**:
- Export each entity type individually
- Full export with all types
- ZIP creation with media files
- Version-specific format differences
- Round-trip: Export → Import (with Yaml2Schema)

#### 7.2 Test Helpers
- Mock Umbraco services (IDataTypeService, IContentService, etc.)
- Sample data builders for each entity type
- YAML assertion helpers

---

### 8. Configuration

#### 8.1 Options Pattern
**File**: `src/Configuration/Schema2YamlOptions.cs`

```csharp
public class Schema2YamlOptions
{
    public const string SectionName = "UmbracoSchema2Yaml";

    public string ExportPath { get; set; } = "exports/umbraco.yaml";
    public bool IncludeMedia { get; set; } = true;
    public string MediaPath { get; set; } = "exports/media";
    public bool IncludeContent { get; set; } = true;
    public bool IncludeUsers { get; set; } = false;
    public bool CompressYaml { get; set; } = false;
}
```

#### 8.2 Configuration Registration
```csharp
builder.Services.Configure<Schema2YamlOptions>(
    builder.Config.GetSection(Schema2YamlOptions.SectionName)
);
```

---

## Success Criteria

- ✅ All entity types export to valid YAML
- ✅ Generated YAML imports successfully with Yaml2Schema plugin
- ✅ Media files downloaded and structured correctly
- ✅ ZIP contains all YAML + media files
- ✅ Dashboard UI functional and user-friendly
- ✅ Supports Umbraco 14-17 with version detection
- ✅ Unit tests cover all exporters
- ✅ Documentation complete in README.md
- ✅ Published to NuGet

---

## Timeline Estimate

| Task | Effort | Dependencies |
|------|--------|--------------|
| Models & DTOs | 1 day | None |
| Core exporters (10 services) | 5 days | Models |
| Orchestration service | 1 day | Exporters |
| Dashboard UI | 2 days | Orchestration |
| Dashboard backend | 1 day | Orchestration |
| Version compatibility | 2 days | Exporters |
| Testing | 3 days | All implementation |
| Documentation | 1 day | Implementation complete |
| **Total** | **~16 days** | |

---

## Next Steps

01. ✅ Create project structure following Yaml2Schema baseline
02. ✅ Implement Models (ExportModels.cs)
03. ✅ Implement DataTypeExporter (simplest, no dependencies)
04. ✅ Implement DocumentTypeExporter
05. ✅ Implement remaining exporters
06. ✅ Implement SchemaExportService orchestrator
07. ✅ Create Dashboard UI
08. ✅ Implement version detection
09. ✅ Write comprehensive tests
10. ✅ Complete documentation
11. ✅ Publish to NuGet


## Important
- NuGet API key: store in a secure secrets manager (e.g., environment variable or GitHub Actions secret), never commit to source control