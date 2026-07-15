# SplatDev.Umbraco.Plugins.PdfCurator

Umbraco plugin for managing a PDF-based digital book library. Upload PDFs through the Umbraco backoffice, import them into a SQLite database, and browse/delete book entries. Includes a Lit-based dashboard with English and Portuguese (Brazil) localization.

## Install

```bash
dotnet add package SplatDev.Umbraco.Plugins.PdfCurator
```

Targets `net8.0` (Umbraco 13) and `net10.0` (Umbraco 17). Published to nuget.org.

## What's implemented

### Backoffice Dashboard

Three Lit-based dashboard tabs registered in `umbraco-package.json`:

| Tab | Functionality |
|-----|---------------|
| **Library** | Table of all books (ID, Title, Author) with pagination and per-row delete |
| **Upload** | File input accepting `.pdf` files with upload progress feedback |

The dashboard appears under **Settings → Pdf Curator** and is available in English and Portuguese (Brazil).

### API Controllers

Three Umbraco API controllers:

#### `ImportApiController`

| Method | Route | Description |
|--------|-------|-------------|
| `GET GetAllReadyAsync(bool done)` | Lists files in the upload or done folder |
| `POST ImportAll()` | Processes uploaded PDF files into the database |

#### `ManagerApiController`

| Method | Route | Description |
|--------|-------|-------------|
| `GET GetAllAsync()` | Returns all books |
| `POST GetFilteredBooksAsync(PagedFilter)` | Paginated, filtered book list |
| `GET GetBookAsync(int id)` | Single book by ID |
| `POST UpdateBookAsync(Book)` | Update a book |
| `DELETE DeleteBookAsync(int id)` | Delete a book |

#### `UploadApiController`

| Method | Route | Description |
|--------|-------|-------------|
| `POST UploadFileAsync(IFormFile)` | Save a single PDF to the upload folder |
| `POST UploadFiles()` | Save multiple PDFs from form upload |

### Configuration

Add to `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "CuratorDb": "Data Source=curator.db"
  },
  "CuratorSettings": {
    "Origin": "wwwroot\\uploads\\pdfs",
    "Destination": "wwwroot\\ebooks",
    "DeleteEmptyFolders": false
  }
}
```

| Setting | Default | Description |
|---------|---------|-------------|
| `CuratorDb` connection string | (required) | SQLite database for book metadata |
| `Origin` | `wwwroot\uploads\pdfs` | Upload folder for incoming PDFs |
| `Destination` | `wwwroot\ebooks` | Folder for processed files |
| `DeleteEmptyFolders` | `false` | Clean up empty folders after import |

### Models

| Type | Description |
|------|-------------|
| `FileImportAvailable` | File metadata DTO (`Name`, `Size` in KB) |
| `Book` | EF Core entity — book metadata stored in SQLite |
| `BookViewModel` | View model for paginated results |
| `PagedFilter` | Filter/sort/pagination request |

## DI Registration

Automatic via `CuratorComposer : IComposer`. No manual registration needed.

Registered services:

| Service | Lifetime |
|---------|----------|
| `CuratorDbContext` | Scoped (DbContext pool, SQLite) |
| `IBookRepository` → `BookRepository` | Scoped |
| `FileManagerService` | Scoped |

## Dependencies

| Package | Purpose |
|---------|---------|
| `Umbraco.Cms.Core` | Composer, controllers, dependency injection |
| `Umbraco.Cms.Infrastructure` | Infrastructure services |
| `Umbraco.Cms.Web.Common` | Web hosting and API controllers |
| `Umbraco.Cms.Api.Management` (net10.0 only) | Management API for Umbraco 17 |
| `Microsoft.EntityFrameworkCore.Sqlite` | SQLite database for book storage |
| `SplatDev.DigitalBookCurator.Core` | EF Core context, repositories, models, file processing |

---

**SplatDev.Umbraco.Plugins.PdfCurator** — part of the [SplatDev.Umbraco.Plugins](https://github.com/SplatDev-Ltda/SplatDev.Umbraco.Plugins) suite. Licensed under MIT. © SplatDev Ltda.
