# SvgViewer

SVG file viewer plugin for Umbraco — renders inline SVG files from the Umbraco media library safely.

[![NuGet](https://img.shields.io/nuget/v/SplatDev.Umbraco.Plugins.SvgViewer.svg)](https://www.nuget.org/packages/SplatDev.Umbraco.Plugins.SvgViewer)

## Compatibility

| Umbraco | .NET | Package Version |
|---------|------|-----------------|
| 13.x    | 8.0  | 1.0.0           |
| 17.x    | 10.0 | 1.0.0           |

## Installation

```sh
dotnet add package SplatDev.Umbraco.Plugins.SvgViewer
```

## Quick Start

Register in `Program.cs`:

```csharp
builder.CreateUmbracoBuilder()
    .AddBackOffice()
    .AddWebsite()
    .AddSvgViewer()   // <-- add this
    .Build();
```

## License

MIT © [SplatDev](https://github.com/SplatDev-Ltda)
