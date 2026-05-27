+ Commit and Push
+ Update nuget package and umbraco marketplace

# Phase 5 — Umbraco 13 (net8.0) Support & CodeFirst Migration

## Multi-targeting (net8.0 / net10.0)
- [x] Add `net8.0` (Umbraco 13.12.x) as a second target framework alongside `net10.0` (Umbraco 17.x)
- [x] Add conditional `#if NET8_0 / #else / #endif` blocks for APIs that differ between versions:
  - `TemplateCreator`: `IFileService.GetTemplate/SaveTemplate/DeleteTemplate` (U13) vs `ITemplateService.CreateAsync/UpdateAsync/DeleteAsync` (U14+)
  - `DocumentTypeCreator`: template resolution via `IFileService` (U13) vs `ITemplateService` (U14+)
  - `DataTypeCreator`: `EditorUiAlias` property on `DataType` is U14+ only (new backoffice); omitted on U13
  - `UserCreator`: `IUserGroupService.UpdateUserGroupsOnUsersAsync` is U14+ only; user group assignment not supported on U13 build (logs a warning)

## Known Umbraco 13 Incompatibilities (best-effort support)
- [ ] **Template creation on U13**: The `new Template(IShortStringHelper, name, alias)` constructor and
      `IFileService` are used for U13. The DI container must supply `IFileService` and `IShortStringHelper`
      for `TemplateCreator` and `DocumentTypeCreator` when running on the net8.0 target.
- [ ] **User group assignment on U13**: `IUserGroupService.UpdateUserGroupsOnUsersAsync` does not exist in
      Umbraco 13. Group assignments for seeded users must be completed manually in the back-office.
- [ ] **`EditorUiAlias` on U13**: The new-backoffice `EditorUiAlias` field on `DataType` is absent in
      Umbraco 13. DataTypes will still be created correctly but the UI alias registration is skipped.
- [ ] **`ILanguageService`** async methods (`CreateAsync`, `UpdateAsync`, `DeleteAsync`) — verify presence
      in U13; if missing, fall back to `ILocalizationService` language helpers.
- [ ] **Test against Umbraco 13**: Full integration test run against a live U13 site has not been
      performed. The `#if NET8_0` guards are best-effort based on API diff analysis.

## Missing CodeFirst Features (UmbracoCms.CodeFirst → Yaml2Schema Migration Gaps)
The following features existed in the Umbraco 8 `UmbracoCms.CodeFirst` library (`IMacro`, `IAccessRule`,
`IDashboard`) but are **not yet implemented** in Yaml2Schema:

- [ ] **Macros support** (`IMacro` in CodeFirst): CodeFirst allowed declaring Umbraco macros with
      properties such as `Alias`, `MacroType`, `MacroSource`, `CacheDuration`, `UseInEditor`, etc.
      Yaml2Schema has no equivalent `macros:` section. To implement: add `YamlMacro` model, a
      `MacroCreator` service using `IMacroService`, and wire it up in the handler.

- [ ] **Access Rules** (`IAccessRule` in CodeFirst): CodeFirst allowed restricting content access by
      member group or role (`AccessRuleTypes.AllowGroup`, `AllowRole`, etc.). Yaml2Schema has no content
      access restriction support. To implement: add `YamlAccessRule` model and use
      `IPublicAccessService` to create access entries after content is seeded.

- [ ] **Dashboard configuration** (`IDashboard` in CodeFirst): CodeFirst allowed registering custom
      dashboards (alias, caption, sections, view, access rules) programmatically. Yaml2Schema has no
      `dashboards:` section. Umbraco 9+ moved dashboard config to `dashboards.json` / `IUmbracoBuilder`
      extensions; to implement: add `YamlDashboard` model and write to the `config/dashboards.json`
      file or register via `IUmbracoBuilder.Dashboards()`.

# Phase 2
+ [x] Add support for including nuget packages (PackageValidator — validates assemblies are loaded, v1.0.19)
+ [x] Add support for custom DataTypes (DataTypeCreator — create/update/remove, v1.0.x)
+ [x] Add support for custom Block List items (contentElementTypeAlias resolution + $type content seeding, v1.0.18)
+ [x] Add support for custom Grid Block Item (contentElementTypeAlias resolution covers BlockGrid, v1.0.18)
+ [x] Add support for custom Media Types (MediaTypeCreator — create/update/remove, v1.0.x)
+ [x] Add support for custom Media Properties (MediaTypeCreator tabs/properties, v1.0.x)
+ [x] Add support for custom Document Types (DocumentTypeCreator — create/update/remove, v1.0.x)
+ [x] Add support for custom Property Editors (PropertyEditorCreator — App_Plugins manifest + JS, valueType fallback in DataTypeCreator, v1.0.19)

# Phase 3 ✅ (v1.0.18)
 + [x] Instead of flat Pilar N - Title / Text properties, use Block List ($type convention in content seeding + contentElementTypeAlias resolution in DataType config)
 + [x] Add media folder creation for organizing media (EnsureFolder helper)
 + [x] Add folder field to each media item
 + [x] Download medias from url and save in media folder into folder set in parameter

 # Phase 4 ✅
- [x] Add support for changing the ModelBuilder output path — `modelsBuilder.outputPath` in YAML writes `Umbraco:CMS:ModelsBuilder:ModelsDirectoryAbsolute` to `appsettings.json` (`ModelsBuilderConfigurator` service).
- [x] Add support for generating `publishedmodels` from the YAML-defined schema — `PublishedModelsGenerator` creates typed C# partial classes (`[PublishedModel]`, `PublishedContentModel`) with property accessors, written to the configured `outputPath`.
- [x] Organize media downloads into a folder structure — `mediaDefaultFolder:` on the `umbraco:` root applies as a section-level default for all `media:` items without their own `folder:`.
- [x] Add support for selecting icon for nested elements in Block List, Block Grid, and Single Block editor configs. *(Already implemented: `icon:` on `documentTypes` entries is applied to element types in DocumentTypeCreator.)*

