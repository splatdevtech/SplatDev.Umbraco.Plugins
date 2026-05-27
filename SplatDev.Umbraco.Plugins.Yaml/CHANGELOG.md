# Changelog

All notable changes to `SplatDev.Umbraco.Plugins.Yaml2Schema` are documented here.

The format follows [Keep a Changelog](https://keepachangelog.com/en/1.0.0/).
This project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

---

## [Unreleased]

## [1.0.32] - 2026-04-03

### Changed

#### Auto-restart after package installation

When `PackageInstaller` newly downloads and loads one or more assemblies, the application now
automatically restarts via `IHostApplicationLifetime.StopApplication()`. The remaining YAML
initialization is skipped for this boot — on the next startup the packages are already cached,
no restart occurs, and the full initialization (DataTypes, DocumentTypes, Content, etc.) runs
normally with the newly registered DI services / `IComposer` implementations in place.

```
Boot 1: package not loaded → download → Assembly.LoadFrom → StopApplication() → restart
Boot 2: assembly already in AppDomain → skip install → full YAML init runs ✓
```

## [1.0.31] - 2026-04-03

### Added

#### `install: true` — automatic NuGet package download and assembly loading

Packages declared in the `packages:` section can now be automatically downloaded and loaded into the running AppDomain:

```yaml
umbraco:
  packages:
    - id: Examine
      version: 3.0.0
      install: true
    - id: Our.Umbraco.Community.SomePlugin
      version: 2.0.0
      required: true
      install: true
      source: https://my-private-feed/v3/index.json  # optional; defaults to NuGet.org
```

**How it works:**

- New `PackageInstaller` service runs before `PackageValidator` at startup
- Skips packages already present in the AppDomain
- Queries the NuGet v3 flat-container API (no `NuGet.Protocol` dependency — the `.nupkg` is just a ZIP)
- Resolves the latest stable version automatically when `version:` is omitted
- Extracts the best-matching TFM DLLs (`net10.0` → `net9.0` → `net8.0` → … → `netstandard2.0`) to `packages/{id}.{version}/` under the content root
- Loads each DLL with `Assembly.LoadFrom`
- Supports private NuGet feeds via `source:` (service index or flat-container URL)

**Limitation:** Assemblies loaded at runtime will not have their DI / `IComposer` registrations executed. A full Umbraco package install (with DI wiring) requires a restart after the first run — on the second start the package is already cached and pre-loaded.

## [1.0.30] - 2026-04-03

### Added

#### `modelsBuilder:` — configure and generate published models from YAML

Two new Phase 4 capabilities controlled by a new top-level `modelsBuilder:` section:

```yaml
umbraco:
  modelsBuilder:
    outputPath: Models/GeneratedModels   # relative to content root, or absolute
    mode: SourceCodeManual               # InMemoryAuto | SourceCodeAuto | SourceCodeManual | Nothing
```

**T1 — `ModelsBuilderConfigurator`:** At startup the plugin writes the specified values into `appsettings.json` under `Umbraco:CMS:ModelsBuilder` (`ModelsDirectoryAbsolute`, `ModelsMode`). Only fields that are explicitly set in the YAML are touched; omitted fields are left unchanged. The path is resolved relative to the content root when not absolute.

**T2 — `PublishedModelsGenerator`:** When `modelsBuilder:` is present, the plugin generates typed C# partial classes (one `.generated.cs` file per document type) in the configured `outputPath`. Each class:
- Is decorated with `[PublishedModel("alias")]`
- Inherits `PublishedContentModel`
- Exposes a typed, lazily-evaluated property per YAML-declared property (editor alias → C# type mapping covers all built-in Umbraco editors)

#### `mediaDefaultFolder:` — section-level default folder for media uploads

```yaml
umbraco:
  mediaDefaultFolder: Images/Uploads
  media:
    - name: Hero Image
      mediaType: Image
      url: https://example.com/hero.jpg
      # no per-item folder needed — inherits mediaDefaultFolder
```

A new `mediaDefaultFolder:` field on the `umbraco:` root acts as a fallback folder for any `media:` item that does not specify its own `folder:`. Per-item `folder:` always takes precedence. The folder path is created automatically if it does not exist.

### Fixed

- Pre-existing test regression: `ContentCreatorTests` and `IntegrationTests` were calling `ContentCreator` without the `IMediaService` parameter added in v1.0.29. Both test files updated.
- `ContentCreatorTests.CreateContent_ShouldUpdateExistingContent` was failing with a `NullReferenceException` because the mock `IContent` node had no `TemplateId` set up, causing the v1.0.28 template-restoration branch to dereference a null `ContentType`. Fixed by stubbing `TemplateId = 1` in the test.

## [1.0.29] - 2026-04-02

### Fixed

#### Media is now seeded before Content

The initialization order was: Content → Media. Any content property using `Umbraco.MediaPicker3` with a media name string (e.g. `heroImage: "My Image"`) would silently fail because the media item did not exist yet when `CoerceValue` ran its lookup. New order: **Media → Content**.

## [1.0.28] - 2026-04-02

### Fixed

#### Content nodes now receive the document type's default template on creation

`IContentService.Create` does not auto-assign the document type's default template. Content nodes were being saved with `TemplateId = null`, making them unrenderable. Fix: the plugin now explicitly sets `content.TemplateId = contentType.DefaultTemplateId` after `Create` when the document type has a default template.

The `[UPDATE]` path also restores a missing template — content nodes created before this fix will have their template backfilled on the next update pass.

## [1.0.27] - 2026-04-02

### Added

#### `Umbraco.MediaPicker3` property — resolve media by name from YAML

Properties using `Umbraco.MediaPicker3` can now be seeded with a media item name string. The plugin looks up the media item by name (case-insensitive) and generates the correct `[{"key":"...","mediaKey":"...","crops":[],"focalPoint":null}]` JSON reference automatically.

```yaml
properties:
  heroImage: "RISIN – Home Hero"   # name of an Umbraco media item
```

### Fixed

#### `Umbraco.DropDown.Flexible` — plain string seeded value no longer causes `JsonException`

`FlexibleDropdownPropertyValueConverter` expects values stored as a JSON array `["selectedValue"]`. Seeding a plain string such as `"Commerce"` caused a runtime `JsonException: 'C' is an invalid start of a value`. Fix: `CoerceValue` now wraps plain string values in a JSON array for `Umbraco.DropDown.Flexible` properties before saving.

## [1.0.26] - 2026-04-02

### Fixed

#### Media image files now stored in ImageCropper JSON format

- Root cause: `TryAttachFileFromUrl` called `media.SetValue(umbracoFile, url)` with a plain URL string. The `Image` media type uses `Umbraco.ImageCropper` which expects a JSON object `{"src":"...","focalPoint":{"left":0.5,"top":0.5},"crops":[]}`. Saving a plain URL meant the backoffice showed no image.
- Fix: `TryAttachFileFromUrl` now detects `Image` media type (via `media.ContentType.Alias`) and serializes the ImageCropper JSON. All other types (`File`, `Video`, etc.) continue to store a plain URL path.

#### `[UPDATE]` media items now re-download their file from `url`

- The `[UPDATE]` path for media called only `SetProperties` and never `TryAttachFileFromUrl`, so media items with a `url` field were always saved without a file when updated.
- Fix: `TryAttachFileFromUrl` is now called in the UPDATE path whenever a `url` is provided.

## [1.0.25] - 2026-04-02

### Fixed

#### `Umbraco.MultipleTextstring` list values now stored as newline-delimited text

- Root cause of "all items collapsed into one entry" when seeding a `multipleTextString` property with a YAML list: `Umbraco.MultipleTextstring` stores its value as `\n`-separated plain text, not as JSON. The generic `NormaliseForJson` + `JsonSerializer.Serialize` fallback produced a JSON array, which Umbraco read back as a single string.
- Fix: `CoerceValue` now detects `Umbraco.MultipleTextstring` properties with a `List<object>` value and joins the items with `\n` before saving.

```yaml
properties:
  revenueSources:
    - "Recurring business revenue (rent, fees, commissions)."
    - "Regional and national advertising (sponsorship plans)."
    - "Integrated digital services (cooperatives, guides, media)."
```

---

## [1.0.24] - 2026-04-02

### Added

#### `Umbraco.MultiUrlPicker` mapping syntax — set link title from YAML

URL picker seed values can now be supplied as a YAML mapping with `url`, `title` (or `name`), and optional `target` fields. The `title` value is saved as the link's display name inside the MultiUrlPicker JSON:

```yaml
properties:
  heroPrimaryCtaUrl:
    url: "/parceiros"
    title: "Join as a Partner"
    target: "_blank"   # optional
```

Produces: `[{"name":"Join as a Partner","target":"_blank","udi":null,"url":"/parceiros","queryString":""}]`

Both `title` and `name` are accepted as the display-name key (`title` takes precedence). Plain string values (`url: "/path"`) continue to work as before.

---

## [1.0.23] - 2026-04-02

### Fixed

#### `Umbraco.MultiUrlPicker` seed values now serialised correctly from plain URL strings

- Root cause of `'/' is an invalid start of a value` errors during content seeding: `Umbraco.MultiUrlPicker` stores its value as a JSON array (`[{"url":"/path",...}]`), but seed data typically supplies a plain string (e.g. `"/parceiros"`). `CoerceValue` passed the raw string through and Umbraco's property value converter tried to deserialise it as JSON, hitting the parse error.
- Fix: `CoerceValue` now detects `Umbraco.MultiUrlPicker` properties and, when the incoming value is a non-empty string that does not already start with `[`, wraps it in the single-link JSON array format: `[{"name":"","target":"","udi":null,"url":"<value>","queryString":""}]`. Values already in JSON format (starting with `[`) are passed through unchanged.

---

## [1.0.22] - 2026-04-02

### Added

#### `Umbraco.SingleBlock` support

- **DataType config**: `DataTypeCreator.LinkBlockListElementTypes` now handles `Umbraco.SingleBlock` data types. For Single Block, `contentElementTypeAlias` sits at the top level of the config (not inside a `blocks` list). The alias is resolved to the element type's GUID and the DataType is re-saved — no more hard-coded GUIDs required.
- **Content seeding**: `ContentCreator.CoerceValue` now detects a YAML mapping with a `$type` key as a Single Block value and serialises it to the correct Umbraco Single Block JSON format. The `layout` key uses an object (`Umbraco.SingleBlock: { contentUdi: ... }`) rather than an array, matching the Umbraco Single Block storage contract.

```yaml
dataTypes:
  - alias: presidentBlock
    name: President Block
    editorUiAlias: Umbraco.SingleBlock
    valueType: NTEXT
    config:
      contentElementTypeAlias: corpPositionElement

content:
  - alias: home
    documentType: homePage
    properties:
      president:
        $type: corpPositionElement
        title: "Presidency"
        text:  "Strategic vision and institutional representation."
```

### Fixed

#### Media file downloads now store files under the correct path

- `MediaCreator.TryAttachFileFromUrl` previously stored downloaded files at the filesystem root with no path prefix (`AddFile(fileName, ...)`) and set the `umbracoFile` property to the bare filename, resulting in broken media references.
- Fix: files are now stored under a media-key-based subfolder (`{mediaKey:N}/{fileName}`), matching Umbraco's expected path convention. `FileSystem.GetUrl(filePath)` is used to derive the public-facing URL stored in the property.

---

## [1.0.21] - 2026-04-02

### Added

#### `isElement` field on Document Types

- `YamlDocumentType` model gains an `isElement` boolean field (default `false`).
- `DocumentTypeCreator` applies `IsElement` on both the **create** path (new `ContentType` constructor) and the **update** path (`UpdateDocumentType`).
- Element types are required for Block List, Block Grid, and Single Block blocks. Previously, element types had to be flagged via a manual post-startup fixup; they can now be declared directly in YAML:

```yaml
documentTypes:
  - alias: cardElement
    name: Card
    icon: icon-item
    isElement: true
    tabs:
      - name: Content
        properties:
          - alias: title
            name: Title
            dataType: textString
```

---

## [1.0.20] - 2026-04-01

### Changed

- Internal maintenance release — version bump to align with dependency updates. No functional changes.

---

## [1.0.19] - 2026-04-01

### Added

#### Package validation (`packages:`)

Declare NuGet packages your site depends on. At startup the plugin checks whether each package's assembly is loaded in the current AppDomain and logs a warning (optional) or error (required) if it is missing. No installation is performed — add missing packages to your `.csproj` manually.

```yaml
packages:
  - id: Our.Umbraco.Community.SomePlugin
    version: "2.0.0"
    required: true
  - id: Another.Plugin
    assemblyName: Another.Plugin.Core   # override when assembly name differs from package ID
```

#### Custom property editors (`propertyEditors:`)

Define frontend-only (Umbraco 14+ Web Component) property editors entirely in YAML. The plugin writes an `App_Plugins/[folderName]/umbraco-package.json` manifest and, when `jsContent` is provided, the corresponding JavaScript file.

```yaml
propertyEditors:
  - alias: My.CustomTextEditor
    name: "My Custom Text Editor"
    icon: icon-code
    group: common
    jsContent: |
      customElements.define('my-custom-editor', class extends HTMLElement {
        // ...
      });
```

For DataType definitions referencing a custom editor, add `valueType` so the plugin can fall back to a compatible built-in server-side editor:

```yaml
dataTypes:
  - alias: myCustomEditorDT
    name: "My Custom Editor DT"
    editorUiAlias: My.CustomTextEditor
    valueType: NVARCHAR   # required for frontend-only editors
```

#### `valueType` field on DataType

New optional field on `dataTypes` entries. Accepted values: `NVARCHAR` (default), `NTEXT`, `TEXT`, `INT`, `INTEGER`, `BIGINT`, `DECIMAL`, `DATE`. When set, this overrides the storage type derived from the server-side editor. Required when `editorUiAlias` points to a custom frontend-only editor (no server-side `IDataEditor` registration).

## [1.0.18] - 2026-04-01

### Added

#### Block List support: `contentElementTypeAlias` resolution and `$type` content seeding

- **DataType config**: Block List (and Block Grid) DataType configs may now use `contentElementTypeAlias` instead of `contentElementTypeKey` in the `blocks` array. The new `DataTypeCreator.LinkBlockListElementTypes()` pass runs after DocumentTypes are created and resolves each alias to the actual content element type GUID, then re-saves the DataType. This means you no longer need to hard-code GUIDs in your YAML.
- **Content seeding**: Properties whose value is a YAML list of mappings with a `$type` key are automatically serialised to the Umbraco Block List JSON format. The `$type` value is the element document-type alias; all other keys become property values on the block.
  ```yaml
  properties:
    pilares:
      - $type: pilarElement
        title: "Pilar 1"
        text:  "Texto do pilar 1"
      - $type: pilarElement
        title: "Pilar 2"
        text:  "Texto do pilar 2"
  ```
- **Generic complex-value serialisation**: Any content property stored as `Ntext` whose YAML value is a list or mapping (but without `$type`) is now serialised to a JSON string automatically. This handles tags, multi-url pickers, and other editors that store JSON.
- New `LinkBlockListElementTypes()` step added to `YamlInitializationHandler` (runs after `LinkTemplatesToDocumentTypes`).

#### Media folder support

- Added `folder` field to `YamlMedia`. Accepts a simple name (`"Images"`) or a slash-separated path (`"Images/Partners"`). The folder hierarchy is created automatically if it does not exist; the media item is placed inside the deepest folder.
  ```yaml
  media:
    - name: Logo Partner A
      mediaType: Image
      folder: "Images/Partners"
      url: https://example.com/logo-a.png
  ```
- New `MediaCreator.EnsureFolder()` helper creates `Folder` media nodes recursively and is idempotent (skips existing folders).

## [1.0.17] - 2026-04-01

### Fixed

#### `DocumentTypeCreator.LinkTemplatesToDocumentTypes` no longer clears existing template assignments when resolution fails
- Root cause of "all pages with sub-pages/children have lost the template assignment": when a template alias could not be resolved (e.g. due to a YAML scalar-continuation bug producing an invalid alias such as `"ecossistema - ecosystemWebsite"`), `resolvedTemplates` was empty. Assigning `contentType.AllowedTemplates = resolvedTemplates` (an empty list) then cleared every previously-assigned template from the document type and saved that destructive state.
- Fix: if no templates could be resolved for a document type, a warning is logged and the method skips the save entirely, preserving the existing template assignments on the document type.

## [1.0.16] - 2026-03-31

### Fixed

#### `DataTypeCreator` now sets `EditorUiAlias` so the Umbraco 17 backoffice can resolve the property editor UI
- Root cause of "The configured property editor UI could not be found" backoffice errors: `DataType.EditorUiAlias` was never set explicitly, so it defaulted to the server-side `EditorAlias` (e.g. `Umbraco.TextBox`). Umbraco 17's new Vite backoffice looks up the property editor Web Component by `EditorUiAlias` and expects the new `Umb.PropertyEditorUi.*` format (e.g. `Umb.PropertyEditorUi.TextBox`), not the legacy `Umbraco.*` format.
- Added a static lookup table (`_editorUiAliasMap`) mapping every built-in Umbraco 17.2.2 schema alias to its correct `Umb.PropertyEditorUi.*` UI alias, extracted directly from the Umbraco 17.2.2 backoffice manifests (`defaultPropertyEditorUiAlias` field). Covers 35 built-in editors including TextBox, TextArea, RichText (→Tiptap), Dropdown, CheckBoxList, Toggle, MediaPicker, BlockList, BlockGrid, and more.
- Added `ResolveEditorUiAlias(string editorAlias)` helper — falls back to the raw alias for third-party editors not in the map.
- `EditorUiAlias` is now set in both the **create** path and the **update** path (`update: true`), ensuring both newly-created and force-updated DataTypes have the correct UI alias.

## [1.0.15] - 2026-04-01

### Fixed

#### Dropdown / CheckBoxList `items` stored as `List<string>` for Umbraco validator in `DataTypeCreator`
- Root cause of all "Invalid data type configuration" errors for `Umbraco.DropDown.Flexible` and similar editors: Umbraco 17's `ValueListConfigurationEditor` performs a strict `is List<string>` check. YamlDotNet deserialises YAML string sequences as `List<object>`, so even though every element was a string, the type check failed and the DataType was saved with invalid config — causing `IDataTypeService.GetDataType` to return null for those types.
- Consolidated the two previous helpers (`IsValueListConfig` / `NormalizeConfig`) into a single `ApplyConfig` method that detects a plain string list under `items` and converts it from `List<object>` to `List<string>` in-place before `SetConfigurationData` is called.
- Applied to both the **create** and **update** paths.

## [1.0.14] - 2026-03-31

### Fixed

#### `DataTypeCreator` UPDATE path now re-applies config and DatabaseType
- When `update: true` is set and a DataType already exists in the database, the UPDATE path previously did nothing ("no structural update required") and skipped — leaving stale entries with incorrect config or wrong `DatabaseType` untouched.
- Fix: the UPDATE path now re-looks up the property editor, re-derives the `DatabaseType` from `editor.GetValueEditor().ValueType` (same logic as creation), re-applies the `config` dict (respecting the `IsValueListConfig` fix from v1.0.13), and re-saves via `IDataTypeService.Save`.
- This corrects: (a) dropdown DataTypes left with `{id,value}` item format from pre-v1.0.13 runs; (b) rich text / text area DataTypes left with `Nvarchar` storage from pre-v1.0.11 runs.
- Cast from `IDataType` to concrete `DataType` is required to call `SetConfigurationData`; graceful skip added for non-concrete instances.

## [1.0.13] - 2026-03-31

### Fixed

#### Dropdown `items` config format corrected in `DataTypeCreator`
- `ValueListConfiguration.Items` in Umbraco 17 is `List<string>`, not a list of `{ id, value }` objects.
- The `NormalizeConfig` helper introduced in v1.0.8 was converting plain string items to `[{ id: N, value: "..." }]` dicts — the wrong shape — causing `InvalidDataTypeConfiguration` validation errors on every dropdown DataType (`Umbraco.DropDown.Flexible`, checkboxlist, etc.) and intermittent `GetDataType` null returns that cascaded into missing properties on DocumentTypes.
- Fix: added `IsValueListConfig` predicate; when the `items` key holds a plain string list, `NormalizeConfig` is skipped and the raw `{ "items": ["val1", "val2"] }` dict is passed directly to `SetConfigurationData` — matching the actual `List<string>` structure Umbraco 17 expects.
- Removed the broken `TryBuildValueListConfiguration` approach (which attempted `dataType.Configuration = ...` and `dataType.ConfigurationObject = ...` — both inaccessible in Umbraco 17).

## [1.0.12] - 2026-03-31

### Fixed

#### Boolean/numeric property coercion in `ContentCreator`
- YamlDotNet deserialises all YAML scalar values (including `true`/`false`) as `System.String` when the target property type is `object`.
- Umbraco's `Property.SetValue` for checkbox (`Umbraco.TrueFalse`) and other integer-backed editors expects `System.Int32`, causing `Cannot assign value "true" of type System.String to property expecting System.Int32`.
- Added `CoerceValue` helper: for `ValueStorageType.Integer` properties, `"true"`→`1`, `"false"`→`0`, numeric strings parsed to `int`; for `ValueStorageType.Decimal` properties, numeric strings parsed with invariant culture. Applied on both create and update paths.

## [1.0.11] - 2026-03-31

### Fixed

#### DataType database storage type derived from editor in `DataTypeCreator`
- `DatabaseType` was hardcoded to `ValueStorageType.Nvarchar` for every DataType, causing SQL truncation errors (`String or binary data would be truncated in column 'varcharValue'`) when seeding content into properties backed by rich text or other long-value editors.
- Now uses `DataType.GetDatabaseType(editor.GetValueEditor().ValueType)` so each DataType gets the correct storage type: `Ntext` for rich text / markdown, `Nvarchar` for short text, `Integer`/`Decimal` for numeric editors, etc.

## [1.0.10] - 2026-03-31

### Fixed

#### Null-safe collection guards in `DocumentTypeCreator`, `ContentCreator`, and `MediaCreator`
- YamlDotNet can override `= new()` property initializers with `null` when deserializing YAML that omits or explicitly nulls a list field.
- `AllowedChildTypes.Any()` in both the create and update paths of `DocumentTypeCreator` now uses `?.Any() == true`.
- `Tabs` foreach in both create and update paths now uses `?? []` to avoid null iteration.
- `Children.Any()` in `ContentCreator` and `MediaCreator` (all three call sites in each) use `?.Any() == true`.
- Previously these threw `ArgumentNullException` from `Enumerable.Any` whenever a DocumentType, content node, or media node omitted a list property.

## [1.0.9] - 2026-03-31

### Fixed

#### True upsert behaviour for `update: true` entities in `DataTypeCreator`, `DocumentTypeCreator`, and `TemplateCreator`
- When `update: true` is set and the entity **does not yet exist**, all three creators now fall through to the creation path instead of logging "not found, skipping" and calling `continue`.
- Previously, every entity flagged with `update: true` was unconditionally skipped on a fresh database (first install), meaning all 20/21 DocumentTypes, all 13 Templates, and any DataType with `update: true` were never created — causing cascading failures in content seeding.
- Now `update: true` behaves as a true upsert: **update if exists, create if not found**.

## [1.0.8] - 2026-03-31

### Fixed

#### Dropdown `items` config normalisation in `DataTypeCreator`
- Added `NormalizeConfig` helper that converts a plain string list under the `items` key into the `[{ id, value }]` format required by `Umbraco.DropDown.Flexible` (and any other editor using `ValueListConfiguration`).
- Previously, YAML `items: [- Foo, - Bar]` was passed as `List<object>` of strings, causing Umbraco to throw `The configuration value ... is not a valid value list configuration` for every dropdown DataType, leaving them unsaved and cascading into `DataType not found` warnings in DocumentTypeCreator.

## [1.0.7] - 2026-03-31

### Fixed

#### DataType alias-to-name resolution in `DocumentTypeCreator`
- `CreateDocumentTypes` now accepts an optional `List<YamlDataType>` parameter and builds an alias→name lookup map from it.
- Previously, `GetDataType(property.DataType)` was called with the YAML alias (e.g. `imagePicker`) but the DataType was stored in Umbraco under its display name (e.g. `Image Picker`), causing 100% property-skipping for any DataType whose alias differed from its name.
- `UpdateDocumentType` receives the same map so the fix applies to both create and update paths.
- The handler passes `yamlRoot.Umbraco.DataTypes` when calling `CreateDocumentTypes`.

#### DataType existence check in `DataTypeCreator`
- Replaced the `GetByEditorAlias` existence check with `GetDataType(name)`.
- The old check skipped creating a custom DataType (e.g. `imagePicker`) if *any* Umbraco built-in DataType shared the same property editor alias (`Umbraco.MediaPicker3`), preventing custom configs from ever being applied.

## [1.0.6] - 2026-03-31

### Fixed

#### Runtime-level guard in `YamlInitializationHandler`
- Handler now checks `IRuntimeState.Level == RuntimeLevel.Run` before executing any initialization logic.
- Previously, the handler fired during the Umbraco installer (before the database schema existed), causing `Invalid object name 'umbracoLanguage'` SQL exceptions and cascading failures (`DataType` editor aliases not found, templates not written, `DocumentType` linking skipped).
- When the runtime level is not `Run` (i.e., `Install` or `Upgrade`), the handler logs an informational message and exits immediately without touching the database.

## [1.0.5] - 2026-03-31

### Added

#### Template Assignment on Document Types (`allowedTemplates` / `defaultTemplate` fields)
- `YamlDocumentType` now supports two new optional fields: `allowedTemplates` (list of template aliases) and `defaultTemplate` (single alias string).
- `DocumentTypeCreator` gains `ITemplateService` injection and a new `LinkTemplatesToDocumentTypes` method that resolves template aliases and assigns `AllowedTemplates` / `SetDefaultTemplate` on the content type.
- Template linking runs as a dedicated step in `YamlInitializationHandler` immediately **after** `CreateTemplates`, ensuring templates exist in the database before the link is attempted.
- Unresolved template aliases are logged as warnings and skipped rather than throwing.

## [1.0.3] - 2026-03-30

### Added

#### Languages (`languages` section)
- New top-level `languages` section for declaring Umbraco languages.
- Each entry supports `isoCode`, `cultureName`, `isDefault`, `isMandatory`, `remove`, and `update` flags.
- `LanguageCreator` service uses `ILanguageService` (async). Languages are created before all other entities so that dictionary items and content can reference culture codes.
- `[REMOVE]` deletes the language by ISO code; `[UPDATE]` modifies `isDefault` and `isMandatory` on the existing entry; create-if-not-found falls through from UPDATE.

#### Dictionary Items (`dictionaryItems` section)
- New top-level `dictionaryItems` section for bootstrapping Umbraco dictionary keys.
- Each entry supports `key`, `translations` (a map of ISO culture code → string value), `remove`, and `update` flags.
- `DictionaryCreator` service uses `ILocalizationService` and `ILanguageService`. Missing language references are logged as warnings rather than thrown.
- `[REMOVE]` deletes the dictionary item; `[UPDATE]` upserts translation values on an existing key; create-if-not-found falls through from UPDATE.

#### Media Types (`mediaTypes` section)
- New top-level `mediaTypes` section mirroring the `documentTypes` structure.
- Each entry supports `alias`, `name`, `icon`, `allowedAtRoot`, `tabs` (with properties), `remove`, and `update` flags.
- `MediaTypeCreator` service uses `IMediaTypeService` with the same tab/property building logic as `DocumentTypeCreator`.
- Additive update strategy: `[UPDATE]` replaces top-level fields only; tab/property merge follows the same additive-only rule as DocumentType updates.

#### Media Items (`media` section)
- New top-level `media` section for creating Umbraco Media nodes.
- Each entry supports `alias`, `name`, `mediaType`, `url` (downloads the file from a web URL), `properties`, `children` (recursive), `remove`, and `update` flags.
- `MediaCreator` service uses `IMediaService`. File downloads use `IHttpClientFactory`; failures are logged as warnings and do not abort the startup sequence.
- `IHttpClientFactory` is registered automatically by the composer (`AddHttpClient()`).

#### Members (`members` section)
- New top-level `members` section for creating Umbraco member accounts.
- Each entry supports `alias`, `name`, `email`, `username`, `password`, `memberType`, `isApproved`, `properties`, `remove`, and `update` flags.
- `MemberCreator` service uses `IMemberService`.
- `[REMOVE]` deletes by email; `[UPDATE]` updates name, approval status, and properties; create-if-not-found falls through from UPDATE.

#### Users (`users` section)
- New top-level `users` section for creating Umbraco backoffice users.
- Each entry supports `alias`, `name`, `email`, `username`, `userGroups` (list of group aliases), `remove`, and `update` flags.
- `UserCreator` service uses `IUserService`. Groups are resolved by alias and assigned via `AddGroup`.
- `[REMOVE]` deletes by email; `[UPDATE]` updates name, username, and group assignments.

#### Razor Content in Templates (`content` field)
- Templates now support an optional `content:` field containing explicit Razor markup.
- When provided, the content is used verbatim instead of the auto-generated `@inherits UmbracoViewPage` scaffold.
- Works for both CREATE and UPDATE operations; multi-line Razor views are supported via YAML block scalars (`|`).

#### DataType Config Application
- The `config:` dictionary declared in `dataTypes` entries is now applied to the created `DataType.Configuration` property.
- Supports any editor that accepts a `Dictionary<string, object>` config, including Block List, Image Cropper, Grid Layout, etc.

---

## [1.0.2] - 2025-xx-xx

### Added

#### Static Assets (JavaScript & CSS)
- New top-level `scripts` and `stylesheets` YAML sections for declaring static files to be written to `wwwroot`.
- Each entry supports `alias`, `name`, `path` (relative to `wwwroot`), and `content` fields.
- `StaticAssetCreator` service handles writing, updating, and deleting files under `IWebHostEnvironment.WebRootPath`.
- Subdirectories are created automatically when the target path includes nested folders.
- Duplicate aliases within a single YAML run are skipped with a warning.
- Leading slashes in paths are normalised automatically.
- Templates can reference static assets via `scripts` and `stylesheets` lists; the generated Razor template injects `<link rel="stylesheet">` tags into `<head>` and `<script src="...">` tags before `</body>`.

#### `[REMOVE]` flag
- Any item in any YAML section (`dataTypes`, `documentTypes`, `templates`, `content`, `scripts`, `stylesheets`) can be flagged with `remove: true` to delete the corresponding Umbraco entity or static file on startup.
- If the target does not exist a warning is logged and execution continues without throwing.
- Umbraco content removal cascades to child nodes automatically (no YAML child enumeration required).

#### `[UPDATE]` flag
- Any item can be flagged with `update: true` to upsert: update if already present, create if not.
- **DataType UPDATE**: looks up by name (`GetDataType`); if found, re-derives `DatabaseType` from the editor and re-applies `config`, then saves. See v1.0.14 for the full implementation.
- **DocumentType UPDATE**: applies an additive merge — name, icon, and `allowedAsRoot` are replaced; new tabs are added wholesale; new properties are merged into existing tabs; no existing property is ever removed to prevent data loss.
- **Template UPDATE**: regenerates the template file content (including any injected script/stylesheet tags) using `ITemplateService.UpdateAsync`.
- **Content UPDATE**: updates matching content node found by name under the same parent; sets property values, sort order, and published state; recurses into children.
- **Script/Stylesheet UPDATE**: overwrites the existing file in `wwwroot` when the `update` flag is set; skips the file otherwise.

#### Tests
- `StaticAssetCreatorTests` — 18 tests covering: file creation, subdirectory creation, skip-existing, overwrite-on-update, delete-on-remove, no-throw when remove target is missing, duplicate alias skipping, null alias, empty path, null list guard, leading slash normalisation — for both `Scripts` and `Stylesheets`.
- `YamlModelsTests` — extended with deserialization tests for `YamlScript`, `YamlStylesheet`, `remove`/`update` flag round-trips on all model types.
- `TemplateCreatorTests` — added REMOVE, UPDATE, create-when-update-target-missing, and HTML tag injection tests.
- `DataTypeCreatorTests` — added REMOVE, UPDATE (skip-when-exists), create-when-update-target-missing, and null list guard tests.
- `DocumentTypeCreatorTests` — refactored to class-level fixtures; added REMOVE, UPDATE (additive merge), create-when-update-target-missing, and null list guard tests.
- `ContentCreatorTests` — refactored to shared `Build()` helper; added REMOVE, no-throw when remove target missing, UPDATE, and create-when-update-target-missing tests.
- `YamlStartupComposerTests` — added assertion that `StaticAssetCreator` is registered in the DI container.
- `WebProjectConfigTests` — new smoke-test class that parses the web project's live `config/umbraco.yml` (linked as `fixtures/web-config.yml`) and verifies structure, counts, aliases, nesting, and absence of accidental REMOVE/UPDATE flags.

### Fixed
- `fixtures/sample.yml` had an `umbraco:` root wrapper that caused all parser-based integration tests to silently produce empty collections (`YamlParser.ParseYaml` deserialises directly to `UmbracoConfig`, not `YamlRoot`). The wrapper and extra indentation have been removed.
- `fixtures/sample.yml` used incorrect YAML keys: `editor:` → `editorUiAlias:`, `type:` → `documentType:`, `published:` → `isPublished:`, `values:` → `properties:`. All keys now match the `[YamlMember]` annotations in the model.
- `IntegrationTests` count assertions updated from 2 to 4 DataTypes to reflect the addition of `update:true` and `remove:true` fixture entries; config assertion offset corrected from `DataTypes[0]` to `DataTypes[2]`.

---

## [1.0.1] - 2025-04-xx

### Added
- Initial public release on NuGet (`SplatDev.Umbraco.Plugins.Yaml2Schema`).
- YAML-driven bootstrapping of Umbraco entities on application startup via `INotificationAsyncHandler<UmbracoApplicationStartedNotification>`.
- `YamlParser` — deserialises a YAML file into `UmbracoConfig` using YamlDotNet with camelCase naming and unmatched-property tolerance.
- `DataTypeCreator` — creates Data Types from the `dataTypes` YAML section; skips if a Data Type with the same editor alias already exists.
- `DocumentTypeCreator` — creates Document Types (Content Types) including tabs and properties; resolves Data Type references by name.
- `TemplateCreator` — creates Razor templates under `Views/`; generates a default `.cshtml` scaffold with `@inherits UmbracoViewPage`.
- `ContentCreator` — creates content nodes recursively; supports property value assignment, sort order, and immediate publishing.
- `YamlStartupComposer` — `IComposer` that registers all services as scoped and hooks the startup handler.
- Duplicate alias detection within a YAML run for all entity types.
- Full unit test suite (`DataTypeCreatorTests`, `DocumentTypeCreatorTests`, `TemplateCreatorTests`, `ContentCreatorTests`, `YamlParserTests`, `YamlModelsTests`, `YamlStartupComposerTests`, `IntegrationTests`).

---

## [1.0.0] - 2025-04-xx

### Added
- Initial prototype / internal release.
