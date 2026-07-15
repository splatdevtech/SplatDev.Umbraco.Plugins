# SplatDev.Umbraco.Tools.Packager

.NET CLI tool for building Umbraco NuGet packages. Generates `umbraco-package.json` manifests, bundles `App_Plugins` assets, and runs `dotnet pack` — all from the command line.

[![NuGet](https://img.shields.io/nuget/v/SplatDev.Umbraco.Tools.Packager.svg)](https://www.nuget.org/packages/SplatDev.Umbraco.Tools.Packager)

## Compatibility

| .NET | Tool Version |
|------|-------------|
| 8.0  | 1.0.0       |
| 10.0 | 1.0.0       |

Multi-targets `net8.0;net10.0`. Packaged as a .NET global tool.

## Installation

```sh
dotnet tool install --global SplatDev.Umbraco.Tools.Packager
```

Then use the `umbraco-packager` command:

```sh
umbraco-packager --help
```

## Commands

### `pack` — full build

Generates the manifest, bundles App_Plugins assets, and runs `dotnet pack` to produce a `.nupkg`.

```sh
umbraco-packager pack --name "MyPlugin" --version "2.1.0" --output "./dist"
```

| Option | Required | Default | Description |
|--------|----------|---------|-------------|
| `--name` | Yes | — | Package name (used in `umbraco-package.json` and `App_Plugins/<name>/`) |
| `--version` | No | `1.0.0` | Semantic version |
| `--output` | No | `./nupkg` | Output directory for the `.nupkg` file |

### `manifest` — manifest only

Generates only the `umbraco-package.json` manifest in `App_Plugins/<name>/` without running `dotnet pack`.

```sh
umbraco-packager manifest --name "MyPlugin" --version "2.1.0"
```

| Option | Required | Default | Description |
|--------|----------|---------|-------------|
| `--name` | Yes | — | Package name |
| `--version` | No | `1.0.0` | Semantic version |
| `--output` | No | `.` | Output directory (manifest is written to `<output>/App_Plugins/<name>/`) |

### `clean` — remove artifacts

Deletes the nupkg output directory.

```sh
umbraco-packager clean --output "./dist"
```

| Option | Required | Default | Description |
|--------|----------|---------|-------------|
| `--output` | No | `./nupkg` | Directory to delete |

## What happens when you run `pack`

1. **`ManifestGenerator`** — writes `App_Plugins/<name>/umbraco-package.json` with name, version, and an empty `extensions` array.
2. **`AssetBundler`** — copies the project's `App_Plugins/` tree into the output directory for inclusion in the NuGet package.
3. **`dotnet pack`** — invokes the .NET SDK pack target, passing `--output` and `/p:Version=<version>`. Stdout and stderr are forwarded to the console.

If any step fails (e.g. `dotnet pack` exits with a non-zero code), the tool throws an error and halts.

## Dependencies

| Package | Purpose |
|---------|---------|
| `System.CommandLine` (2.0.0-beta4) | CLI argument parsing and command routing |

No Umbraco runtime dependencies. This is a build-time/CI tool.

## Usage in CI

```yaml
# GitHub Actions example
- name: Pack Umbraco plugin
  run: umbraco-packager pack --name "MyPlugin" --version "${{ github.ref_name }}" --output "./artifacts"
```

```powershell
# Local / scripted
umbraco-packager pack --name "SplatDev.Umbraco.Plugins.Backups" --version "2.3.0" --output ".\release"
```

---

**SplatDev.Umbraco.Tools.Packager** — part of the [SplatDev.Umbraco.Plugins](https://github.com/SplatDev-Ltda/SplatDev.Umbraco.Plugins) suite. Licensed under MIT. © SplatDev Ltda.
