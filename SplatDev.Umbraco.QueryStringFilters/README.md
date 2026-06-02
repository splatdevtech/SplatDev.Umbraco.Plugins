# SplatDev.Umbraco.QueryStringFilters

Query string filtering middleware for Umbraco CMS. Provides middleware and extension methods for sanitizing and stripping query string parameters from incoming requests.

[![NuGet](https://img.shields.io/nuget/v/SplatDev.Umbraco.QueryStringFilters)](https://www.nuget.org/packages/SplatDev.Umbraco.QueryStringFilters)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

## Compatibility

| Umbraco Version | .NET Target | Package Version |
|---|---|---|
| Umbraco 13.x | net8.0 | 2.0.x |
| Umbraco 17.x | net10.0 | 2.0.x |

## Features

- `QueryStringStripMiddleware` — ASP.NET Core middleware that strips or sanitizes specified query string parameters
- `QueryStringExtensions` — Helper methods for query string manipulation

## Installation

```bash
dotnet add package SplatDev.Umbraco.QueryStringFilters
```

## Usage

```csharp
app.UseMiddleware<QueryStringStripMiddleware>();
```

## Dependencies

- Microsoft.AspNetCore.App (framework reference)

## License

MIT © [SplatDev](https://github.com/SplatDev-Ltda)
