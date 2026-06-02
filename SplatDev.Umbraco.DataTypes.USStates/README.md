# SplatDev.Umbraco.DataTypes.USStates

US States data type for Umbraco CMS. Registers a custom property editor that provides a dropdown list of all US states.

[![NuGet](https://img.shields.io/nuget/v/SplatDev.Umbraco.DataTypes.USStates)](https://www.nuget.org/packages/SplatDev.Umbraco.DataTypes.USStates)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

## Compatibility

| Umbraco Version | .NET Target | Package Version |
|---|---|---|
| Umbraco 13.x | net8.0 | 2.0.x |
| Umbraco 17.x | net10.0 | 2.0.x |

## Features

- `USStatesDataType` — Custom Umbraco data type with all 50 US states
- `USStatesDataTypeComposer` — Auto-registers the data type on startup
- Ready to use as a property editor on any document type

## Installation

```bash
dotnet add package SplatDev.Umbraco.DataTypes.USStates
```

The data type is automatically registered via the composer on application startup.

## License

MIT © [SplatDev](https://github.com/SplatDev-Ltda)
