# SEO

Umbraco SEO plugin supporting Umbraco 13 (net8.0) and Umbraco 17 (net10.0).

[![NuGet](https://img.shields.io/nuget/v/SplatDev.Umbraco.Plugins.SEO.svg)](https://www.nuget.org/packages/SplatDev.Umbraco.Plugins.SEO)

## Compatibility

| Umbraco | .NET | Package Version |
|---------|------|-----------------|
| 13.x    | 8.0  | 2.0.0           |
| 17.x    | 10.0 | 2.0.0           |

## Installation

```sh
dotnet add package SplatDev.Umbraco.Plugins.SEO
```

## Quick Start

Register in `Program.cs`:

```csharp
builder.CreateUmbracoBuilder()
    .AddBackOffice()
    .AddWebsite()
    .AddSEO()   // <-- add this
    .Build();
```


## Dashboard

![SEO dashboard](../docs/screenshots/seo.png)
## License

MIT © [SplatDev](https://github.com/SplatDev-Ltda)
