# SplatDev.Umbraco.Plugins.Schema2Yaml

[![NuGet](https://img.shields.io/nuget/v/SplatDev.Umbraco.Plugins.Schema2Yaml.svg)](https://www.nuget.org/packages/SplatDev.Umbraco.Plugins.Schema2Yaml)
[![NuGet Downloads](https://img.shields.io/nuget/dt/SplatDev.Umbraco.Plugins.Schema2Yaml.svg)](https://www.nuget.org/packages/SplatDev.Umbraco.Plugins.Schema2Yaml)
[![CI](https://github.com/SplatDev-Ltda/umbraco-yaml/actions/workflows/ci.yml/badge.svg)](https://github.com/SplatDev-Ltda/umbraco-yaml/actions/workflows/ci.yml)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

A migration and Infrastructure-as-Code tool for **Umbraco 13–17** that exports your entire Umbraco site structure to YAML format. Export once, version-control it, and import anywhere using the companion [SplatDev.Umbraco.Plugins.Yaml2Schema](https://www.nuget.org/packages/SplatDev.Umbraco.Plugins.Yaml2Schema) plugin.

> **Umbraco 13 users:** install [v1.x](https://www.nuget.org/packages/SplatDev.Umbraco.Plugins.Schema2Yaml/1.0.10) of this package.

Perfect for:
- **Migrating** between Umbraco versions (14 → 15 → 16 → 17)
- **Site cloning** and environment bootstrapping
- **Version control** of your Umbraco structure
- **Backup** and documentation
- **Team collaboration** with declarative site definitions

---

## Umbraco Version Compatibility

| Umbraco | .NET | Dashboard | NuGet package |
|---------|------|-----------|---------------|
| 13.x | net8.0 | AngularJS dashboard | [v1.x](https://www.nuget.org/packages/SplatDev.Umbraco.Plugins.Schema2Yaml/1.0.10) |
| 14.x | net8.0 | Lit web component | v2.x (latest) |
| 15.x | net9.0 | Lit web component | v2.x (latest) |
| 16.x | net9.0 | Lit web component | v2.x (latest) |
| 17.x | net10.0 | Lit web component | v2.x (latest) |

The v2.x NuGet package ships **all three TFMs** (net8.0 / net9.0 / net10.0) in a single package — NuGet automatically selects the right one at install time.

---

## Features

### Complete Schema Export
- ✅ **DataTypes** — Property editors with full configuration
- ✅ **DocumentTypes** — Content types with all properties, tabs, compositions, and templates
- ✅ **Media Types** — Media types with properties and folder structure
- ✅ **Templates** — Razor templates with full content
- ✅ **Languages** — Multi-language configuration
- ✅ **Dictionary Items** — Translations for all languages with parent-child hierarchy

### Content & Media Export
- ✅ **Content** — All content nodes with properties, sort order, and published state
- ✅ **Media** — Media items with automatic file download to local folder
- ✅ **Media Files** — Physical files downloaded and organized in Umbraco folder structure

### User Management Export
- ✅ **Members** — Member accounts with properties and groups (passwords excluded)
- ✅ **Users** — Back-office users with roles (opt-in via config, passwords excluded)

### Export Features
- 📦 **Single ZIP Download** — All YAML + media bundled in one archive
- 📊 **Export Statistics** — Live counts per entity type with duration
- 🖥️ **Dashboard UI** — Settings section dashboard (AngularJS for Umbraco 13, Lit web component for Umbraco 14–17)
- 🔄 **Version Detection** — Automatic API compatibility for Umbraco 13–17
- 📁 **Folder Structure** — Media files maintain Umbraco's original folder hierarchy

---

## Installation

```bash
dotnet add package SplatDev.Umbraco.Plugins.Schema2Yaml
```

Or via NuGet Package Manager:

```
Install-Package SplatDev.Umbraco.Plugins.Schema2Yaml
```

No further registration is required — the plugin self-registers via `Schema2YamlComposer` on startup and the dashboard appears automatically in the **Settings** section.

### Pack a themed NuGet package (metadata-only)

By default, packing produces the standard package `SplatDev.Umbraco.Plugins.Schema2Yaml`.

To produce a themed variant from the same project, set pack properties:

```bash
dotnet pack SplatDev.Umbraco.Plugins.Schema2Yaml/SplatDev.Umbraco.Plugins.Schema2Yaml.csproj -c Release -p:Schema2YamlThemePackage=true -p:Schema2YamlThemeName=Corporate
```

This generates package id:

`SplatDev.Umbraco.Plugins.Schema2Yaml.Corporate`

Notes:
- This only changes NuGet metadata (PackageId, Description, Tags).
- One theme package is produced per pack run.
- If `Schema2YamlThemePackage=false` (default), behavior is unchanged.

### Publish themed package via GitHub Actions

Push a tag using this format:

`schema2yaml-theme-<ThemeName>-v<Version>`

Example:

`schema2yaml-theme-Corporate-v2.0.6`

The publish workflow will automatically:
- detect `<ThemeName>` from the tag
- pack with `Schema2YamlThemePackage=true`
- pack with `Schema2YamlThemeName=<ThemeName>`
- publish the themed package to NuGet

---

## Usage

### Via Dashboard (Recommended)

1. Navigate to the **Settings** section in the Umbraco back-office
2. Click the **Schema Export** dashboard tab
3. Click **Export to YAML** to generate YAML from your current site
4. Review the export statistics and YAML preview
5. Click **Download YAML** to save as a single file, or
6. Click **Download ZIP** to get YAML + all media files bundled together

The ZIP archive structure:
```
umbraco-export.zip
├── umbraco.yml           # Complete schema + content file
└── media/                 # Media files in Umbraco folder structure
    ├── images/
    │   └── logo.png
    └── documents/
        └── brochure.pdf
```

### Via API (Programmatic)

```csharp
using SplatDev.Umbraco.Plugins.Schema2Yaml.Services;

public class MyController : Controller
{
    private readonly ISchemaExportService _exportService;

    public MyController(ISchemaExportService exportService)
    {
        _exportService = exportService;
    }

    public async Task<IActionResult> ExportSchema()
    {
        // Export to YAML string
        var yaml = await _exportService.ExportToYamlAsync();

        // Export to file on disk
        await _exportService.ExportToFileAsync("exports/umbraco.yml");

        // Export to ZIP with media files
        var zipBytes = await _exportService.ExportToZipAsync();
        return File(zipBytes, "application/zip", "umbraco-export.zip");
    }
}
```

### REST API Endpoints

The plugin exposes authenticated endpoints under `/umbraco/api/SchemaExport/`:

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/Export` | Export and return `{ yaml, statistics }` as JSON |
| GET | `/DownloadYaml` | Stream YAML as a file download |
| GET | `/DownloadZip` | Stream ZIP (YAML + media) as a file download |
| GET | `/Statistics` | Return statistics from the last export |

All endpoints require the **Settings** section access policy.

### Configuration

Override defaults in `appsettings.json`:

```json
{
  "UmbracoSchema2Yaml": {
    "ExportPath": "exports/umbraco.yml",
    "IncludeMedia": true,
    "MediaPath": "exports/media",
    "IncludeContent": true,
    "IncludeMembers": true,
    "IncludeUsers": false,
    "CompressYaml": false,
    "MaxHierarchyDepth": 50
  }
}
```

| Setting | Default | Description |
|---------|---------|-------------|
| `ExportPath` | `exports/umbraco.yml` | Default YAML export file path |
| `IncludeMedia` | `true` | Include media items and file downloads |
| `MediaPath` | `exports/media` | Media files download location |
| `IncludeContent` | `true` | Include content nodes |
| `IncludeMembers` | `true` | Include member accounts |
| `IncludeUsers` | `false` | Include back-office users (security consideration) |
| `CompressYaml` | `false` | Minimise YAML output |
| `MaxHierarchyDepth` | `50` | Max depth for content/media tree traversal |

---

## Generated YAML Structure

```yaml
umbraco:
  languages:
    - isoCode: en-US
      cultureName: English (United States)
      isDefault: true
      isMandatory: true

  dataTypes:
    - alias: pageTitle
      name: Page Title
      editorUiAlias: Umbraco.TextBox
      config:
        maxLength: 100

  documentTypes:
    - alias: homePage
      name: Home Page
      icon: icon-home
      allowedAsRoot: true
      allowedTemplates:
        - homePage
      defaultTemplate: homePage
      tabs:
        - name: Content
          properties:
            - alias: pageTitle
              name: Page Title
              dataType: Page Title
              mandatory: true

  templates:
    - alias: homePage
      name: Home Page
      content: |
        @inherits Umbraco.Cms.Web.Common.Views.UmbracoViewPage
        @{ Layout = "_Layout.cshtml"; }
        <h1>@Model.Value("pageTitle")</h1>

  content:
    - name: Home
      documentType: homePage
      template: homePage
      published: true
      properties:
        pageTitle: Welcome to My Site

  dictionaryItems:
    - key: Welcome
      translations:
        en-US: Welcome
        es-ES: Bienvenido

  members:
    - name: John Doe
      email: john@example.com
      username: johndoe
      memberType: Member
      isApproved: true
```

---

## Migration Workflow

```
Source Site (Umbraco 13–17)
       │
       ▼  Schema2Yaml (this plugin)
  umbraco.yml + media files
       │
       ▼  Git / file transfer
Target Site (Umbraco 13–17)
       │
       ▼  Yaml2Schema companion plugin
  Replicated Site Structure
```

### What transfers
✅ All structure (types, templates, languages)  
✅ All content and media  
✅ Property values and data type configurations  
✅ Parent-child relationships and sort order  
✅ Multi-language content  
✅ Dictionary translations  

### What doesn't transfer
❌ User/member passwords (require reset on import)  
❌ Package-specific data (packages must be reinstalled)  
❌ Database IDs (new GUIDs generated on import)  
❌ Content version history (only latest published version)  

---

## Best Practices

### Security
- ⚠️ **Passwords are never exported** — members and users require a password reset after import
- ⚠️ **Review before committing** — check for sensitive property values before adding YAML to version control
- ✅ **Add `exports/` to `.gitignore`** — prevent accidental commits of local export files
- ✅ **`IncludeUsers` is off by default** — opt in explicitly if you need back-office user export

### Performance
- For **large sites** (1 000+ content nodes): export may take several minutes
- Disable media export for **schema-only** exports: `"IncludeMedia": false`
- Disable content export for **structure-only** exports: `"IncludeContent": false`

### Team Collaboration
- Commit `umbraco.yml` to version control for shared environments
- Don't commit media files unless the library is small — use a CDN or shared storage instead
- Use `Yaml2Schema` on local/staging environments to bootstrap from the shared YAML

---

## Troubleshooting

### Export is slow
Disable media and/or content export in `appsettings.json` for schema-only exports.

### Media files not downloading
Check file permissions on the `wwwroot/media` folder and verify there is enough disk space for ZIP creation.

### YAML import fails on target site
- Ensure the Yaml2Schema version supports the target Umbraco version
- Verify all required packages/property editors are installed on the target site

---

## Roadmap

- [x] Core export — all 10 entity types
- [x] Dashboard UI — AngularJS (Umbraco 13) + Lit web component (Umbraco 14–17)
- [x] ZIP download with media
- [x] Multi-version support (Umbraco 13–17, net8/9/10)
- [x] CI/CD — GitHub Actions for build, test, and NuGet publish
- [ ] Selective export (choose specific types or subtrees)
- [ ] Incremental exports (only changes since last export)
- [ ] Export scheduling and automation
- [ ] CLI tool for CI/CD pipeline integration
- [ ] Cloud storage export (Azure Blob, S3)

---

## Contributing

1. Fork the repository at [github.com/SplatDev-Ltda/umbraco-yaml](https://github.com/SplatDev-Ltda/umbraco-yaml)
2. Create a feature branch
3. Follow the existing code style (see companion `Yaml2Schema` plugin as baseline)
4. Add unit tests for new functionality
5. Submit a pull request

---

## Support

- **Issues**: [GitHub Issues](https://github.com/SplatDev-Ltda/umbraco-yaml/issues)
- **Repository**: [github.com/SplatDev-Ltda/umbraco-yaml](https://github.com/SplatDev-Ltda/umbraco-yaml)
- **Companion plugin**: [SplatDev.Umbraco.Plugins.Yaml2Schema](https://www.nuget.org/packages/SplatDev.Umbraco.Plugins.Yaml2Schema)

---

## License

MIT — see [LICENSE](https://github.com/SplatDev-Ltda/umbraco-yaml/blob/master/LICENSE) for details.

---

Developed by **[SplatDev Ltda](https://github.com/SplatDev-Ltda)**
