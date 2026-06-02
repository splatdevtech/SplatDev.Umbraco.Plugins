# SplatDev.Database

Generic database service abstractions using NPoco for .NET applications. Provides a common `IDbService` interface, migration helpers, and attribute-driven schema management.

[![NuGet](https://img.shields.io/nuget/v/SplatDev.Database)](https://www.nuget.org/packages/SplatDev.Database)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

## Compatibility

| .NET Target | Package Version |
|---|---|
| net8.0 | 1.0.x |
| net10.0 | 1.0.x |

## Features

- `IDbService` / `DbService` — Generic database operations abstraction
- `ITable` — Table entity contract
- Schema migration support via `DatabaseMigration`
- Post-creation hooks via `PostDbCreation`
- Custom attributes: `AlteredColumnAttribute`, `NvarcharMaxAttribute`, `TableCreateOrderAttribute`
- Reflection-based attribute and helper utilities

## Installation

```bash
dotnet add package SplatDev.Database
```

## Key Interfaces

```csharp
public interface IDbService
{
    // Generic CRUD operations over NPoco
}

public interface ITable
{
    // Contract for database table entities
}
```

## Dependencies

- NPoco 5.3.0
- Microsoft.Extensions.DependencyInjection.Abstractions 8.0.0
- SplatDev.Reflection.Helpers (project reference)

## License

MIT © [SplatDev](https://github.com/SplatDev-Ltda)
