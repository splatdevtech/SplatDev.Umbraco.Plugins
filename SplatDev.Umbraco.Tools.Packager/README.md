# SplatDev.Umbraco.Tools.Packager

Umbraco package builder and NuGet packager tool. Targets `net8.0` and `net10.0`.

## Installation

```bash
dotnet add package SplatDev.Umbraco.Tools.Packager
```

## Features

- Generate `package.xml` from assembly manifest
- Bundle static assets (views, CSS, JS, config)
- Create NuGet `.nupkg` for Umbraco marketplace
- Version stamping and dependency resolution

## Usage

```bash
dotnet packager --project MyPlugin.csproj --output ./dist/
```

## License

MIT
