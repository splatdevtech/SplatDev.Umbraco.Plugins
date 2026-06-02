# SplatDev.Umbraco.Pagination

Pagination helpers for Umbraco CMS. Shared models and extension methods for paginating content, search results, and database queries.

[![NuGet](https://img.shields.io/nuget/v/SplatDev.Umbraco.Pagination)](https://www.nuget.org/packages/SplatDev.Umbraco.Pagination)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

## Compatibility

| Umbraco Version | .NET Target | Package Version |
|---|---|---|
| Umbraco 13.x | net8.0 | 2.0.x |
| Umbraco 17.x | net10.0 | 2.0.x |

## Features

- `PagedResults<T>` — Generic paged result container with total count and page metadata
- `Pagination` — Pagination parameters model (page number, page size, sort)
- `PaginationExtensions` — Extension methods for paginating `IQueryable` and `IEnumerable`
- `StringToPersistenceDataType` — String-to-data-type conversion utilities
- `IEntity` / `IText` / `IDescription` — Base entity contracts

## Installation

```bash
dotnet add package SplatDev.Umbraco.Pagination
```

## Dependencies

- Microsoft.AspNetCore.App (framework reference)

## License

MIT © [SplatDev](https://github.com/SplatDev-Ltda)
