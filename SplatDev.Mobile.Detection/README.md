# SplatDev.Mobile.Detection

Mobile browser detection for ASP.NET Core applications. Identifies mobile devices using HTTP request headers and a large dictionary of regex patterns — no third-party device database required.

## Install

```bash
dotnet add package SplatDev.Mobile.Detection
```

No external dependencies beyond `Newtonsoft.Json` and `Microsoft.AspNetCore.Http`.

## How it works

### `IsMobileBrowser(HttpRequest)` — header-based detection

Reads HTTP request headers (`HTTP_X_WAP_PROFILE`, `HTTP_ACCEPT`, `HTTP_USER_AGENT`) and matches against a compiled list of known mobile device codes loaded from `device_list.json` at startup.

```csharp
public static bool IsMobileBrowser(HttpRequest request)
```

- Checks `HTTP_X_WAP_PROFILE` header presence
- Checks `HTTP_ACCEPT` header for `wap`
- Cross-references `HTTP_USER_AGENT` against the `MobileDevices` registry

### `IsMobileBrowserRegex(string userAgent)` — regex-based detection

A pure regex engine that matches against two compiled patterns:

- `MobileCheck` — matches ~50 well-known mobile browser tokens (`android`, `iphone`, `blackberry`, `opera mobi`, `windows ce`, etc.)
- `MobileVersionCheck` — matches ~500+ legacy device codes (`nokia`, `motorola`, `samsung`, `lg`, `htc`, etc.)

```csharp
public static bool IsMobileBrowserRegex(string userAgent)
```

Returns `false` for null/empty strings and short user agents (< 4 chars).

### `MobileDevice` model

```csharp
public class MobileDevice
{
    public string Code { get; set; }  // device identifier code
    public override string ToString() => $"{Code}";
}
```

### Static device registry

On first use, the static constructor loads `device_list.json` from the assembly directory:

```csharp
public static MobileDevice[] MobileDevices { get; private set; }
```

## Usage

### In a Razor view or middleware

```csharp
@using SplatDev.Mobile.Detection

@if (MobileDetection.IsMobileBrowser(Request))
{
    <partial name="_MobileLayout" />
}
else
{
    <partial name="_DesktopLayout" />
}
```

### With a custom user agent string

```csharp
var ua = "Mozilla/5.0 (iPhone; CPU iPhone OS 17_0 like Mac OS X)";
bool isMobile = MobileDetection.IsMobileBrowserRegex(ua);
// returns true
```

### Direct device code lookup

```csharp
var devices = MobileDetection.MobileDevices;
bool isKnownDevice = devices.Any(d => d.Code == "iphone");
```

## Configuration

Place `device_list.json` alongside the assembly. The file is a JSON array of device code strings:

```json
[
  { "Code": "iphone" },
  { "Code": "ipad" },
  { "Code": "nexus" }
]
```

## Caveats

- `CodeBase` path resolution in the static constructor uses `Assembly.GetCallingAssembly().CodeBase` which may not work in all hosting contexts (see SPL-2496). Consider injecting an `IHostEnvironment` or `IWebHostEnvironment` for production use.
- `IsMobileBrowser` uses `CodeBase.Substring(8)` to strip the `file:///` prefix — Windows-specific. Unix paths require different stripping.

---

Copyright SplatDev
