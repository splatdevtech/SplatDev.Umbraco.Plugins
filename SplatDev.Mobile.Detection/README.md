# SplatDev.Mobile.Detection

ASP.NET Core library for detecting mobile browser requests. Combines a JSON database of 67 known mobile device codes with two compiled regex patterns to determine whether an incoming HTTP request originated from a mobile device. No Umbraco dependency — works in any ASP.NET Core application targeting `net8.0` or `net10.0`.

## Install

```bash
dotnet add package SplatDev.Mobile.Detection
```

Multi-targets `net8.0` and `net10.0`. Published to nuget.org.

## What's implemented

### Static detection API

All methods live on `SplatDev.Mobile.Detection.MobileDetection` — a static class with no DI registration required.

#### `IsMobileBrowser(HttpRequest request)`

Checks query-string parameters for mobile indicators:

- Returns `true` if `HTTP_X_WAP_PROFILE` is present in the query string.
- Returns `true` if `HTTP_ACCEPT` contains `"wap"`.
- Returns `true` if `HTTP_USER_AGENT` matches any of the 67 known device codes in `device_list.json`.

```csharp
using SplatDev.Mobile.Detection;

app.MapGet("/", (HttpRequest request) =>
{
    var isMobile = MobileDetection.IsMobileBrowser(request);
    return isMobile ? "Mobile" : "Desktop";
});
```

#### `IsMobileBrowserRegex(string userAgent)`

Checks a raw user-agent string against two compiled regex patterns:

- **`MobileCheck`** — matches broad mobile browser identifiers (Android, iPhone, BlackBerry, Opera Mini, Windows CE, etc.).
- **`MobileVersionCheck`** — matches the first 4 characters of the user-agent against device manufacturer codes (Nokia, Samsung, Sony, LG, Motorola, etc.).

```csharp
var ua = "Mozilla/5.0 (iPhone; CPU iPhone OS 17_0 like Mac OS X)";
var isMobile = MobileDetection.IsMobileBrowserRegex(ua); // true
```

Both regex properties are public and mutable — replace them at startup if you need custom patterns:

```csharp
MobileDetection.MobileCheck = new Regex("my-custom-pattern", RegexOptions.IgnoreCase);
```

### Device database

`device_list.json` is bundled as content (copied to output on build) and loaded at static-initialization time. Contains 67 codes including: `midp`, `j2me`, `blackberry`, `symbian`, `nokia`, `iphone`, `android`, `windows ce`, `palm`, and more.

The loaded array is available at `MobileDetection.MobileDevices`.

## DI Registration

No DI registration needed. All members are static. Add the package, call the methods directly.

## Dependencies

| Package | Purpose |
|---------|---------|
| `Newtonsoft.Json` (13.0.3) | Loading `device_list.json` at static init |
| `Microsoft.AspNetCore.App` (framework reference) | `HttpRequest` type |

---

**SplatDev.Mobile.Detection** — part of the [SplatDev.Umbraco.Plugins](https://github.com/SplatDev-Ltda/SplatDev.Umbraco.Plugins) suite. Licensed under MIT. © SplatDev Ltda.
