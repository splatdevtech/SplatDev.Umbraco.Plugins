# SplatDev.Umbraco.Tools.PackageActions

Umbraco package action helpers. Run document type, data type, template, content node, and permissions setup during package installation.

[![NuGet](https://img.shields.io/nuget/v/SplatDev.Umbraco.Tools.PackageActions)](https://www.nuget.org/packages/SplatDev.Umbraco.Tools.PackageActions)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

## Compatibility

| Umbraco Version | .NET Target | Package Version |
|---|---|---|
| Umbraco 13.x | net8.0 | 1.0.x |
| Umbraco 17.x | net10.0 | 1.0.x |

## Features

- `IPackageAction` — Contract for package installation actions
- `PackageActionRunner` — Executes registered package actions in order
- `DocumentTypeAction` — Create/configure document types during install
- `DataTypeAction` — Register data types during install
- `TemplateAction` — Create templates during install
- `ContentNodeAction` — Create content nodes during install
- `PermissionsAction` — Set up user permissions during install

## Installation

```bash
dotnet add package SplatDev.Umbraco.Tools.PackageActions
```

## License

MIT © [SplatDev](https://github.com/SplatDev-Ltda)
