# Gdrp

GDPR compliance plugin for Umbraco — cookie consent banner, data export, and right to erasure requests. Supports Umbraco 13 (net8.0) and Umbraco 17 (net10.0).

[![NuGet](https://img.shields.io/nuget/v/SplatDev.Umbraco.Plugins.Gdrp.svg)](https://www.nuget.org/packages/SplatDev.Umbraco.Plugins.Gdrp)

## Compatibility

| Umbraco | .NET | Package Version |
|---------|------|-----------------|
| 13.x    | 8.0  | 2.0.0           |
| 17.x    | 10.0 | 2.0.0           |

## Installation

```sh
dotnet add package SplatDev.Umbraco.Plugins.Gdrp
```

## Quick Start

Register in `Program.cs`:

```csharp
builder.CreateUmbracoBuilder()
    .AddBackOffice()
    .AddWebsite()
    .AddGdrp()   // <-- add this
    .Build();
```


## Dashboard

![GDPR Compliance dashboard](../docs/screenshots/gdpr-compliance.png)
## License

MIT © [SplatDev](https://github.com/SplatDev-Ltda)
