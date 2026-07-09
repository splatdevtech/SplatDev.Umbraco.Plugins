# SplatDev.Umbraco.EntityFramework

EF Core repository base for use inside Umbraco 13/17 applications. Provides async CRUD, paged results, and optional second-level cache integration.

## Install

```bash
dotnet add package SplatDev.Umbraco.EntityFramework
```

Depends on EF Core 8 (net8.0) or EF Core 10 (net10.0), and `SplatDev.Umbraco.Pagination` for paging models.

## What's implemented

### `IRepository<TEntity>` — async CRUD contract

```csharp
public interface IRepository<TEntity> : IDisposable where TEntity : class
{
    Task<int?> CountRecordsAsync();
    Task<TEntity> CreateAsync(TEntity entity);
    Task DeleteAsync(int id);
    Task<IList<TEntity>> FetchAsync(string query);
    Task<IList<TEntity>?> GetAllAsync();
    Task<TEntity?> GetByIdAsync(int id);
    Task<PagedResults<TEntity>> GetPagedResultsAsync(int pageNumber, int pageSize);
    Task<TEntity> UpdateAsync(TEntity entity);
}
```

### `DbContextRepository<TEntity>` — primary constructor implementation

```csharp
public class DbContextRepository<TEntity>(
    DbContext context,
    ILogger<DbContextRepository<TEntity>> logger) : IRepository<TEntity>
```

Key behaviours:
- `GetAllAsync` / `GetPagedResultsAsync` — use `AsNoTracking()` for read-only queries
- `CreateAsync` / `UpdateAsync` — call `SaveChangesAsync()` and return the entity
- `DeleteAsync(int id)` — fetches by ID then removes
- `FetchAsync(string query)` — uses `FromSql(...)` for parameterized raw SQL (post-SPL-2493)
- `GetPagedResultsAsync` — includes total count, page count calculation
- `CountRecordsAsync` — `CountAsync()` projection

### Paging model

Returns `PagedResults<TEntity>` from `SplatDev.Umbraco.Pagination`:

```csharp
public class PagedResults<T>
{
    public IList<T> Results { get; set; }
    public Pagination Pagination { get; set; }
}

public class Pagination
{
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalResults { get; set; }
    public int TotalPages { get; set; }
}
```

## Usage

### Deriving a repository

```csharp
public class ProductRepository : DbContextRepository<Product>
{
    public ProductRepository(MyDbContext context, ILogger<ProductRepository> logger)
        : base(context, logger) { }
}
```

### Registering in DI

```csharp
builder.Services.AddDbContext<MyDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddScoped<ProductRepository>();
```

### Paged fetch

```csharp
var paged = await repo.GetPagedResultsAsync(pageNumber: 1, pageSize: 20);
Console.WriteLine($"Page {paged.Pagination.Page} of {paged.Pagination.TotalPages}");
```

### Raw query

```csharp
var results = await repo.FetchAsync(
    "SELECT * FROM Products WHERE Category = {0}", "widgets");
```

## Known caveats

- `FetchAsync(string query)` — ensure the query string is parameterized (post-SPL-2493). Prefer typed queries via the DbSet when possible
- `CreateAsync` catches all exceptions and logs — consider rethrowing critical errors at the call site
- `GetByIdAsync(string)` vs `GetByIdAsync(int)` — only `int` overload available; string IDs require a custom implementation

---

Copyright SplatDev
