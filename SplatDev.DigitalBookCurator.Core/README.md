# SplatDev.DigitalBookCurator.Core

Standalone domain library for e-book/PDF management with EF Core + SQLite persistence. Provides the data layer consumed by the [SplatDev.Umbraco.Plugins.PdfCurator](../SplatDev.Umbraco.Plugins.PdfCurator) Umbraco plugin.

## Targets

- **net8.0** (Umbraco 13)
- **net10.0** (Umbraco 17)

## Installation

```bash
dotnet add package SplatDev.DigitalBookCurator.Core
```

## What's Implemented

### Data Layer

| Type | Description |
|------|-------------|
| `CuratorDbContext` | EF Core `DbContext` with soft-delete query filter on `Book` |
| `Book` | Domain model (Title, Author, FileName, FilePath, FileSize, FileType, Isbn, Description, CoverImagePath, PageCount, CreatedAt, ModifiedAt, IsDeleted) |
| `IBookRepository` / `BookRepository` | Async CRUD, paged filter/sort, soft delete |

### Services

| Type | Description |
|------|-------------|
| `FileManagerService` | Scans source directories for PDFs, moves files to destination, and registers books via streaming `IAsyncEnumerable<BookImportResult>` |

### View Models

| Type | Description |
|------|-------------|
| `BookViewModel` | Lightweight projection for list views (Id, Title, Author, FileName, FileType, FileSize, PageCount, CreatedAt) |
| `PagedFilter` | Query shape: `SearchTerm`, `SortBy`, `SortDescending`, `Page`, `PageSize` |
| `PagedResults<T>` | Generic paginated response with `Items`, `TotalCount`, `TotalPages` |

### Configuration

| Type | Description |
|------|-------------|
| `CuratorSettings` | Origin/Destination paths and `DeleteEmptyFolders` flag |
| `BookImportResult` | Per-file import outcome (Success, FileName, ErrorMessage, Book) |

## Configuration

`CuratorSettings` provides three properties:

```csharp
var settings = new CuratorSettings
{
    Origin = "/path/to/source",
    Destination = "/path/to/storage",
    DeleteEmptyFolders = true
};
```

## Usage

Register the DbContext and repository:

```csharp
services.AddDbContext<CuratorDbContext>(options =>
    options.UseSqlite("Data Source=books.db"));

services.AddScoped<IBookRepository, BookRepository>();
services.AddScoped<FileManagerService>();
```

Paged query example:

```csharp
var filter = new PagedFilter
{
    Page = 1,
    PageSize = 20,
    SearchTerm = "design patterns",
    SortBy = "title",
    SortDescending = false
};

var result = await repository.GetFilteredBooksAsync(filter);
// result.Items, result.TotalCount, result.TotalPages
```

## EF Core Migrations

**No migrations have been generated yet.** Run the following to create the initial schema:

```bash
dotnet ef migrations add InitialCreate --context CuratorDbContext --startup-project <your-startup-project>
dotnet ef database update --context CuratorDbContext
```

## Companion Plugin

The sole consumer is [SplatDev.Umbraco.Plugins.PdfCurator](../SplatDev.Umbraco.Plugins.PdfCurator) — an Umbraco backoffice plugin that provides a dashboard for uploading, managing, and searching PDFs. It references this library as a project dependency and wires up the `CuratorDbContext`, `IBookRepository`, and `FileManagerService` via an Umbraco composer.

## Caveats

- **SQLite only.** No multi-provider abstraction exists. The `CuratorDbContext` is tied to `Microsoft.EntityFrameworkCore.Sqlite`.
- **LIKE-scoped filtering.** `GetFilteredBooksAsync` uses `Contains()` (translates to `LIKE %term%`). Acceptable for small-to-medium catalogs. No FTS5 full-text search is configured.
- **No built-in cover or ISBN extraction.** `CoverImagePath` and `Isbn` are nullable metadata fields that must be populated externally (via the companion plugin or manual update).
- **No migrations.** The schema must be generated manually with `dotnet ef migrations`.

---

[SplatDev Ltda](https://github.com/SplatDev-Ltda)
