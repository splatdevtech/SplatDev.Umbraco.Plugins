# SplatDev.Umbraco.EntityFramework

Entity Framework Core integration for Umbraco CMS. Provides a generic repository pattern, second-level caching, and dynamic LINQ support on top of EF Core.

[![NuGet](https://img.shields.io/nuget/v/SplatDev.Umbraco.EntityFramework)](https://www.nuget.org/packages/SplatDev.Umbraco.EntityFramework)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

## Compatibility

| Umbraco Version | .NET Target | EF Core | Package Version |
|---|---|---|---|
| Umbraco 13.x | net8.0 | 8.0.20 | 2.0.x |
| Umbraco 17.x | net10.0 | 10.0.7 | 2.0.x |

## Features

- `IRepository<T>` — Generic repository interface
- `DbContextRepository<T>` — EF Core repository implementation with CRUD operations
- Second-level cache support via `EFCoreSecondLevelCacheInterceptor`
- Dynamic LINQ queries via `Microsoft.EntityFrameworkCore.DynamicLinq`
- SQL Server provider with design-time and migration tooling

## Installation

```bash
dotnet add package SplatDev.Umbraco.EntityFramework
```

## Dependencies

- Microsoft.EntityFrameworkCore.SqlServer
- Microsoft.EntityFrameworkCore.Design
- Microsoft.EntityFrameworkCore.Tools
- Microsoft.EntityFrameworkCore.DynamicLinq
- EFCoreSecondLevelCacheInterceptor 5.3.2
- SplatDev.Umbraco.Pagination (project reference)

## License

MIT © [SplatDev](https://github.com/SplatDev-Ltda)
