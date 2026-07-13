# SplatDev.Umbraco.Pagination

Generic pagination models, helpers, and extension methods for ASP.NET Core applications.
Despite the "Umbraco" in the package name, this package has **no Umbraco dependency** —
it is a pure framework library targeting `Microsoft.AspNetCore.App`.

## Installation

```bash
dotnet add package SplatDev.Umbraco.Pagination
```

## Target Frameworks

- `net8.0` (Umbraco 13)
- `net10.0` (Umbraco 17)

## Features

- `Pagination` model with page metadata (size, number, total, search)
- `PagedResults<T>` generic container for paginated data
- `PagedResultsViewModel` for serialization-safe viewmodel conversion
- Extension methods: `PagedResults<T>`, `GetTotalPages`, `Shuffle`
- `PersistenceDataType` enum (All / Content / Media) with string conversion utilities
- Lightweight `IEntity`, `IDescription`, and `IText` interfaces

## Usage

### Basic pagination with an enumerable

```csharp
using SplatDev.Umbraco.Pagination.Extensions;
using SplatDev.Umbraco.Pagination.Models;

var items = Enumerable.Range(1, 100).Select(i => new { Id = i });
int page = 2, pageSize = 10;

var paged = items.PagedResults(page, pageSize); // returns IEnumerable
int totalPages = PaginationExtensions.GetTotalPages(items.Count(), pageSize);
```

### Wrapping results with pagination metadata

```csharp
var results = new PagedResults<MyDto>
{
    Results = dtos.PagedResults(page, pageSize).ToList(),
    Pagination = new Pagination
    {
        Page = page,
        PageSize = pageSize,
        TotalPages = PaginationExtensions.GetTotalPages(total, pageSize),
        TotalResults = total,
        SearchTerm = query
    }
};
```

### Converting to a viewmodel (for JSON serialization)

```csharp
PagedResultsViewModel vm = (PagedResultsViewModel)results;
return Ok(vm);
```

### Persistence data type filtering

```csharp
using SplatDev.Umbraco.Pagination.Extensions;
using SplatDev.Umbraco.Pagination.Models;

// Parse a query string value
var type = "content".ToPersistenceDataType(); // PersistenceDataType.Content

// Use as a dropdown
<select asp-items="StringToPersistenceDataType.SELECT_LIST"></select>
```

### Shuffling a list

```csharp
var randomized = list.Shuffle();
```

## Models

| Type | Description |
|---|---|
| `Pagination` | Page metadata: `PageSize`, `Page`, `TotalPages`, `TotalResults`, `SearchTerm`, `SearchDataType` |
| `PagedResults<TEntity>` | Generic container with `Results` list, `Pagination`, `FullQueryString`, `ShowSearchTerm` |
| `PagedResultsViewModel` | Untyped viewmodel with `object` result list (safe for serialization) |
| `PersistenceDataType` | Enum: `All`, `Content`, `Media` |
| `IEntity` | Contract: `Id` (int) + `Url` (string?) |
| `IDescription` | Contract: `Description` (string) |
| `IText` | Contract: `Text` (string) |

## Extension Methods

| Method | Description |
|---|---|
| `PagedResults<T>(page, pageSize)` | Slices an `IEnumerable<T>` by page |
| `GetTotalPages(totalResults, pageSize)` | Calculates total page count |
| `Shuffle<T>()` | Fisher-Yates shuffle for `IList<T>` |
| `ToPersistenceDataType(string)` | Parses `"content"` / `"media"` / `"all"` to enum |
| `ToSearchTypeString(PersistenceDataType)` | Converts enum back to display string |
| `ToCamelCase(PersistenceDataType)` | e.g. `Content` → `"content"` |

## Dependencies

- `Microsoft.AspNetCore.App` (framework reference — no additional packages)

## Package Identity Note

This package is named `SplatDev.Umbraco.Pagination` for historical consistency
with the plugin suite, but it has **zero Umbraco dependencies**. It works in any
ASP.NET Core application. A rename to `SplatDev.Pagination` may be considered in
a future major version.

## License

MIT
