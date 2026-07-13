# SplatDev.Umbraco.EntityFramework

Generic Entity Framework Core repository with CRUD, raw SQL query, and pagination support
for Umbraco 13 (net8.0) and Umbraco 17 (net10.0) applications.

## Installation

```bash
dotnet add package SplatDev.Umbraco.EntityFramework
```

## Target Frameworks

| Framework | Umbraco | EF Core |
|---|---|---|
| `net8.0` | Umbraco 13 | EF Core 8.0 |
| `net10.0` | Umbraco 17 | EF Core 10.0 |

## Dependencies

- `Microsoft.EntityFrameworkCore` + `SqlServer` + `Design` + `Tools`
- `Microsoft.EntityFrameworkCore.DynamicLinq`
- `EFCoreSecondLevelCacheInterceptor` + `MemoryCache`
- `Umbraco.Cms.*` (core, infra, backoffice, web)
- `SplatDev.Umbraco.Pagination` (internal package)

## Features

- Generic `IRepository<TEntity>` interface for consistent data access
- `DbContextRepository<TEntity>` implementation with full CRUD
- Paginated results via `GetPagedResultsAsync` (returns `PagedResults<TEntity>`)
- Raw SQL execution via `FetchAsync(string query)`
- Entity count via `CountRecordsAsync`
- Implements `IDisposable` for proper `DbContext` cleanup

## Usage

### Register the repository

```csharp
// Program.cs — register with DI
builder.Services.AddScoped(typeof(IRepository<>), typeof(DbContextRepository<>));
```

### Inject and use

```csharp
public class MyService(IRepository<Product> repo)
{
    public async Task<PagedResults<Product>> ListAsync(int page, int pageSize)
    {
        return await repo.GetPagedResultsAsync(page, pageSize);
    }

    public async Task<Product?> GetAsync(int id)
    {
        return await repo.GetByIdAsync(id);
    }

    public async Task<Product> AddAsync(Product p)
    {
        return await repo.CreateAsync(p);
    }

    public async Task<Product> SaveAsync(Product p)
    {
        return await repo.UpdateAsync(p);
    }

    public async Task RemoveAsync(int id)
    {
        await repo.DeleteAsync(id);
    }
}
```

### Raw SQL queries

```csharp
var results = await repo.FetchAsync("SELECT * FROM Products WHERE Price > 100");
```

> **Security**: `FetchAsync` uses `FromSqlRaw`. Always use parameterized queries or
> validate input when constructing SQL strings. See the `SplatDev.Security` package
> for input sanitization helpers.

## API Reference

### `IRepository<TEntity>`

| Method | Returns | Description |
|---|---|---|
| `CreateAsync(TEntity)` | `Task<TEntity>` | Insert a new entity |
| `GetByIdAsync(int id)` | `Task<TEntity?>` | Find by primary key |
| `GetAllAsync()` | `Task<IList<TEntity>?>` | List all entities |
| `GetPagedResultsAsync(int, int)` | `Task<PagedResults<TEntity>>` | Paginated list with metadata |
| `UpdateAsync(TEntity)` | `Task<TEntity>` | Update an existing entity |
| `DeleteAsync(int id)` | `Task` | Remove by primary key |
| `FetchAsync(string query)` | `Task<IList<TEntity>>` | Execute raw SQL |
| `CountRecordsAsync()` | `Task<int?>` | Total entity count |

### `PagedResults<TEntity>` (from SplatDev.Umbraco.Pagination)

| Property | Type | Description |
|---|---|---|
| `Results` | `IList<TEntity>` | Current page items |
| `Pagination.Page` | `int` | Current page number |
| `Pagination.PageSize` | `int` | Items per page |
| `Pagination.TotalPages` | `int` | Computed page count |
| `Pagination.TotalResults` | `long` | Total matching records |

## Relationship to SplatDev.Umbraco.NPoco

This package provides EF Core integration. For Umbraco-native data access via NPoco
(the ORM Umbraco uses internally), see `SplatDev.Umbraco.NPoco`.

## License

MIT
