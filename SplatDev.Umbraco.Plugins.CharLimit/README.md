# CharLimit

Character limit property editor for Umbraco — enforces max length on text properties with live counter display.

[![NuGet](https://img.shields.io/nuget/v/SplatDev.Umbraco.Plugins.CharLimit.svg)](https://www.nuget.org/packages/SplatDev.Umbraco.Plugins.CharLimit)

## Compatibility

| Umbraco | .NET | Package Version |
|---------|------|-----------------|
| 13.x    | 8.0  | 1.0.0           |
| 17.x    | 10.0 | 1.0.0           |

## Installation

```sh
dotnet add package SplatDev.Umbraco.Plugins.CharLimit
```

## Quick Start

Register in `Program.cs`:

```csharp
builder.CreateUmbracoBuilder()
    .AddBackOffice()
    .AddWebsite()
    .AddCharLimit()   // <-- add this
    .Build();
```

## License

MIT © [SplatDev](https://github.com/SplatDev-Ltda)
