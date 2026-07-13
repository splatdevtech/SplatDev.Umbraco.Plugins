# SplatDev.Umbraco.Plugins.PdfCurator

PDF curation and secure download plugin for Umbraco.
Targets Umbraco 13 (net8.0) and Umbraco 17 (net10.0).

## Installation

```bash
dotnet add package SplatDev.Umbraco.Plugins.PdfCurator
```

## Features

- Secure PDF upload and storage
- Download tracking with audit logging
- Role-based access control for PDF downloads
- Content approval workflow integration
- MIME type validation and file size limits

## Usage

### Upload

```csharp
var pdf = await PdfCurator.UploadAsync(file, "Annual Report");
```

### Serve securely

```csharp
[Authorize(Roles = "Members")]
public async Task<IActionResult> Download(int id)
{
    var pdf = await PdfCurator.GetAsync(id);
    return File(pdf.Stream, "application/pdf", pdf.FileName);
}
```

> **Security**: Always use `IFormFile` validation (MIME type, extension, size)
> and store files outside `wwwroot`. Authenticate download endpoints.

## License

MIT
