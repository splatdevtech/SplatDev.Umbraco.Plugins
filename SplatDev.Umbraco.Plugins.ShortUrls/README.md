# ShortUrls

Umbraco short URLs plugin supporting Umbraco 13 (net8.0) and Umbraco 17 (net10.0).

[![NuGet](https://img.shields.io/nuget/v/SplatDev.Umbraco.Plugins.ShortUrls.svg)](https://www.nuget.org/packages/SplatDev.Umbraco.Plugins.ShortUrls)

## Compatibility

| Umbraco | .NET | Package Version |
|---------|------|-----------------|
| 13.x    | 8.0  | 2.0.0           |
| 17.x    | 10.0 | 2.0.0           |

## Installation

```sh
dotnet add package SplatDev.Umbraco.Plugins.ShortUrls
```

## Quick Start

Register in `Program.cs`:

```csharp
builder.CreateUmbracoBuilder()
    .AddBackOffice()
    .AddWebsite()
    .AddShortUrls()   // <-- add this
    .Build();
```


## Dashboard

![Short URLs dashboard](../docs/screenshots/short-urls.png)
## License

MIT © [SplatDev](https://github.com/SplatDev-Ltda)
