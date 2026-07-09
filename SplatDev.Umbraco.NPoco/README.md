# SplatDev.Umbraco.NPoco

NPoco database layer extensions for Umbraco 13 (net8.0) and Umbraco 17 (net10.0). Provides an `IScopeProvider`-backed repository pattern with events, audit tracking, and paged queries.

## Install

```bash
dotnet add package SplatDev.Umbraco.NPoco
```

Targets both Umbraco 13 (`net8.0`) and Umbraco 17 (`net10.0`) via conditional package references.

## What's implemented

### `IBaseEntity` — entity marker

```csharp
public interface IBaseEntity
{
    int Id { get; set; }
}
```

All repository entities must implement `IBaseEntity` — provides the `Id` contract required by `BaseEntityRepository<T>`.

### `IRepository<T>` — sync + async CRUD contract

Synchronous operations (existing):
- `Count()` / `Exists(id)`
- `Get(int?)` / `Get(string column, object? value)`
- `GetAll()` / `GetMany(params int[])` / `GetMany(int)`
- `Fetch(string query)`
- `Insert(T entity)` / `InsertBulk(List<T>)`
- `Update(T entity)` / `UpdateBulk(List<T>)`
- `Delete(int)` / `Delete(T entity)` / `DeleteBulk(...)`
- `GetPagedResultsByQuery(...)` — paged queries with ordering and custom SQL
- `UserId` property for audit tracking (defaults to Umbraco super-user)

Async overloads (added via SPL-2499):
- `GetAsync(int?)` / `GetAllAsync()`
- `InsertAsync(T)` / `UpdateAsync(T)` / `DeleteAsync(int)`
- `ExistsAsync(int)` / `CountAsync()`

### `BaseEntityRepository<T>` — Umbraco-scoped NPoco base

```csharp
public abstract class BaseEntityRepository<T>(IScopeProvider scopeProvider)
    : IDisposable, IRepository<T>
    where T : class, IBaseEntity
```

Uses `IScopeProvider` with `autoComplete: true` for fire-and-forget scope management. Raises `ActionCompleted` events with audit context on every mutation.

### `ActionCompleted` event + audit payloads

- `OnActionCompleted` delegate — `void(object sender, ActionCompletedEvent e)`
- `ActionCompletedEvent` — includes `AuditType` (Save/Delete), entity reference, `Id`, `UserId`, message, and logging flag
- `UniqueActionCompletedEvent<T>` — generic variant for type-specific listeners

### Query support

- `GetBase(Action<Sql>)` / `GetBaseQuery(bool isCount)` — build NPoco SQL from `IScopeProvider.SqlContext`
- `GetPagedResultsByQuery(...)` — paged with `Ordering`, optional `sqlCustomization`, filter via `IQuery<T>.GetWhereClauses()`
- `ApplyOrdering(ref Sql, Ordering)` — ascending/descending helper

### Known limitations

- `Get(string column, object? value)` accepts a string column name — column-whitelist validation required at the call site (see SPL-2492)
- `Fetch(string query)` accepts raw SQL — prefer parameterized calls or use the repository's typed query methods
- Umbraco 17 target has a pre-existing build issue with `Constants.Security.SuperUserKey` (under investigation)

## Usage

### Deriving a repository

```csharp
public class ProductRepository : BaseEntityRepository<Product>
{
    public ProductRepository(IScopeProvider scopeProvider) : base(scopeProvider) { }
}

// Register in DI
services.AddScoped<ProductRepository>();
```

### Subscribing to audit events

```csharp
var repo = new ProductRepository(scopeProvider);
repo.ActionCompleted += (sender, e) =>
{
    if (e.AuditType == AuditType.Delete)
        _logger.LogInformation("Deleted {Id} by {UserId}", e.Id, e.UserId);
};
```

### Paged queries

```csharp
var products = repo.GetPagedResultsByQuery(
    query, pageIndex: 0, pageSize: 20,
    out var total, filter: null, ordering: Ordering.By("name"));
```

### Async usage

```csharp
var product = await repo.GetAsync(42);
await repo.InsertAsync(new Product { Name = "Widget" });
await repo.UpdateAsync(existing);
await repo.DeleteAsync(42);
var exists = await repo.ExistsAsync(42);
```

## Dependencies

- `Umbraco.Cms.Core` / `Umbraco.Cms.Infrastructure` (13.12.0 for net8.0, 17.3.4 for net10.0)
- `NPoco` (transitively via Umbraco)
- `SplatDev.Umbraco.NPoco.Notifications` — event payload types

---

Copyright SplatDev
