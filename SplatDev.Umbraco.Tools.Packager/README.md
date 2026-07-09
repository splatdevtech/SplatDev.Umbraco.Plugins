# SplatDev.Umbraco.Tools.Packager

A .NET CLI tool that automates Umbraco plugin packaging: generates `package.manifest`, bundles `App_Plugins` assets, and wraps `dotnet pack`.

## Install

```sh
dotnet tool install --global SplatDev.Umbraco.Tools.Packager
```

## Commands

### `umbraco-packager pack`

Full build pipeline — generate manifest, bundle assets, run `dotnet pack`.

| Option | Required | Default | Description |
|--------|----------|---------|-------------|
| `--name` | Yes | — | Package name (used in manifest and NuGet ID) |
| `--version` | No | `1.0.0` | Semantic version for the package |
| `--output` | No | `./nupkg` | Directory for the `.nupkg` output |

```sh
umbraco-packager pack --name MyPlugin --version 2.1.0 --output ./dist
```

### `umbraco-packager manifest`

Generate `App_Plugins/{name}/package.manifest` only — skip asset bundling and `dotnet pack`.

| Option | Required | Default | Description |
|--------|----------|---------|-------------|
| `--name` | Yes | — | Package name |
| `--version` | No | `1.0.0` | Semantic version |
| `--output` | No | `.` | Directory for the generated manifest |

```sh
umbraco-packager manifest --name MyPlugin --version 1.0.0
```

### `umbraco-packager clean`

Remove generated `App_Plugins` output and `.nupkg` artifacts.

| Option | Required | Default | Description |
|--------|----------|---------|-------------|
| `--output` | No | `./nupkg` | Output directory to clean |

```sh
umbraco-packager clean --output ./dist
```

## What It Does

1. **Manifest generation** — creates a `package.manifest` (JSON) in `App_Plugins/{name}/` with the package name, version, and metadata.
2. **Asset bundling** — copies all files from the source `App_Plugins` directory into the output structure, ready for NuGet packaging.
3. **NuGet pack** — runs `dotnet pack` with the specified version, producing a `.nupkg` ready for NuGet.org or a private feed.

## Requirements

- .NET 8.0 or .NET 10.0 SDK
- A .NET project with the `SplatDev.Umbraco.Tools.Packager` tool reference (or global install)

## Known Limitations

- **System.CommandLine 2.0.0-beta4** — the tool currently uses a 2022 beta of System.CommandLine. Upgrading to the stable 2.0.x release requires migrating from the beta handler API (`SetHandler` with inline lambdas) to the stable fluent builder pattern. Tracked in the project backlog.

## License

MIT
