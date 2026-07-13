# SplatDev.Mobile.Detection

Mobile device detection middleware for ASP.NET Core. Detects mobile browsers, tablets,
and device capabilities from HTTP request headers. Targets `net8.0` and `net10.0`.

## Installation

```bash
dotnet add package SplatDev.Mobile.Detection
```

## Features

- Middleware that sets `HttpContext.Features` with device info
- `MobileDevice` model — `IsMobile`, `IsTablet`, `IsBot`, device name, OS
- `MobileDetection.IsMobileBrowser(HttpContext)` — single-call check
- Regex-based User-Agent parsing for common mobile browsers

## Usage

### Register middleware

```csharp
// Program.cs
app.UseMobileDetection();
```

### Check in a controller or view

```csharp
var feature = HttpContext.Features.Get<MobileDevice>();
if (feature?.IsMobile == true)
    return View("Mobile");
```

### Direct check

```csharp
using SplatDev.Mobile.Detection;

if (MobileDetection.IsMobileBrowser(HttpContext))
    Redirect("/mobile");
```

## Model

| Property | Type | Description |
|---|---|---|
| `IsMobile` | `bool` | Mobile phone browser |
| `IsTablet` | `bool` | Tablet device |
| `IsBot` | `bool` | Bot/spider/crawler |
| `DeviceName` | `string?` | Detected device name |
| `OperatingSystem` | `string?` | e.g. "iOS", "Android" |

> **Note**: `IsMobileBrowser` reads from `HttpContext.Request.Headers["User-Agent"]`,
> not the query string. The middleware sets the feature early in the pipeline so
> downstream components can branch on device type.

## License

MIT
