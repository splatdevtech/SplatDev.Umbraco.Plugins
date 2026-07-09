# DictionaryManager

Dictionary import/export/CRUD manager for Umbraco — full rewrite of the Umbraco 8 plugin using ILocalizationService. Supports Umbraco 13 (net8.0) and Umbraco 17 (net10.0).

[![NuGet](https://img.shields.io/nuget/v/SplatDev.Umbraco.Plugins.DictionaryManager.svg)](https://www.nuget.org/packages/SplatDev.Umbraco.Plugins.DictionaryManager)

## Compatibility

| Umbraco | .NET | Package Version |
|---------|------|-----------------|
| 13.x    | 8.0  | 2.0.0           |
| 17.x    | 10.0 | 2.0.0           |

## Installation

```sh
dotnet add package SplatDev.Umbraco.Plugins.DictionaryManager
```

## Quick Start

Register in `Program.cs`:

```csharp
builder.CreateUmbracoBuilder()
    .AddBackOffice()
    .AddWebsite()
    .AddDictionaryManager()   // <-- add this
    .Build();
```

## API Endpoints

All endpoints are under `/umbraco/api/dictionarymanager/` and return JSON.

| Method | Route | Purpose |
|--------|-------|---------|
| `GET` | `/GetAll` | List all dictionary items with translations |
| `POST` | `/Create` | Create a new dictionary item (body: `DictionaryItemDto`) |
| `PUT` | `/Update` | Update an existing dictionary item (body: `DictionaryItemDto`) |
| `DELETE` | `/Delete?key={key}` | Delete a dictionary item by key |
| `POST` | `/Import?overrideExisting=true` | Bulk-import items (body: `List<DictionaryItemDto>`) |
| `GET` | `/Export` | Export all items as a JSON file download |

### `DictionaryItemDto` shape

```json
{
  "key": "my.dictionary.key",
  "parentKey": null,
  "languageCode": "en-US",
  "value": "Default translation",
  "translations": {
    "en-US": "Default translation",
    "es": "Traduccion",
    "pt-BR": "Traducao"
  }
}
```

## Import / Export

Import and export use **JSON only**. No CSV or XLSX paths are supported — the `DictionaryItemDto` list serializes directly to JSON.

- **Export** — `GET /Export` returns a `.json` file with all items and their translations.
- **Import** — `POST /Import` accepts a JSON array of `DictionaryItemDto`. Set `overrideExisting=true` to replace existing keys; by default, keys already in the dictionary are skipped.
- Parents are processed before children automatically, so nested dictionary trees import cleanly.

## Dashboard & Settings Surface

The plugin ships with **two manifests**:

### `App_Plugins/DictionaryManager/package.manifest` — Dashboard

Registers a **Settings-section dashboard** ("Dictionary Manager" tab) accessible to admin users. This is the main UI for browsing, importing, and exporting dictionary items.

### `App_Plugins/DictionarySettingsSurface/package.manifest` — Property Editor

Registers a **property editor** named "Dictionary Import & Export" (`alias: dictionaryManager`). This surface is intended to be placed on a content node so content editors can trigger import/export from the content tree without needing admin access to the Settings section.

## U13 vs U17 UI

| Umbraco | Dashboard | Technology |
|---------|-----------|------------|
| 13.x | `angular/dictionarymanager.html` | AngularJS controller (`dictionarymanager.controller.js`) |
| 17.x | `dist/dictionarymanager-dashboard.element.js` | Bellissima Lit element (`LiterElement` + `UmbElementMixin`) |

Both dashboards share the same API controller (`DictionaryManagerApiController`). The AngularJS dashboard (U13) is a full CRUD UI with inline editing and bulk operations. The Lit dashboard (U17) is a Bellissima-idiomatic port rendered inside the Umbraco 17 backoffice shell.

The `DictionarySettingsSurface` property editor works on both Umbraco versions as it uses only the standard backoffice property-editor contract.

---

## License

MIT © [SplatDev](https://github.com/SplatDev-Ltda)
