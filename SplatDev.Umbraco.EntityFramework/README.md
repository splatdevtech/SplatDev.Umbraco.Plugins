# SplatDev.Umbraco.EntityFramework

Generic Entity Framework Core repository abstraction for Umbraco 13 and 17. Provides `IRepository<TEntity>` and a complete `DbContextRepository<TEntity>` implementation that wraps a standard EF Core `DbContext`, handling CRUD, raw SQL queries, paginated results, and record counts with consistent error logging. Includes second-level caching interceptors and Dynamic LINQ support out of the box.

## Install

```bash
dotnet add package SplatDev.Umbraco.EntityFramework
```

Multi-targets `net8.0` (Umbraco 13, EF Core 8) and `net10.0` (Umbraco 17, EF Core 10). Published to nuget.org.

## What's implemented

### `IRepository<TEntity>` where TEntity : class

The generic repository contract with 10 async methods:

| Method | Returns | Description |
|--------|---------|-------------|
| `GetByIdAsync(id)` | `Task<TEntity?>` | Find by primary key (tracked entity) |
| `GetAllAsync()` | `Task<IList<TEntity>?>` | All entities, no-tracking |
| `GetPagedResultsAsync(page, size)` | `Task<PagedResults<TEntity>>` | Paginated result with `Pagination` metadata |
| `FetchAsync(sql)` | `Task<IList<TEntity>>` | Raw SQL query, no-tracking |
| `CreateAsync(entity)` | `Task<TEntity>` | Insert and save |
| `UpdateAsync(entity)` | `Task<TEntity>` | Update and save |
| `DeleteAsync(id)` | `Task` | Delete by primary key |
| `CountRecordsAsync()` | `Task<int?>` | Total row count |

### `DbContextRepository<TEntity>` : IRepository<TEntity>

The concrete implementation using a primary constructor:

```csharp
public class DbContextRepository<TEntity>(DbContext context, ILogger<DbContextRepository<TEntity>> logger)
    : IRepository<TEntity>
```

All read methods use `AsNoTracking()` except `GetByIdAsync` which uses `FindAsync(id)` (tracked entity, good for update scenarios). Write methods catch exceptions and log via `ILogger.LogError`.

Pagination uses the `SplatDev.Umbraco.Pagination` package's `PagedResults<T>` and `Pagination` models, with `Skip/Take` and a `CountAsync` for total records.

### Second-level caching

The package includes `EFCoreSecondLevelCacheInterceptor` and its `MemoryCache` provider, ready to be registered as EF Core interceptors. When registered, queries are automatically cached at the EF level, reducing database round-trips for repeated reads.

## Usage

### 1. Define your entity and DbContext

```csharp
public class Order
{
    public int Id { get; set; }
    public string CustomerName { get; set; }
    public decimal Total { get; set; }
}

public class AppDbContext : DbContext
{
    public DbSet<Order> Orders { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options.UseSqlServer(connectionString);
    }
}
```

### 2. Register DI

```csharp
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddScoped(typeof(IRepository<>), typeof(DbContextRepository<>));
```

### 3. Use it

```csharp
var repo = serviceProvider.GetRequiredService<IRepository<Order>>();

var orders = await repo.GetAllAsync();

var page = await repo.GetPagedResultsAsync(pageNumber: 1, pageSize: 20);
// page.Results, page.Pagination.TotalPages, page.Pagination.TotalResults

var custom = await repo.FetchAsync("SELECT * FROM Orders WHERE Total > 100");
```

## DI Registration

No automatic registration. Register manually as shown above. Dependencies to satisfy:

| Service | Provided by |
|---------|-------------|
| `DbContext` | Consumer's own `AddDbContext<T>` |
| `ILogger<T>` | .NET logging infrastructure (automatic) |

## Dependencies

| Package | Purpose |
|---------|---------|
| `Microsoft.EntityFrameworkCore` (8.0.20 / 10.0.7) | Core EF ORM |
| `Microsoft.EntityFrameworkCore.SqlServer` | SQL Server provider |
| `Microsoft.EntityFrameworkCore.DynamicLinq` | Dynamic LINQ queries |
| `EFCoreSecondLevelCacheInterceptor` (5.3.2) | Second-level query caching |
| `EFCoreSecondLevelCacheInterceptor.MemoryCache` (5.3.2) | In-memory cache provider |
| `SplatDev.Umbraco.Pagination` | Paged result models and pagination helpers |
| `Umbraco.Cms.Core` | Umbraco types and DI infrastructure |
| `Umbraco.Cms.Infrastructure` | Infrastructure services |

---

**SplatDev.Umbraco.EntityFramework** — part of the [SplatDev.Umbraco.Plugins](https://github.com/SplatDev-Ltda/SplatDev.Umbraco.Plugins) suite. Licensed under MIT. © SplatDev Ltda.
