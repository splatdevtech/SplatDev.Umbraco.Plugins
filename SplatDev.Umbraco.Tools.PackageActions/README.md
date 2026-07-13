# SplatDev.Umbraco.Tools.PackageActions

Umbraco package actions — install, uninstall, and migration actions for plugins.
Targets Umbraco 13 (net8.0) and Umbraco 17 (net10.0).

## Installation

```bash
dotnet add package SplatDev.Umbraco.Tools.PackageActions
```

## Features

- `IPackageAction` interface for install/uninstall hooks
- Document type creation and updates
- Data type registration
- Template and stylesheet deployment
- Dictionary item seeding
- Migration action for version upgrades

## Usage

```csharp
public class InstallAction : IPackageAction
{
    public bool Execute(string packageName, XElement xml)
    {
        // Create document types, data types, templates
        return true;
    }
}
```

## License

MIT
