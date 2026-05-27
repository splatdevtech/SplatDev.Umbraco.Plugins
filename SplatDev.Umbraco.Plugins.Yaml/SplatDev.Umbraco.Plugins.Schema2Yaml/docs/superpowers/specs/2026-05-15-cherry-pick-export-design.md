# Cherry-Pick Export Dialog — Design Spec

**Date:** 2026-05-15  
**Status:** Approved  
**Plugin:** SplatDev.Umbraco.Plugins.Schema2Yaml

---

## Summary

Add a named-profile export selection system to the Schema2Yaml dashboard.  
Users can configure which categories and specific entities to include in an export, save those selections as named profiles, and activate a profile as a filter on the main Export button.  
A full one-click export remains available at all times when no profile is active.

---

## Goals

- Category-level on/off toggles for all 10 export buckets
- Entity-level filtering within each category:
  - Flat checklist for: Languages, DataTypes, DocumentTypes, MediaTypes, Templates, DictionaryItems, Members, Users
  - Tree picker (with recursive check/uncheck) for: Content, Media
- Named profiles stored in a new Umbraco database table
- Only one profile active at a time; activating a profile makes the main Export button use it
- Full export (no filter) always reachable via "clear active profile"
- All profile state persists server-side across users and deployments

---

## Architecture

### New layers

| Layer | Location | Purpose |
|---|---|---|
| DB migration | `src/Migrations/AddExportProfilesTable.cs` | Creates `schema2yamlExportProfiles` table |
| Profile models | `src/Models/ExportProfileModels.cs` | `ExportProfile`, `ExportSelection`, `CategorySelection` |
| Profile service | `src/Services/ExportProfileService.cs` | CRUD + activation logic |
| Profile controller | `src/Controllers/ExportProfileController.cs` | REST API for profile management |
| Items controller | `src/Controllers/ExportItemsController.cs` | Available items + content/media trees |
| Modified service | `src/Services/SchemaExportService.cs` | New filtered overloads |
| Modified exporters | `src/Services/*Exporter.cs` | New `ExportAsync(CategorySelection)` overload on each |
| Modified dashboard | `wwwroot/App_Plugins/Schema2Yaml/dashboard.js` | Configure dialog + active-profile badge |

### Unchanged

- All existing `GET /umbraco/api/schemaexport/*` endpoints
- All existing exporter `ExportAsync()` no-arg signatures
- `ExportRoot` / `UmbracoExport` models

---

## Data Model

### Database table: `schema2yamlExportProfiles`

| Column | Type | Notes |
|---|---|---|
| `id` | INT PK AUTOINCREMENT | |
| `name` | NVARCHAR(255) NOT NULL | Display name |
| `isActive` | BIT NOT NULL DEFAULT 0 | At most one row = 1 |
| `selectionJson` | NTEXT NOT NULL | Serialised `ExportSelection` |
| `createdDate` | DATETIME NOT NULL | |
| `modifiedDate` | DATETIME NOT NULL | |

Enforced invariant: when activating profile N, set all other rows `isActive = 0` before setting row N `isActive = 1`. Done in a single transaction in `ExportProfileService`.

### C# models — `src/Models/ExportProfileModels.cs`

```csharp
public class ExportProfile
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public ExportSelection Selection { get; set; } = new();
    public DateTime CreatedDate { get; set; }
    public DateTime ModifiedDate { get; set; }
}

public class ExportSelection
{
    public CategorySelection Languages       { get; set; } = new();
    public CategorySelection DataTypes       { get; set; } = new();
    public CategorySelection DocumentTypes   { get; set; } = new();
    public CategorySelection MediaTypes      { get; set; } = new();
    public CategorySelection Templates       { get; set; } = new();
    public CategorySelection Media           { get; set; } = new();
    public CategorySelection Content         { get; set; } = new();
    public CategorySelection DictionaryItems { get; set; } = new();
    public CategorySelection Members         { get; set; } = new();
    public CategorySelection Users           { get; set; } = new();
}

public class CategorySelection
{
    public bool IncludeAll { get; set; } = true;
    public List<string> Aliases { get; set; } = [];  // flat categories
    public List<int> NodeIds { get; set; } = [];      // Content + Media only
}
```

### Filter semantics (applied at export time per category)

| State | Result |
|---|---|
| Category not present / `IncludeAll = false` + empty list | Excluded entirely (empty list written to YAML) |
| `IncludeAll = true` | Export everything in that category |
| `IncludeAll = false` + non-empty `Aliases` | Export only matching aliases |
| `IncludeAll = false` + non-empty `NodeIds` (Content/Media) | Export only those subtrees (node + all descendants) |

---

## API Endpoints

### `ExportProfileController` — `/umbraco/api/exportprofile/`

| Method | Route | Body | Response |
|---|---|---|---|
| GET | `list` | — | `[{ id, name, isActive }]` |
| GET | `active` | — | Full `ExportProfile` or 204 |
| POST | `create` | `{ name, selection }` | Created `ExportProfile` |
| PUT | `update/{id}` | `{ name, selection }` | Updated `ExportProfile` |
| DELETE | `delete/{id}` | — | 204 |
| POST | `activate/{id}` | — | 200 |
| POST | `deactivate` | — | 200 |

### `ExportItemsController` — `/umbraco/api/exportitems/`

| Method | Route | Response |
|---|---|---|
| GET | `available` | All selectable items per flat category (see shape below) |
| GET | `contenttree` | Nested `{ id, name, children[] }` tree |
| GET | `mediatree` | Nested `{ id, name, children[] }` tree |

`/available` response:
```json
{
  "dataTypes":       [{ "alias": "Textstring",   "name": "Textstring" }],
  "documentTypes":   [{ "alias": "article",       "name": "Article" }],
  "mediaTypes":      [{ "alias": "Image",         "name": "Image" }],
  "templates":       [{ "alias": "home",          "name": "Home" }],
  "languages":       [{ "alias": "en-US",         "name": "English (US)" }],
  "dictionaryItems": [{ "alias": "Common.Close",  "name": "Common.Close" }],
  "members":         [{ "alias": "user@x.com",    "name": "User Name" }],
  "users":           [{ "alias": "admin@x.com",   "name": "Admin" }]
}
```

### `SchemaExportController` — new filtered endpoints

| Method | Route | Body | Response |
|---|---|---|---|
| POST | `exportselected` | `ExportSelection` | `{ yaml, statistics }` |
| POST | `downloadyamlselected` | `ExportSelection` | YAML file download |
| POST | `downloadzipselected` | `ExportSelection` | ZIP file download |

Existing GET endpoints (`export`, `downloadyaml`, `downloadzip`) unchanged.

### `SchemaExportService` — new signatures

```csharp
// New — filtered path
Task<string> ExportToYamlAsync(ExportSelection selection);
Task<byte[]> ExportToZipAsync(ExportSelection selection);
```

Each sub-exporter gains:
```csharp
Task<List<T>> ExportAsync(CategorySelection filter);
```

---

## Migration

File: `src/Migrations/AddExportProfilesTable.cs`  
Plan name: `Schema2Yaml.AddExportProfiles`  
Registered in `Schema2YamlComposer` via `builder.WithMigrationPlan<AddExportProfilesPlan>()`.

Migration runs once on startup if the table does not exist. No down-migration needed (plugin uninstall handles cleanup separately).

---

## UI — Lit Component Changes

### New state

```js
_profiles           // [{ id, name, isActive }]
_activeProfile      // full ExportProfile | null
_showConfigDialog   // bool
_configuring        // ExportSelection being edited
_editingProfileId   // int | null  (null = new profile)
_editingProfileName // string
_availableItems     // { dataTypes[], documentTypes[], ... }
_contentTree        // nested tree nodes
_mediaTree          // nested tree nodes
_loadingItems       // bool
```

### Action bar

No active profile:
```
[ Export to YAML ]  [ Download YAML ]  [ Download ZIP (with media) ]  [ Configure Export ]
```

Active profile (`Schema only`):
```
[ Export (Schema only ✕) ]  [ Download YAML ]  [ Download ZIP ]  [ Configure Export ]
```

The `✕` is an inline icon button inside the Export button that calls `POST /deactivate` and resets to full-export mode.

### Configure dialog — `<uui-dialog>` ~900px wide

Two-column layout:

```
┌─────────────────────────────────────────────────────────────────────┐
│  Configure Export                                          [  ×  ]  │
├──────────────────┬──────────────────────────────────────────────────┤
│  PROFILES        │  SELECTION                                        │
│                  │                                                   │
│  + New profile   │  Profile name: [ Schema Only          ]          │
│  ─────────────── │  ─────────────────────────────────────────────── │
│  • Schema Only ● │  ☑ Languages        (all)                        │
│    Content Only  │  ☑ Data Types       ▼ filter...                  │
│    Full Backup   │     ☑ Textstring   ☑ Rich Text  ☐ Checkbox      │
│                  │  ☑ Document Types   ▼ filter...                  │
│  [ Delete ]      │     ☑ Article      ☑ Home       ☐ Blog Post     │
│                  │  ☑ Media Types     (all)                         │
│                  │  ☑ Templates       (all)                         │
│                  │  ☐ Content         ── (disabled)                 │
│                  │  ☑ Media           ▼ tree...                     │
│                  │     ▶ Images  ▶ Documents  ☑ Thumbnails          │
│                  │  ☐ Dictionary Items── (disabled)                 │
│                  │  ☐ Members         ── (disabled)                 │
│                  │  ☐ Users           ── (disabled)                 │
├──────────────────┴──────────────────────────────────────────────────┤
│                          [ Cancel ]  [ Save ]  [ Save & Apply ]     │
└─────────────────────────────────────────────────────────────────────┘
```

### Dialog interaction rules

- **Category checkbox off** → collapses and disables its entity section; category will be excluded from export
- **Category checkbox on, no entity filter** → shows `(all)` label; entire category exported
- **"▼ filter..." (flat categories)** → expands inline checklist populated from `/available`; unchecking items sets `IncludeAll = false` with explicit `aliases` list
- **"▼ tree..." (Content/Media)** → expands inline tree from `/contenttree` or `/mediatree`; checking a node checks all descendants; unchecking a node unchecks all descendants; checking all children auto-checks parent
- **Profile list click** → loads that profile's selection for editing
- **"+ New profile"** → clears the right panel to a blank selection, clears profile name field
- **Save** → `POST /create` or `PUT /update/{id}`; does not activate
- **Save & Apply** → save then `POST /activate/{id}`; closes dialog; dashboard shows active-profile badge
- **Cancel** → discards unsaved changes; closes dialog
- **Delete** → `DELETE /delete/{id}`; if deleted profile was active, clears active profile on dashboard

### On dialog open

1. Call `GET /list` → populate profile list
2. Call `GET /active` → if profile active, select it in list and load its selection
3. Call `GET /available` → populate flat entity checklists (lazy: only fetch if not already cached)
4. Content/Media trees fetched lazily on first expand of that category's tree picker

---

## Files Changed / Created

### New files
- `src/Migrations/AddExportProfilesTable.cs`
- `src/Models/ExportProfileModels.cs`
- `src/Services/ExportProfileService.cs`
- `src/Controllers/ExportProfileController.cs`
- `src/Controllers/ExportItemsController.cs`

### Modified files
- `src/Services/SchemaExportService.cs` — filtered overloads
- `src/Services/*Exporter.cs` (10 files) — `ExportAsync(CategorySelection)` overload on each
- `src/Controllers/SchemaExportController.cs` — 3 new POST endpoints
- `src/Composers/Schema2YamlComposer.cs` — register new services + migration plan
- `wwwroot/App_Plugins/Schema2Yaml/dashboard.js` — configure dialog + active-profile state

### Unchanged
- `src/Models/ExportModels.cs`
- `wwwroot/App_Plugins/Schema2Yaml/umbraco-package.json`
- All existing GET endpoints

---

## Error Handling

- Profile not found (GET/PUT/DELETE/activate) → 404
- DB write failure on save → 500 with `{ error, message }`; dashboard shows danger notification
- `/available` or tree fetch failure → dialog shows inline error; entity-level selectors fall back to "all"
- Export with invalid selection (e.g., nodeIds that no longer exist) → silently skip missing items, log warning; export proceeds with whatever is found

---

## Out of Scope

- Import of profiles from another environment (copy/paste JSON is a workaround)
- Scheduling exports with a saved profile
- Per-user profiles (all profiles are shared across backoffice users)
