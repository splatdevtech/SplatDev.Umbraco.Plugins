# SplatDev.Database

Generic NPoco-based data access layer for .NET applications. Provides a typed CRUD abstraction (`IDbService<T>` / `DbService<T>`), schema-discovery helpers, custom attributes for table/column metadata, CLR-to-SQL type mapping, and a seed-data bulk-insertion facility. Extend `DbService<T>` for each of your tables and get create, read, update, delete, and raw query support against NPoco's `IDatabase`.

## Install

```bash
dotnet add package SplatDev.Database
```

Multi-targets `net8.0` and `net10.0`. Published to nuget.org.

## What's implemented

### `ITable`

Your entity classes implement this contract:

```csharp
public interface ITable
{
    string TableName { get; }
    int Id { get; set; }
}
```

### `IDbService<T>` where T : class, new()

| Method | Returns | Description |
|--------|---------|-------------|
| `GetById(id)` | `Task<T>` | Single entity by integer primary key |
| `Insert(data)` | `Task<bool>` | Insert a new row |
| `Update(data, primaryKeyValue)` | `Task<bool>` | Update an existing row |
| `Delete(id)` | `Task<bool>` | Delete by primary key |
| `Exists(id)` | `Task<bool>` | Check existence by primary key |

### `DbService<T>` : IDbService<T>

Abstract base class. Inject NPoco's `IDatabase` and an optional table name:

```csharp
public abstract class DbService<T> : IDbService<T> where T : class, new()
{
    protected DbService(IDatabase database, string tableName = "") { ... }
    protected IDatabase Database { get; }
    protected string _tableName;
}
```

Virtual methods you can override:

| Method | Description |
|--------|-------------|
| `GetAll()` | `SELECT * FROM {table}` via `Db.Query<T>()` |
| `GetAllIds(ids)` | `SELECT * FROM {table} WHERE Id IN (...)` |
| `GetById(string id, params string[] navProps)` | String-key lookup with NPoco navigation properties |
| `Query(sql)` | Raw SQL returning `IEnumerable<T>` |
| `DeleteAll()` | `DELETE FROM {table}` |

### `PostDbCreation`

Abstract hook to run custom logic after a table is created:

```csharp
public abstract class PostDbCreation
{
    public virtual void Execute() { }
}
```

Extend to seed data, create indexes, or insert default rows.

### Schema helpers

#### `DatabaseHelper` (static)

| Method | Description |
|--------|-------------|
| `ColumnExists(db, table, column)` | Checks `INFORMATION_SCHEMA.COLUMNS` |
| `TableExistsQuery(db, table)` | Checks `INFORMATION_SCHEMA.TABLES` |
| `GetTableColumns(type)` | Reflects property names, excluding `[Ignore]` |
| `GetTableName(type)` | Resolves via `TABLENAME` const or `TableName` property |
| `GetTables(assembly)` | Discovers all `ITable` types in an assembly |
| `InsertData(db, list)` | Bulk-inserts seed data with optional skip-if-exists |

#### `AttributeHelper` (static)

| Method | Description |
|--------|-------------|
| `OrderByCreateOrderAttribute(types)` | Sorts types by `[CreateOrder(order)]` |
| `NvarcharMaxProperties<Type>(type)` | Finds properties with `[NvarcharMax]` |
| `PrettyDisplayName(property)` | Returns `[Display(Name=...)]` or raw name |

### Custom attributes

| Attribute | Applies to | Purpose |
|-----------|-----------|---------|
| `[CreateOrder(int order)]` | Class | Controls table creation ordering |
| `[NvarcharMax]` | Property | Flags column for `NVARCHAR(MAX)` |
| `[AlteredColumn]` | Property | Marks column as altered (migrations) |

### Type mapping

`DatabaseTypes` struct maps CLR types to SQL Server types:

| CLR Type | SQL Type |
|----------|----------|
| `byte` | `TINYINT` |
| `short` (`int16`) | `SMALLINT` |
| `int` (`int32`) | `INT` |
| `long` (`int64`) | `BIGINT` |
| `string` | `NVARCHAR(255)` |
| `bool` | `BIT` |
| `decimal` | `DECIMAL` |
| `float` / `double` | `FLOAT` |
| `DateTime` | `DATETIME` |

### `DatabaseMigration`

A migration handler class for `ALTER TABLE` generation. Currently a stub — the migration logic is commented out and pending implementation.

## Usage

```csharp
public class Product : ITable
{
    public const string TABLENAME = "Products";
    public string TableName => TABLENAME;
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
}

public class ProductService : DbService<Product>
{
    public ProductService(IDatabase database) : base(database, Product.TABLENAME) { }
}
```

## DI Registration

No built-in DI extensions. Register manually:

```csharp
builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<IDbService<Product>>(sp => sp.GetRequiredService<ProductService>());
```

Wire up `IDatabase` via NPoco's own `DatabaseFactory` or your DI container.

## Dependencies

| Package | Purpose |
|---------|---------|
| `NPoco` (5.3.0) | Micro-ORM (database, query, mapping) |
| `SplatDev.Reflection.Helpers` | Attribute and reflection extension methods |
| `Microsoft.Extensions.DependencyInjection.Abstractions` (8.0.0) | DI interfaces |

---

**SplatDev.Database** — part of the [SplatDev.Umbraco.Plugins](https://github.com/SplatDev-Ltda/SplatDev.Umbraco.Plugins) suite. Licensed under MIT. © SplatDev Ltda.
