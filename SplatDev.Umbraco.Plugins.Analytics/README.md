# Analytics

Google Analytics integration for Umbraco — store tracking/measurement ID in settings, inject GA4 script via view component. Supports Umbraco 13 (net8.0) and Umbraco 17 (net10.0).

[![NuGet](https://img.shields.io/nuget/v/SplatDev.Umbraco.Plugins.Analytics.svg)](https://www.nuget.org/packages/SplatDev.Umbraco.Plugins.Analytics)

## Compatibility

| Umbraco | .NET | Package Version |
|---------|------|-----------------|
| 13.x    | 8.0  | 2.0.0           |
| 17.x    | 10.0 | 2.0.0           |

## Installation

```sh
dotnet add package SplatDev.Umbraco.Plugins.Analytics
```

## Quick Start

Register in `Program.cs`:

```csharp
builder.CreateUmbracoBuilder()
    .AddBackOffice()
    .AddWebsite()
    .AddAnalytics()   // <-- add this
    .Build();
```

## Configuration

Add to `appsettings.json`:

```json
{
  "Analytics": {
    "MeasurementId": "G-XXXXXXXXXX"
  }
}
```

## License

MIT © [SplatDev](https://github.com/SplatDev-Ltda)
