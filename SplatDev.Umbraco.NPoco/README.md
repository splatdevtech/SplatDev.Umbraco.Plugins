# SplatDev.Umbraco.NPoco

Generic repository abstraction built on NPoco for Umbraco 13 and 17. Provides an `IRepository<T>` contract, a fully-implemented `BaseEntityRepository<T>` base class that wraps Umbraco's `IScopeProvider` for database scoping, and audit eventing on every data mutation. Extend the base class for your custom tables and get CRUD, paged queries, bulk operations, and audit notifications out of the box.

## Install

```bash
dotnet add package SplatDev.Umbraco.NPoco
```

Multi-targets `net8.0` (Umbraco 13) and `net10.0` (Umbraco 17). NPoco is included transitively via `Umbraco.Cms.Infrastructure`.

## What's implemented

### `IBaseEntity`

Minimal contract your domain entities must implement:

```csharp
public interface IBaseEntity
{
    int Id { get; set; }
}
```

### `IRepository<T>` where T : IBaseEntity

Full CRUD contract with 17 members:

| Category | Methods |
|----------|---------|
| **Read** | `Get(id)`, `Get(column, value)`, `GetAll()`, `GetMany(ids)`, `Exists(id)`, `Count()`, `Fetch(sql)` |
| **Write** | `Insert(entity)`, `InsertBulk(list)`, `Update(entity)`, `UpdateBulk(list)` |
| **Delete** | `Delete(id)`, `Delete(entity)`, `DeleteBulk(ids)`, `DeleteBulk(entities)` |
| **Paged** | `GetPagedResultsByQuery(query, ordering)` with optional `sqlCustomization` callback |
| **Events** | `ActionCompleted` event with `OnActionCompleted` delegate |

### `BaseEntityRepository<T>` : IRepository<T>

The abstract base implementation. Takes `IScopeProvider` via primary constructor.

```csharp
public abstract class BaseEntityRepository<T>(IScopeProvider scopeProvider)
    : IDisposable, IRepository<T>
    where T : class, IBaseEntity
```

Every CRUD operation creates a scoped database session via `IScopeProvider.CreateScope(autoComplete: true)`. Each `Insert`, `Update`, and `Delete` fires `ActionCompleted` with the entity, its ID, the `AuditType` (Save/Delete), the acting `UserId`, and a message string.

### Audit events

Subscribe to `ActionCompleted` to log or react to data changes:

```csharp
repo.ActionCompleted += (sender, e) =>
{
    logger.LogInformation("User {UserId} {Action} entity {Id}",
        e.UserId, e.AuditType, e.Id);
};
```

Event payload (`ActionCompletedEvent`):

| Property | Type | Description |
|----------|------|-------------|
| `Id` | `int` | Entity primary key |
| `Entity` | `IBaseEntity?` | Full entity (may be null on Delete-by-id) |
| `AuditType` | `AuditType` | `Save` or `Delete` |
| `UserId` | `Guid` | Defaults to SuperUser |
| `Message` | `string?` | Operation description |
| `Log` | `bool` | Set to `true` on deletes |

### `UserId` tracking

Set `repo.UserId` before operations to attribute them to a specific backoffice user. Defaults to `Constants.Security.SuperUserKey` if not set.

## Usage

### 1. Define your entity

```csharp
using SplatDev.Umbraco.NPoco.Repositories;

public class Product : IBaseEntity
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
}
```

### 2. Extend the base repository

```csharp
public class ProductRepository(IScopeProvider scopeProvider)
    : BaseEntityRepository<Product>(scopeProvider)
{
    // Add custom queries here using GetBase(), GetBaseQuery(), etc.
}
```

### 3. Register in DI

```csharp
builder.Services.AddScoped<IRepository<Product>, ProductRepository>();
```

### 4. Use it

```csharp
var repo = serviceProvider.GetRequiredService<IRepository<Product>>();
repo.UserId = currentUserKey;

var product = new Product { Name = "Widget", Price = 9.99m };
product = repo.Insert(product);       // Id is populated, event fires
product.Price = 12.99m;
repo.Update(product);                 // event fires
repo.Delete(product.Id);              // event fires
```

## DI Registration

No built-in registration extensions. Register your concrete repository types yourself. The only required dependency is `IScopeProvider`, which Umbraco's DI already provides.

## Dependencies

| Package | Purpose |
|---------|---------|
| `Umbraco.Cms.Core` | `IScopeProvider`, `AuditType`, `Constants` |
| `Umbraco.Cms.Infrastructure` | NPoco is included transitively |
| `Umbraco.Cms.Web.BackOffice` (net8.0) / `Umbraco.Cms.Api.Management` (net10.0) | Backoffice context |

---

**SplatDev.Umbraco.NPoco** — part of the [SplatDev.Umbraco.Plugins](https://github.com/SplatDev-Ltda/SplatDev.Umbraco.Plugins) suite. Licensed under MIT. © SplatDev Ltda.
