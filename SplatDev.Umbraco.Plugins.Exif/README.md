# Exif

EXIF metadata extractor for Umbraco media — reads camera, GPS, and image EXIF data using MetadataExtractor.

[![NuGet](https://img.shields.io/nuget/v/SplatDev.Umbraco.Plugins.Exif.svg)](https://www.nuget.org/packages/SplatDev.Umbraco.Plugins.Exif)

## Compatibility

| Umbraco | .NET | Package Version |
|---------|------|-----------------|
| 13.x    | 8.0  | 1.0.0           |
| 17.x    | 10.0 | 1.0.0           |

## Installation

```sh
dotnet add package SplatDev.Umbraco.Plugins.Exif
```

## Quick Start

Register in `Program.cs`:

```csharp
builder.CreateUmbracoBuilder()
    .AddBackOffice()
    .AddWebsite()
    .AddExif()   // <-- add this
    .Build();
```

## License

MIT © [SplatDev](https://github.com/SplatDev-Ltda)
