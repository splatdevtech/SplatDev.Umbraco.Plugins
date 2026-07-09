# SplatDev.Content

General-purpose content utility library for CSV and Excel import/export, HTML and URL manipulation, ISO 3166 country lookups, timezone reference data, QR code generation, string sanitization, and SHA-256 hashing. No Umbraco dependency — use it in any .NET application.

## Install

```bash
dotnet add package SplatDev.Content
```

Multi-targets `net8.0` and `net10.0`. Published to nuget.org.

## What's implemented

### Excel import (`ImportDigest`)

Reads Excel files via EPPlus into a structured cell grid with validation error collection:

```csharp
using SplatDev.Content;

using var stream = File.OpenRead("data.xlsx");
var digest = new ImportDigest(stream);
var sheet = digest.Import(ignoreRowWithEmptyCells: false);

Console.WriteLine($"Rows: {digest.RowsImported}, Columns: {digest.ColumnsImported}");
foreach (var cell in sheet)
    Console.WriteLine($"{cell.CellName}: {cell.CellValue}");

if (digest.ErrorList.Errors.Count > 0)
    Console.WriteLine($"Import errors: {digest.ErrorList.Errors.Count}");
```

**Note:** Sets EPPlus `LicenseContext.NonCommercial`. For commercial use, acquire an EPPlus license.

### CSV export helpers

| Method | Description |
|--------|-------------|
| `data.WriteCells<T>(locale)` | Converts typed objects to cell arrays, formatting dates/numbers per locale |
| `type.WriteHeaders()` | Returns display names from `[Display(Name=...)]` attributes |

### HTML & URL helpers (`HtmlHelpers`)

A large static class with 20+ utility methods:

| Category | Methods |
|----------|---------|
| **URL building** | `BuildUrl`, `RootUrl`, `RootDomain`, `GetAbsoluteDomain`, `Protocol` |
| **Sanitization** | `Sanitize`, `SanitizeUrl`, `CleanUrl` (slugify), `StripHtmlTags`, `StripParagraph` |
| **Encoding** | `EncodeUrl`, `DecodeUrl` |
| **HTTP context** | `GetUserIP`, `IsHttps`, `GetMIMEType` |
| **String** | `RandomString`, `RemoveDiacritics`, `ConvertLineBreakToHtml`, `SearchEngineFormat` |

### Country & culture lookups

| Type | Description |
|------|-------------|
| `Countries` enum | 733 entries covering all culture/locale combinations |
| `ISO3166Country` | ISO 3166-1 country: `Name`, `Alpha2`, `Alpha3`, `NumericCode`, `DialCodes` |
| `Iso3166Countries.LIST` | 260 countries/territories (alpha-2, alpha-3, numeric, dial codes) |
| `RegionInfo` helpers | Search by culture code, alpha-2, or alpha-3 via `CountryHelpers` |

```csharp
var country = CountryHelpers.FromAlpha2("US");
Console.WriteLine($"{country.Name}, dial: {country.DialCodes[0]}"); // United States, 1

var all = CountryHelpers.GetCountriesByIso3166(); // 260 entries
```

### Timezone reference data

```csharp
foreach (var tz in TimeZones.AllTimeZones)  // ~206 entries
    Console.WriteLine($"{tz.Abbreviation} — {tz.Name} ({tz.UtcOffsetDisplay})");
```

### QR code generation

Delegates to the QRServer API (no local generation):

```csharp
var qrUrl = QRHelpers.GenerateQR(size: 300, data: "https://splatdev.com");
// Returns: "https://api.qrserver.com/v1/create-qr-code/?size=300x300&data=https://splatdev.com"
```

### Encryption helpers

```csharp
var hash = EncryptDecryptHelpers.Sha256Encrypt("password");
var match = EncryptDecryptHelpers.Sha256Matches("password", hash); // true
```

### String utilities (`StringHelper`)

| Method | Description |
|--------|-------------|
| `RemoveDiacritics()` | Strips accents via Unicode normalization |
| `RemoveExtraSpaces()` | Collapses double spaces |
| `Truncate(maxLength)` | Truncates to N characters |

## DI Registration

No DI registration needed. All types are static helper classes or plain models.

## Dependencies

| Package | Purpose |
|---------|---------|
| `EPPlus` (7.6.1) | Excel file reading (`.xlsx`) |
| `Microsoft.AspNetCore.Http` (2.2.0) | `HttpContext` for URL/IP helpers |

---

**SplatDev.Content** — part of the [SplatDev.Umbraco.Plugins](https://github.com/SplatDev-Ltda/SplatDev.Umbraco.Plugins) suite. Licensed under MIT. © SplatDev Ltda.
