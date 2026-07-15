# SplatDev.Umbraco.Pagination

Shared pagination DTOs and helpers for .NET applications. Despite the "Umbraco" prefix, this package has **no Umbraco dependency** — it is framework-agnostic and serves as the most-referenced foundation package across the SplatDev ecosystem (consumed by `SplatDev.Umbraco.EntityFramework`, `SplatDev.Umbraco.Examine`, `SplatDev.Umbraco.NPoco`, and others).

[![NuGet](https://img.shields.io/nuget/v/SplatDev.Umbraco.Pagination.svg)](https://www.nuget.org/packages/SplatDev.Umbraco.Pagination)

## Compatibility

| .NET | Package Version |
|------|-----------------|
| 8.0  | 2.0.0           |
| 10.0 | 2.0.0           |

Multi-targets `net8.0;net10.0`. Published to nuget.org.

## Installation

```sh
dotnet add package SplatDev.Umbraco.Pagination
```

## What's implemented

### Models — `SplatDev.Umbraco.Pagination.Models`

| Type | Kind | Description |
|------|------|-------------|
| `Pagination` | `class` | Paging metadata: `PageSize`, `Page`, `TotalPages`, `TotalResults`, `SearchTerm`, `SearchDataType` |
| `PagedResults<TEntity>` | `class` | Generic paged wrapper combining `IList<TEntity>? Results` + `Pagination` + `bool FullQueryString` + `bool ShowSearchTerm` |
| `PagedResultsViewModel` | `class` | Untyped (legacy) variant with `List<object>? Results` for Razor views that can't consume generics |
| `PersistenceDataType` | `enum` | `All`, `Media`, `Content` — persisted data category |
| `IDescription` | `interface` | `string Description { get; set; }` |
| `IText` | `interface` | `string Text { get; set; }` |
| `IEntity` | `interface` | `int Id { get; set; }` + `string? Url { get; set; }` |

### Extensions — `SplatDev.Umbraco.Pagination.Extensions`

| Method | Signature | Description |
|--------|-----------|-------------|
| `GetTotalPages` | `int GetTotalPages(long totalResults, int pageSize)` | Calculates total pages from result count and page size. Returns at least 1. |
| `PagedResults<T>` | `IEnumerable<T> PagedResults<T>(this IEnumerable<T>, int page, int pageSize)` | LINQ extension: `Skip((page - 1) * pageSize).Take(pageSize)` |
| `Shuffle<T>` | `IList<T> Shuffle<T>(this IList<T>)` | Fisher-Yates shuffle using `Random.Shared` (.NET 6+). Thread-safe. |
| `ToPersistenceDataType` | `PersistenceDataType ToPersistenceDataType(this string)` | Maps search-type strings (`"Content"`, `"Media"`, `"All"`) to enum |
| `ToSearchTypeString` | `string ToSearchTypeString(this PersistenceDataType)` | Reverse map: enum → search-type string |
| `ToCamelCase` | `string ToCamelCase(this PersistenceDataType)` | `PersistenceDataType.Content` → `"content"` |

Also exposes `SEARCH_TYPES` (`string[]`) and `SELECT_LIST` (`SelectList`) from the `StringToPersistenceDataType` static class.

## Usage

```csharp
using SplatDev.Umbraco.Pagination.Extensions;
using SplatDev.Umbraco.Pagination.Models;

// Calculate total pages
int totalPages = PaginationExtensions.GetTotalPages(totalResults: 250, pageSize: 20);
// => 13

// Page a collection
var results = allItems.PagedResults(page: 3, pageSize: 20);

// Build a paged response
var response = new PagedResults<MyEntity>
{
    Results = results.ToList(),
    Pagination = new Pagination
    {
        Page = 3,
        PageSize = 20,
        TotalPages = totalPages,
        TotalResults = 250
    }
};
```

## Design notes

### Package naming

This package is named `SplatDev.Umbraco.Pagination` for historical reasons — it originated as part of the Umbraco plugin
ecosystem. Since it has no Umbraco dependency and is consumed by non-Umbraco packages, renaming to `SplatDev.Pagination`
has been proposed. Until a rename is executed (with a package deprecation + new package migration), this name remains.

### `PagedResultsViewModel` (untyped)

`PagedResultsViewModel` exists exclusively for legacy Razor views that cannot consume the generic `PagedResults<TEntity>`.
New code should use `PagedResults<T>` directly. `PagedResults<T>` includes an explicit conversion operator to
`PagedResultsViewModel` if needed.

### `SearchDataType` vs `PersistenceDataType`

The `Pagination.SearchDataType` property is typed as `string?` while the enum is named `PersistenceDataType`.
This terminology drift is a known issue — the string field stores the "search" representation for presentation
layers while `PersistenceDataType` is the canonical model. The `StringToPersistenceDataType` helper class bridges
these two worlds. Future versions should consolidate under a single name.

### `Shuffle<T>` placement

`Shuffle<T>` currently lives in `PaginationExtensions`, which is a misplacement — it is a general-purpose collection
extension unrelated to pagination. A future refactor should move it to a separate `SplatDev.Collections` or
`SplatDev.Extensions` utility package.

## Dependencies

| Dependency | Purpose |
|-----------|---------|
| `Microsoft.AspNetCore.App` (framework reference) | Provides `SelectList` for `StringToPersistenceDataType` |

No Umbraco or third-party package dependencies.

---

**SplatDev.Umbraco.Pagination** — part of the [SplatDev.Umbraco.Plugins](https://github.com/SplatDev-Ltda/SplatDev.Umbraco.Plugins) suite. Licensed under MIT. © SplatDev Ltda.
