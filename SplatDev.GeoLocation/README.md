# SplatDev.GeoLocation

IP-based geolocation using the [ipinfo.io](https://ipinfo.io) API. Resolves an IP address to city, region, country, coordinates, timezone, ISP, and more via a single async call. No caching, no rate-limiting, no Umbraco integration — a thin utility you call from your own middleware, background job, or analytics pipeline.

## Install

```bash
dotnet add package SplatDev.GeoLocation
```

Multi-targets `net8.0` and `net10.0`. Published to nuget.org.

## What's implemented

### `GeoLocator`

A single service class with one method:

```csharp
using SplatDev.GeoLocation;

var locator = new GeoLocator();
var result = await locator.GetIpInfoGeoLocation(
    token: "your-ipinfo-token",
    ipAddress: "8.8.8.8");

Console.WriteLine($"{result.City}, {result.Region}, {result.Country}");
Console.WriteLine($"Location: {result.Loc}");
Console.WriteLine($"Org: {result.Org}");
Console.WriteLine($"Timezone: {result.Timezone}");
```

### `GeoLocationResult`

| Property | Description |
|----------|-------------|
| `Ip` | The queried IP address |
| `Hostname` | Reverse DNS hostname |
| `City` | City name |
| `Region` | Region / state name |
| `Country` | ISO 3166-1 alpha-2 country code |
| `Loc` | Latitude, longitude (e.g. `"37.422,-122.084"`) |
| `Org` | ISP / organization |
| `Postal` | Postal / ZIP code |
| `Timezone` | IANA timezone identifier |

### API base URL

```csharp
SplatDev.GeoLocation.Constants.APINFO  // "https://ipinfo.io/"
```

The `APINFO` field is a public mutable `string` — replace it before first use if you run your own ipinfo-compatible proxy.

## DI Registration

No built-in DI extensions. Register manually if you prefer constructor injection:

```csharp
builder.Services.AddSingleton<GeoLocator>();
```

## Getting an ipinfo.io token

Sign up at [ipinfo.io/signup](https://ipinfo.io/signup). The free tier provides 50k requests/month. Paste the token into your appsettings or secrets manager and pass it to `GetIpInfoGeoLocation` on each call.

## Dependencies

| Package | Purpose |
|---------|---------|
| `RestSharp` (112.1.0) | HTTP client |

---

**SplatDev.GeoLocation** — part of the [SplatDev.Umbraco.Plugins](https://github.com/SplatDev-Ltda/SplatDev.Umbraco.Plugins) suite. Licensed under MIT. © SplatDev Ltda.
