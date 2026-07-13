# SplatDev.Content

Content helper utilities including CSV import, Excel import, HTML processing,
encryption, and QR code generation. Targets `net8.0` and `net10.0`.

## Installation

```bash
dotnet add package SplatDev.Content
```

## Dependencies

- `CsvHelper` — CSV parsing
- `EPPlus` — Excel (.xlsx) read/write
- `QRCoder` — QR code generation
- `HtmlAgilityPack` — HTML parsing

## Features

- **CSV Import** — parse CSV files into typed collections
- **Excel Import** — read .xlsx files with column mapping
- **HTML Helpers** — sanitize, strip tags, extract text
- **Encryption** — AES encryption/decryption for strings
- **QR Generation** — generate QR codes as images or Base64 strings

## Usage

### CSV import

```csharp
using SplatDev.Content.Csv;

var products = CsvImporter.Import<Product>(fileStream);
```

### Excel import

```csharp
using SplatDev.Content.Excel;

var rows = ExcelImporter.Import<Dictionary<string, object>>(filePath);
```

### Generate a QR code

```csharp
using SplatDev.Content.Qr;

var base64 = QrGenerator.GenerateAsBase64("https://example.com");
```

### Encrypt / Decrypt

```csharp
using SplatDev.Content.Encryption;

var encrypted = AesHelper.Encrypt("sensitive data", key);
var decrypted = AesHelper.Decrypt(encrypted, key);
```

## License

MIT
