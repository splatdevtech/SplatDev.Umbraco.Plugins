# PDF Curator

Umbraco backoffice PDF library manager — upload, organize, and serve PDF digital books and documents. Supports Umbraco 13 (net8.0) and Umbraco 17 (net10.0).

[![NuGet](https://img.shields.io/nuget/v/SplatDev.Umbraco.Plugins.PdfCurator.svg)](https://www.nuget.org/packages/SplatDev.Umbraco.Plugins.PdfCurator)

## Features

- Upload PDFs directly from the Umbraco backoffice Settings section
- Manage a PDF library with SQLite-backed metadata (net8) or SQL storage (net10)
- Import, browse, and delete stored documents via a dedicated dashboard
- Localized UI (English + pt-BR)

## Compatibility

| Umbraco | .NET | Package Version |
|---------|------|-----------------|
| 13.x    | 8.0  | 2.0.0           |
| 17.x    | 10.0 | 2.0.0           |

## Installation

```sh
dotnet add package SplatDev.Umbraco.Plugins.PdfCurator
```

## Quick Start

Register in `Program.cs`:

```csharp
builder.CreateUmbracoBuilder()
    .AddBackOffice()
    .AddWebsite()
    .AddPdfCurator()   // <-- add this
    .Build();
```

The dashboard appears at **Settings → PDF Curator** (`/umbraco/settings/pdf-curator`).

## API Endpoints

| Method | Route | Purpose |
|--------|-------|---------|
| `GET`  | `/umbraco/api/pdfcurator/manager/list` | List all stored PDFs |
| `POST` | `/umbraco/api/pdfcurator/upload` | Upload a new PDF |
| `POST` | `/umbraco/api/pdfcurator/import` | Import available PDFs |
| `DELETE` | `/umbraco/api/pdfcurator/manager/delete` | Remove a PDF by ID |

## License

MIT © [SplatDev](https://github.com/SplatDev-Ltda)
