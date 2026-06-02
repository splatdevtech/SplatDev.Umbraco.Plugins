# SplatDev.Content

Content helper utilities for .NET applications including CSV parsing, Excel import, HTML manipulation, encryption, QR code generation, and country/timezone data.

[![NuGet](https://img.shields.io/nuget/v/SplatDev.Content)](https://www.nuget.org/packages/SplatDev.Content)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

## Compatibility

| .NET Target | Package Version |
|---|---|
| net8.0 | 1.0.x |
| net10.0 | 1.0.x |

## Features

- **CSV Helpers** — Parse and generate CSV content
- **Excel Import** — Import data from Excel files via EPPlus
- **HTML Helpers** — HTML sanitization and manipulation utilities
- **Encryption** — Encrypt/decrypt helper methods
- **QR Codes** — Generate QR code images
- **Country Data** — ISO 3166 country list with attributes and timezone data
- **String Helpers** — Common string manipulation extensions
- **Validation** — Structured validation error collection

## Installation

```bash
dotnet add package SplatDev.Content
```

## Key Classes

| Class | Description |
|-------|-------------|
| `CsvHelpers` | CSV parsing and generation |
| `ExcelImporterHelpers` | Excel file import via EPPlus |
| `HtmlHelpers` | HTML content utilities |
| `EncrypDecryptHelpers` | Encryption and decryption |
| `QRHelpers` | QR code generation |
| `CountryHelpers` | Country lookup and filtering |
| `StringHelpers` | String manipulation extensions |

## Dependencies

- EPPlus 7.6.1
- Microsoft.AspNetCore.Http 2.2.0

## License

MIT © [SplatDev](https://github.com/SplatDev-Ltda)
