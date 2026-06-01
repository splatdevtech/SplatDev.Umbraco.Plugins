# Yaml2Schema

Import YAML document type definitions into Umbraco — creates/updates content types from YAML files.

[![NuGet](https://img.shields.io/nuget/v/SplatDev.Umbraco.Plugins.Yaml2Schema.svg)](https://www.nuget.org/packages/SplatDev.Umbraco.Plugins.Yaml2Schema)

## Compatibility

| Umbraco | .NET | Package Version |
|---------|------|-----------------|
| 13.x    | 8.0  | 1.0.38          |
| 17.x    | 10.0 | 1.0.38          |

## Installation

```sh
dotnet add package SplatDev.Umbraco.Plugins.Yaml2Schema
```

## Quick Start

Register in `Program.cs`:

```csharp
builder.CreateUmbracoBuilder()
    .AddBackOffice()
    .AddWebsite()
    .AddYaml2Schema()   // <-- add this
    .Build();
```

## Configuration

Add to `appsettings.json`:

```json
{
  "UmbracoYaml2Schema": {
    "YamlPath": "umbraco.yml",
    "RunOnStartup": false,
    "OverwriteExisting": false
  }
}
```

## License

MIT © [SplatDev](https://github.com/SplatDev-Ltda)
