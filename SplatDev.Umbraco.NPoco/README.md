# SplatDev.Umbraco.NPoco

Umbraco-native data repository using NPoco (Umbraco's internal ORM) with built-in audit
events for Umbraco 13 (net8.0) and Umbraco 17 (net10.0).

## Installation

```bash
dotnet add package SplatDev.Umbraco.NPoco
```

## Target Frameworks

| Framework | Umbraco |
|---|---|
| `net8.0` | Umbraco 13 |
| `net10.0` | Umbraco 17 |

## Features

- Full CRUD with `IScopeProvider` integration (Umbraco's scoping mechanism)
- `BaseEntityRepository<T>` abstract class — implement once, get all operations
- Audit events via `ActionCompleted` delegate (Insert, Update, Delete)
- Bulk operations (Insert, Update, Delete by IDs or entities)
- `Fetch(string query)` — raw SQL execution
- `Get(string column, object? value)` — parameterized single-column lookup
- Paginated queries with `GetPagedResultsByQuery` + ordering support
- `IBaseEntity` contract ensures entities carry an `Id` property

## Usage

### Define an entity

```csharp
public class Product : IBaseEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public decimal Price { get; set; }
}
```

### Create a repository

```csharp
public class ProductRepository(IScopeProvider scopeProvider)
    : BaseEntityRepository<Product>(scopeProvider)
{
    // All CRUD inherited — add custom queries here
    public IEnumerable<Product> GetByPrice(decimal min)
    {
        using var scope = scopeProvider.CreateScope(autoComplete: true);
        return scope.Database.Query<Product>()
            .Where(x => x.Price >= min)
            .ToList();
    }
}
```

### Register and use

```csharp
// Program.cs
builder.Services.AddScoped<ProductRepository>();

// Controller/Service
public class ProductsController(ProductRepository repo)
{
    public IActionResult Get(int id)
    {
        var product = repo.Get(id);
        return product is null ? NotFound() : Ok(product);
    }
}
```

### Audit events

```csharp
repo.ActionCompleted += (sender, e) =>
{
    _logger.LogInformation("{AuditType} on entity {Id} by user {UserId}",
        e.AuditType, e.Id, e.UserId);
};
```

### Paginated queries

```csharp
var query = scope.Database.Query<Product>();
var results = repo.GetPagedResultsByQuery(
    query, pageIndex: 0, pageSize: 20,
    out long totalRecords, filter: null, ordering: null);
```

## API Reference

### `IRepository<T>`

| Method | Description |
|---|---|
| `Get(int? id)` | Single entity by PK |
| `Get(string column, object? value)` | Single entity by column (parameterized) |
| `GetAll()` | All entities |
| `GetMany(params int[] ids)` | Multiple entities by PKs |
| `GetPagedResultsByQuery(...)` | Paginated, filtered, ordered results |
| `Insert(T)` / `InsertBulk(List<T>)` | Create one or many |
| `Update(T)` / `UpdateBulk(List<T>)` | Update one or many |
| `Delete(int)` / `Delete(T)` | Remove by PK or entity |
| `DeleteBulk(IList<int>)` / `DeleteBulk(IList<T>)` | Batch delete |
| `Fetch(string query)` | Raw SQL execution |
| `Count()` | Total count |
| `Exists(int id)` | Existence check |

### `ActionCompletedEvent`

| Property | Type | Description |
|---|---|---|
| `AuditType` | `AuditType` | `Save` or `Delete` |
| `Id` | `int` | Entity primary key |
| `Entity` | `object` | The entity instance |
| `UserId` | `Guid?` | User performing the action |
| `Message` | `string?` | Optional description |
| `Log` | `bool` | Whether to emit a log |

> **Security**: `Fetch(string)` executes raw SQL. Prefer parameterized methods (`Get`,
> `GetMany`, `GetPagedResultsByQuery`) for user-facing queries. `Get(string, object)`
> uses NPoco's parameterized `@0` syntax.

## Relationship to SplatDev.Umbraco.EntityFramework

This package provides NPoco integration (Umbraco-native ORM). For Entity Framework Core
integration, see `SplatDev.Umbraco.EntityFramework`.

## License

MIT
