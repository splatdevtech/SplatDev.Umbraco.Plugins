# SplatDev.Umbraco.Common

Common Umbraco extensions and utilities. A shared library of extension methods, content finders, and helpers used across SplatDev Umbraco plugins.

[![NuGet](https://img.shields.io/nuget/v/SplatDev.Umbraco.Common)](https://www.nuget.org/packages/SplatDev.Umbraco.Common)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

## Compatibility

| Umbraco Version | .NET Target | Package Version |
|---|---|---|
| Umbraco 13.x | net8.0 | 2.0.x |
| Umbraco 17.x | net10.0 | 2.0.x |

## Features

### Content Finders
- `SanitizedUrlContentFinder` — URL sanitization and content resolution

### Extension Methods
- `AlphabetExtensions` — Alphabetical indexing helpers
- `CookieExtensions` — Cookie read/write helpers
- `DataTypeExtensions` — Umbraco data type utilities
- `EnumExtensions` — Enum display and conversion helpers
- `FallbackExtensions` — Content fallback strategies
- `PaginationExtensions` — Collection pagination helpers
- `PublishedContentExtensions` — IPublishedContent navigation and querying
- `PublishedContentPropertyExtensions` — Property value extraction
- `QueryableExtensions` — IQueryable filtering and sorting
- `RuntimeMinifierExtensions` — Client-side asset minification
- `StringExtensions` — String manipulation utilities
- `TempDataExtensions` — TempData serialization helpers
- `TypeCheckingExtensions` — Type validation helpers

### Security
- CSP and security header support via Joonasw.AspNetCore.SecurityHeaders and NWebsec

## Installation

```bash
dotnet add package SplatDev.Umbraco.Common
```

## License

MIT © [SplatDev](https://github.com/SplatDev-Ltda)
