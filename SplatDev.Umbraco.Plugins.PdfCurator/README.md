# SplatDev.Umbraco.Plugins.PdfCurator

PDF curator and manager plugin for Umbraco CMS. Import, upload, and manage PDF documents directly from the backoffice.

[![NuGet](https://img.shields.io/nuget/v/SplatDev.Umbraco.Plugins.PdfCurator)](https://www.nuget.org/packages/SplatDev.Umbraco.Plugins.PdfCurator)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

## Compatibility

| Umbraco Version | .NET Target | Package Version |
|---|---|---|
| Umbraco 13.x | net8.0 | 2.0.x |
| Umbraco 17.x | net10.0 | 2.0.x |

## Features

- Upload and manage PDF files from the Umbraco backoffice
- Import PDFs from external sources
- Browse and search curated PDF collections
- SQLite-backed metadata storage
- Built on top of the SplatDev.DigitalBookCurator.Core engine

## Installation

```bash
dotnet add package SplatDev.Umbraco.Plugins.PdfCurator
```

Or via the Package Manager Console:

```powershell
Install-Package SplatDev.Umbraco.Plugins.PdfCurator
```

## API Endpoints

| Method | Route | Description |
|--------|-------|-------------|
| POST | `/umbraco/api/import/*` | Import PDFs from external sources |
| GET/POST | `/umbraco/api/manager/*` | Manage curated PDF collection |
| POST | `/umbraco/api/upload/*` | Upload PDF files |

## Dependencies

- [SplatDev.DigitalBookCurator.Core](https://github.com/SplatDev-Ltda) — PDF processing engine
- Microsoft.EntityFrameworkCore.Sqlite

## License

MIT © [SplatDev](https://github.com/SplatDev-Ltda)
