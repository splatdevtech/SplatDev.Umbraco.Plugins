# Slider

Image slider plugin for Umbraco — stores slide data with EF Core, renders via configurable view component.

[![NuGet](https://img.shields.io/nuget/v/SplatDev.Umbraco.Plugins.Slider.svg)](https://www.nuget.org/packages/SplatDev.Umbraco.Plugins.Slider)

## Compatibility

| Umbraco | .NET | Package Version |
|---------|------|-----------------|
| 13.x    | 8.0  | 1.0.0           |
| 17.x    | 10.0 | 1.0.0           |

## Installation

```sh
dotnet add package SplatDev.Umbraco.Plugins.Slider
```

## Quick Start

Register in `Program.cs`:

```csharp
builder.CreateUmbracoBuilder()
    .AddBackOffice()
    .AddWebsite()
    .AddSlider()   // <-- add this
    .Build();
```

## License

MIT © [SplatDev](https://github.com/SplatDev-Ltda)
