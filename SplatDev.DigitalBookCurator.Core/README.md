# SplatDev.DigitalBookCurator.Core

Standalone domain library for PDF/eBook management with EF Core + SQLite persistence. Provides `Book` entity modeling, streaming file import, soft-delete repository, and paged query support. No Umbraco dependency — usable in any .NET host.

## Install

```sh
dotnet add package SplatDev.DigitalBookCurator.Core
```

## What's implemented

### Domain model
- `Book` — entity with full book metadata (Title, Author, FileName, FilePath, FileSize, FileType, Isbn, Description, CoverImagePath, PageCount, CreatedAt, ModifiedAt, IsDeleted). Soft-delete via EF Core global query filter.

### Repository
- `IBookRepository` / `BookRepository` — async CRUD:
  - `GetAllBooksAsync()` — all non-deleted books.
  - `GetFilteredBooksAsync(PagedFilter)` — search by title/author, sort by any column, paginated.
  - `GetBookByIdAsync(int)` — single book lookup.
  - `UpdateBookAsync(Book)` — update metadata.
  - `DeleteBookAsync(int)` — soft-delete.

### File import
- `FileManagerService.ProcessUploadedAsync(sourcePath, destPath, ct)` — streams `*.pdf` files from a source directory, moves them to a destination, creates `Book` records. Yields `BookImportResult` per file.

### Query DTOs
- `PagedFilter` — `SearchTerm`, `SortBy`, `SortDescending`, `Page`, `PageSize`.
- `PagedResults<T>` — generic wrapper with `Results` + `Pagination` metadata.
- `BookViewModel` — view-focused projection of book data.

### Ef Core context
- `CuratorDbContext : DbContext` — exposes `DbSet<Book>`. Configured for SQLite. Applies `.HasQueryFilter(b => !b.IsDeleted)` at the model level.

## Configuration

Bind `CuratorSettings` from `appsettings.json`:

```json
{
  "SplatDev": {
    "Curator": {
      "Origin": "/path/to/incoming-pdfs",
      "Destination": "/path/to/ebook-library",
      "DeleteEmptyFolders": true
    }
  },
  "ConnectionStrings": {
    "Curator": "Data Source=curator.db"
  }
}
```

```csharp
services.Configure<CuratorSettings>(
    configuration.GetSection("SplatDev:Curator"));
```

## DI registration

No built-in DI extensions. Register manually:

```csharp
// DbContext
builder.Services.AddDbContext<CuratorDbContext>(options =>
    options.UseSqlite(configuration.GetConnectionString("Curator")));

// Repository
builder.Services.AddScoped<IBookRepository, BookRepository>();
builder.Services.AddScoped<FileManagerService>();
```

## Usage

### Import books from a directory

```csharp
var fileManager = services.GetRequiredService<FileManagerService>();
var settings = services.GetRequiredService<IOptions<CuratorSettings>>().Value;

await foreach (var result in fileManager.ProcessUploadedAsync(
    settings.Origin, settings.Destination, CancellationToken.None))
{
    Console.WriteLine($"{result.FileName}: {(result.Success ? "OK" : "FAIL")}");
}
```

### Search books with pagination

```csharp
var repo = services.GetRequiredService<IBookRepository>();
var filter = new PagedFilter
{
    SearchTerm = "dostoevsky",
    SortBy = "Title",
    Page = 0,
    PageSize = 20
};
var results = await repo.GetFilteredBooksAsync(filter);
```

---

**SplatDev.DigitalBookCurator.Core** — part of the [SplatDev.Umbraco.Plugins](https://github.com/SplatDev-Ltda/SplatDev.Umbraco.Plugins) suite. Licensed under MIT. © SplatDev Ltda.
