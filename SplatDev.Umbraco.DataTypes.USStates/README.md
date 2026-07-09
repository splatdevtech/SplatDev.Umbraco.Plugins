# SplatDev.Umbraco.DataTypes.USStates

Umbraco plugin that auto-installs a pre-configured "US States" dropdown data type on application startup. The data type uses Umbraco's built-in `DropDownListFlexible` property editor, is single-select, and is populated with all 50 US states plus territories, military addresses, and the District of Columbia. Idempotent — safe to run on every startup.

## Install

```bash
dotnet add package SplatDev.Umbraco.DataTypes.USStates
```

Multi-targets `net8.0` (Umbraco 13) and `net10.0` (Umbraco 17).

## What's implemented

### Auto-install on startup

The plugin registers an Umbraco `IAsyncComponent` via `USStatesDataTypeComposer`. On application startup, `InitializeAsync` creates the data type if it does not already exist — no manual setup required.

After installation, the "US States" data type appears in the Umbraco backoffice under **Settings → Data Types** and is available for any document type property.

### Dropdown values (62 items)

| Category | Count | Examples |
|----------|-------|----------|
| US States | 50 | Alabama, Alaska, Wyoming |
| Territories | 4 | American Samoa, Guam, Puerto Rico, Virgin Islands |
| Freely Associated States | 4 | Marshall Islands, Micronesia |
| Canadian Territories | 3 | Northwest Territories, Nunavut, Yukon |
| Military | 3 | Armed Forces Europe, Americas, Pacific |
| District | 1 | District of Columbia |
| **Total** | **62** | |

All values are stored in ALL CAPS (e.g. `CALIFORNIA`, `TEXAS`). The display text uses title case.

### Configuration

| Property | Value |
|----------|-------|
| Property Editor | `Umbraco.DropDown.Flexible` |
| Database Type | `Ntext` |
| Multiple (multi-select) | `false` |
| Name | `US States` |

There are no appsettings keys, no `IOptions<T>` configuration classes, and no runtime toggles — the data type is installed unconditionally with the hardcoded list.

### Idempotency

`Install()` checks `IDataTypeService.GetDataType("US States")` before creation. If the data type already exists (including any custom edits an admin made to the values), the plugin skips creation. Repeated restarts are safe.

### Umbraco 17 status

The `net10.0` target compiles and registers the composer, but the `Install()` method is a **stub** — the data type is not automatically created in Umbraco 17. This is tracked for a future release that will use Umbraco 17's Management API.

## DI Registration

No manual DI registration required. `USStatesDataTypeComposer` is auto-discovered by Umbraco's `TypeFinder` at startup.

## Dependencies

**net8.0 (Umbraco 13):**

| Package | Purpose |
|---------|---------|
| `Umbraco.Cms.Core` (13.12.0) | Data type service, property editors, composers |
| `Umbraco.Cms.Infrastructure` (13.12.0) | Infrastructure services |
| `Umbraco.Cms.Web.BackOffice` (13.12.0) | Backoffice integration |
| `Umbraco.Cms.Web.Common` (13.12.0) | Web hosting |
| `Umbraco.Cms.Web.Website` (13.12.0) | Website routing |

**net10.0 (Umbraco 17):**

| Package | Purpose |
|---------|---------|
| `Umbraco.Cms.Core` (17.3.4) | Management API, composers |
| `Umbraco.Cms.Infrastructure` (17.3.4) | Infrastructure services |
| `Umbraco.Cms.Api.Management` (17.3.4) | Management API endpoints |
| `Umbraco.Cms.Web.Common` (17.3.4) | Web hosting |
| `Umbraco.Cms.Web.Website` (17.3.4) | Website routing |

---

**SplatDev.Umbraco.DataTypes.USStates** — part of the [SplatDev.Umbraco.Plugins](https://github.com/SplatDev-Ltda/SplatDev.Umbraco.Plugins) suite. Licensed under MIT. © SplatDev Ltda.
