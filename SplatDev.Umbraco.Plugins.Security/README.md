# Security

Umbraco security headers plugin supporting Umbraco 13 (net8.0) and Umbraco 17 (net10.0).

[![NuGet](https://img.shields.io/nuget/v/SplatDev.Umbraco.Plugins.Security.svg)](https://www.nuget.org/packages/SplatDev.Umbraco.Plugins.Security)

## Compatibility

| Umbraco | .NET | Package Version |
|---------|------|-----------------|
| 13.x    | 8.0  | 2.0.0           |
| 17.x    | 10.0 | 2.0.0           |

## Installation

```sh
dotnet add package SplatDev.Umbraco.Plugins.Security
```

## Quick Start

Register in `Program.cs`:

```csharp
builder.CreateUmbracoBuilder()
    .AddBackOffice()
    .AddWebsite()
    .AddSecurity()   // <-- add this
    .Build();
```

## Configuration

Add to `appsettings.json`:

```json
{
  "Security": {
    "ContentSecurityPolicy": "default-src 'self'",
    "XFrameOptions": "SAMEORIGIN",
    "ReferrerPolicy": "no-referrer-when-downgrade"
  }
}
```

## License

MIT © [SplatDev](https://github.com/SplatDev-Ltda)
