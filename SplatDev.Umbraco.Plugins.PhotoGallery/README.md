# PhotoGallery

Photo gallery plugin for Umbraco — stores gallery albums and photos with EF Core, renders via view component.

[![NuGet](https://img.shields.io/nuget/v/SplatDev.Umbraco.Plugins.PhotoGallery.svg)](https://www.nuget.org/packages/SplatDev.Umbraco.Plugins.PhotoGallery)

## Compatibility

| Umbraco | .NET | Package Version |
|---------|------|-----------------|
| 13.x    | 8.0  | 1.0.0           |
| 17.x    | 10.0 | 1.0.0           |

## Installation

```sh
dotnet add package SplatDev.Umbraco.Plugins.PhotoGallery
```

## Quick Start

Register in `Program.cs`:

```csharp
builder.CreateUmbracoBuilder()
    .AddBackOffice()
    .AddWebsite()
    .AddPhotoGallery()   // <-- add this
    .Build();
```

## License

MIT © [SplatDev](https://github.com/SplatDev-Ltda)
