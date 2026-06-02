# SplatDev.Umbraco.Tools.T4.Plugins

CLI code generator for Umbraco plugin scaffolding. Creates Composer, Controller, Service, `package.manifest`, and language files for new Umbraco plugins.

[![NuGet](https://img.shields.io/nuget/v/SplatDev.Umbraco.Tools.T4.Plugins)](https://www.nuget.org/packages/SplatDev.Umbraco.Tools.T4.Plugins)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

## Compatibility

| .NET Target | Package Version |
|---|---|
| net8.0 | 1.0.x |
| net10.0 | 1.0.x |

## Installation

Install as a .NET global tool:

```bash
dotnet tool install -g SplatDev.Umbraco.Tools.T4.Plugins
```

## Usage

```bash
umbraco-gen-plugin [options]
```

## Generated Files

- `ComposerTemplate` — Umbraco composer/startup registration
- `ControllerTemplate` — Backoffice API controller
- `ServiceTemplate` — Plugin service class
- `PackageManifestTemplate` — `package.manifest` for Umbraco 17+
- `PackageManifestV13Template` — `package.manifest` for Umbraco 13

## Dependencies

- System.CommandLine 2.0.0-beta4.22272.1

## License

MIT © [SplatDev](https://github.com/SplatDev-Ltda)
