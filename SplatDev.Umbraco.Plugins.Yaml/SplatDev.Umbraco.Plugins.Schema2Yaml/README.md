# Schema2Yaml

Export Umbraco document types to YAML format — reverse of Yaml2Schema.

[![NuGet](https://img.shields.io/nuget/v/SplatDev.Umbraco.Plugins.Schema2Yaml.svg)](https://www.nuget.org/packages/SplatDev.Umbraco.Plugins.Schema2Yaml)

## Compatibility

| Umbraco | .NET | Package Version |
|---------|------|-----------------|
| 13.x    | 8.0  | 2.0.7           |
| 17.x    | 10.0 | 2.0.7           |

## Installation

```sh
dotnet add package SplatDev.Umbraco.Plugins.Schema2Yaml
```

## Quick Start

No explicit registration is required — the plugin self-registers via `Schema2YamlComposer` on startup and the dashboard appears automatically in the Settings section.

## Configuration

Add to `appsettings.json`:

```json
{
  "UmbracoSchema2Yaml": {
    "ExportPath": "exports/umbraco.yml",
    "IncludeMedia": true,
    "IncludeContent": true,
    "IncludeMembers": true,
    "IncludeUsers": false
  }
}
```

## License

MIT © [SplatDev](https://github.com/SplatDev-Ltda)
