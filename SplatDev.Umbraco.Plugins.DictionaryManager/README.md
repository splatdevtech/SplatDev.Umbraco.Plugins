# DictionaryManager

Dictionary import/export/CRUD manager for Umbraco — full rewrite of the Umbraco 8 plugin using ILocalizationService. Supports Umbraco 13 (net8.0) and Umbraco 17 (net10.0).

[![NuGet](https://img.shields.io/nuget/v/SplatDev.Umbraco.Plugins.DictionaryManager.svg)](https://www.nuget.org/packages/SplatDev.Umbraco.Plugins.DictionaryManager)

## Compatibility

| Umbraco | .NET | Package Version |
|---------|------|-----------------|
| 13.x    | 8.0  | 2.0.0           |
| 17.x    | 10.0 | 2.0.0           |

## Installation

```sh
dotnet add package SplatDev.Umbraco.Plugins.DictionaryManager
```

## Quick Start

Register in `Program.cs`:

```csharp
builder.CreateUmbracoBuilder()
    .AddBackOffice()
    .AddWebsite()
    .AddDictionaryManager()   // <-- add this
    .Build();
```

## License

MIT © [SplatDev](https://github.com/SplatDev-Ltda)

## Screenshots

The following screenshot shows the plugin in the Umbraco backoffice:

![Umbraco backoffice screenshot](https://raw.githubusercontent.com/splatdevtech/SplatDev.Umbraco.Plugins/master/assets/screenshots/SplatDev.Umbraco.Plugins.DictionaryManager-dashboard.png)
