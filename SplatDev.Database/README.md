# SplatDev.Database

Generic database service abstractions using [NPoco](https://github.com/schotime/NPoco) for .NET applications. Provides an async CRUD base class, schema introspection helpers, and T-SQL type mapping — all MSSQL-flavored.

## Install

```bash
dotnet add package SplatDev.Database
```

Requires NPoco 5.3+ and registers via `Microsoft.Extensions.DependencyInjection`.

## What's implemented

### `IDbService<T>` — async CRUD contract

```csharp
public interface IDbService<T>
{
    Task<T> GetById(int id);
    Task<bool> Update(T data, int primaryKeyValue);
    Task<bool> Delete(int id);
    Task<bool> Insert(T data);
    Task<bool> Exists(int id);
}
```

### `DbService<T>` — abstract base (real async in v1.0.0)

- `GetAll()` / `GetAllIds(params int[])` — fetch all records or filtered by ID list
- `GetById(int id)` / `GetById(string id, params string[] navigationProperties)` — single-record lookup
- `Query(string sql)` — arbitrary parameterized SQL
- `Insert(T data)` / `Update(T data, int primaryKeyValue)` — async write operations
- `Delete(int id)` / `DeleteAll()` — row and table-level deletion
- `Exists(int id)` — fast existence check

All methods use NPoco 5.3.0 async API (`FetchAsync`, `SingleAsync`, `InsertAsync`, `UpdateAsync`, `ExecuteAsync`). No `Task.FromResult` wrapping.

### `ITable` — table marker interface

```csharp
public interface ITable { string TableName { get; } }
```

Entities implement `ITable` to declaratively identify their backing table.

### Attributes

| Attribute | Purpose |
|-----------|---------|
| `[NvarcharMax]` | Marks a property to be created as `NVARCHAR(MAX)` during schema generation |
| `[TableCreateOrder]` | Specifies the order in which tables are created during migration |
| `[AlteredColumn]` | Tracks column alterations for incremental schema evolution |

### `DatabaseHelper` — schema introspection

- `ColumnExists(this Database, ITable, PropertyInfo)` — checks if a column exists in the target table
- `GetTableColumns(this Type)` — returns the set of non-ignored properties for a table type
- `GetTableName(Type)` — resolves the table name from a type
- `TableExistsQuery(this Database, ITable)` — checks if a table already exists

### `AttributeHelper` — NPoco/database attribute reflection

- `NvarcharMaxProperties<Type>(this Type)` — finds all properties decorated with `[NvarcharMax]`
- `OrderByCreateOrderAttribute(this IEnumerable<Type>)` — sorts types by `[TableCreateOrder]` value

### `DatabaseTypes` — .NET to T-SQL type dictionary

Maps .NET types (`string`, `int`, `DateTime`, `bool`, `Guid`, `decimal`, etc.) to their T-SQL equivalents (`NVARCHAR(MAX)`, `INT`, `DATETIME`, `BIT`, `UNIQUEIDENTIFIER`, `DECIMAL(18,2)`). Used internally by schema generation and code-first tooling.

## Usage

### Deriving a service

```csharp
public class ProductService : DbService<Product>
{
    public ProductService(IDatabase database)
        : base(database, "Products") { }
}

// Usage via DI
var service = new ProductService(database);
var products = await service.GetAll();
var product = await service.GetById(1);
await service.Insert(new Product { Name = "Widget" });
await service.Update(existing, existing.Id);
await service.Delete(42);
```

### Applying `[TableCreateOrder]`

```csharp
[TableCreateOrder(1)]
public class Product : ITable
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string TableName => "Products";
}

[TableCreateOrder(2)]  // created after Product
public class Order : ITable { ... }
```

### Paged fetch

```csharp
var results = await service.Query(
    "WHERE Category = @0 ORDER BY Name OFFSET @1 ROWS FETCH NEXT @2 ROWS ONLY",
    "widgets", 0, 50);
```

## Security

- All SQL uses NPoco parameterized queries (`@0`, `@1`, etc.) or NPoco ORM methods (`Insert`, `Single`, `Fetch`).
- No string concatenation for SQL values.
- Dynamic table names in `DbService` are validated against schema (see `PostDbCreation`).

## Dependencies

- `NPoco` (5.3.0) — micro-ORM
- `SplatDev.Reflection.Helpers` — attribute and type reflection utilities
- `Microsoft.Extensions.DependencyInjection.Abstractions` — DI registration support

---

Copyright SplatDev
