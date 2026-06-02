# SplatDev.Umbraco.Tools.Packager

CLI tool for creating Umbraco NuGet packages. Generates `package.manifest` files, bundles `App_Plugins` assets, and wraps `dotnet pack` for streamlined package creation.

[![NuGet](https://img.shields.io/nuget/v/SplatDev.Umbraco.Tools.Packager)](https://www.nuget.org/packages/SplatDev.Umbraco.Tools.Packager)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

## Compatibility

| .NET Target | Package Version |
|---|---|
| net8.0 | 1.0.x |
| net10.0 | 1.0.x |

## Installation

Install as a .NET global tool:

```bash
dotnet tool install -g SplatDev.Umbraco.Tools.Packager
```

## Usage

```bash
umbraco-packager [options]
```

## Features

- `ManifestGenerator` — Generate `package.manifest` for Umbraco packages
- `AssetBundler` — Bundle `App_Plugins` static assets into the NuGet package
- `PackageBuilder` — Orchestrate the full build-and-pack workflow

## Dependencies

- System.CommandLine 2.0.0-beta4.22272.1

## License

MIT © [SplatDev](https://github.com/SplatDev-Ltda)
