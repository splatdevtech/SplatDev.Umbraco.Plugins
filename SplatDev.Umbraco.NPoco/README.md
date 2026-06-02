# SplatDev.Umbraco.NPoco

NPoco database layer extensions for Umbraco CMS. Provides a base entity repository pattern with CRUD operations and event notifications built on Umbraco's NPoco infrastructure.

[![NuGet](https://img.shields.io/nuget/v/SplatDev.Umbraco.NPoco)](https://www.nuget.org/packages/SplatDev.Umbraco.NPoco)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

## Compatibility

| Umbraco Version | .NET Target | Package Version |
|---|---|---|
| Umbraco 13.x | net8.0 | 2.0.x |
| Umbraco 17.x | net10.0 | 2.0.x |

## Features

- `IRepository<T>` / `IBaseEntity` — Generic repository and entity contracts
- `BaseEntityRepository<T>` — NPoco-backed repository with insert, update, delete, and query operations
- `ActionCompletedEvent` — Notification event raised after repository actions complete

## Installation

```bash
dotnet add package SplatDev.Umbraco.NPoco
```

## License

MIT © [SplatDev](https://github.com/SplatDev-Ltda)
