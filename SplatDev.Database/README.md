# SplatDev.Database

Generic database service abstraction using NPoco for .NET applications.
Framework-independent — no Umbraco or EF Core dependency. Targets `net8.0` and `net10.0`.

## Installation

```bash
dotnet add package SplatDev.Database
```

## Target Frameworks

- `net8.0`
- `net10.0`

## Dependencies

- `NPoco` (5.3.0) — lightweight micro-ORM
- `Microsoft.Extensions.DependencyInjection.Abstractions`

## Features

- `IDbService<T>` interface for CRUD operations
- `DbService<T>` abstract base class with NPoco implementation
- `ITable` contract for table name + primary key
- `DatabaseMigration` service for schema creation
- `PostDbCreation` hooks for seed data
- Custom attributes: `AlteredColumn`, `NvarcharMax`, `TableCreateOrder`
- `DatabaseHelper` utility for connection strings and type mapping

## Usage

### Define a table entity

```csharp
using SplatDev.Database.Interfaces;

public class Product : ITable
{
    public string TableName => "Products";
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public decimal Price { get; set; }
}
```

### Create a service

```csharp
using SplatDev.Database.Services;
using SplatDev.Database.Interfaces;

public class ProductService(IDatabase database)
    : DbService<Product>(database, "Products")
{
    public async Task<IEnumerable<Product>> GetExpensive(decimal min)
    {
        return await Query($"SELECT * FROM Products WHERE Price > {min}");
    }
}
```

### Register with DI

```csharp
// Program.cs
builder.Services.AddScoped(sp => new Database(
    sp.GetRequiredService<IConfiguration>().GetConnectionString("Default")));

builder.Services.AddScoped<ProductService>();
```

### CRUD operations

```csharp
var product = new Product { Name = "Widget", Price = 9.99m };

await svc.Insert(product);          // returns true/false
var item = await svc.GetById(1);    // returns Product
await svc.Update(item, 1);          // returns true/false
await svc.Delete(1);                // returns true/false
bool exists = await svc.Exists(1);  // true/false
```

### Bulk operations

```csharp
var all = await svc.GetAll();           // SELECT * FROM Products
var many = await svc.GetAllIds(1,2,3); // IN clause
await svc.DeleteAll();                  // DELETE FROM Products
```

> **Security**: `DeleteAll()`, `GetAll()`, `GetAllIds()`, and `Query()` construct SQL
> strings. For user-facing queries, always parameterize or use the parameterized
> NPoco methods directly on `Database`. The `GetAllIds` method uses inline `IN`
> clauses — prefer `service.Query()` with parameterized SQL for dynamic input.

## API Reference

### `IDbService<T>`

| Method | Returns | Description |
|---|---|---|
| `GetById(int)` | `Task<T>` | Fetch by primary key |
| `Insert(T)` | `Task<bool>` | Insert entity |
| `Update(T, int)` | `Task<bool>` | Update by PK |
| `Delete(int)` | `Task<bool>` | Delete by PK |
| `Exists(int)` | `Task<bool>` | Check existence |

### `DbService<T>` (additional virtual methods)

| Method | Description |
|---|---|
| `GetAll()` | `SELECT * FROM {table}` |
| `GetAllIds(params int[])` | `SELECT * FROM {table} WHERE Id IN (...)` |
| `GetById(string, params string[])` | NPoco `Single` with navigation properties |
| `Query(string sql)` | Raw SQL execution |
| `DeleteAll()` | `DELETE FROM {table}` |

### `ITable`

| Property | Type | Description |
|---|---|---|
| `TableName` | `string` | Database table name |
| `Id` | `int` | Primary key |

## Relationship to SplatDev.Umbraco.NPoco

`SplatDev.Umbraco.NPoco` provides Umbraco-scoped repositories (IScopeProvider).
`SplatDev.Database` provides raw NPoco access without Umbraco — suitable for
standalone services, worker processes, and non-Umbraco applications.

## License

MIT
