# SplatDev.Umbraco.Examine

Examine/Lucene search extensions for Umbraco CMS. Enhanced search capabilities with custom analyzers, paged results, and strongly-typed search result models.

[![NuGet](https://img.shields.io/nuget/v/SplatDev.Umbraco.Examine)](https://www.nuget.org/packages/SplatDev.Umbraco.Examine)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

## Compatibility

| Umbraco Version | .NET Target | Package Version |
|---|---|---|
| Umbraco 13.x | net8.0 | 2.0.x |
| Umbraco 17.x | net10.0 | 2.0.x |

## Features

- `ExamineExtensionsPlus` — Extended search query building and execution
- `NoStopWordsAnalyzer` — Custom Lucene analyzer that preserves stop words
- `SearchModel` / `Search` — Structured search request models
- `SearchResultItem` / `SearchResultItemDto` — Strongly-typed search result models
- `PagedResults` / `Pagination` — Paginated search results with metadata
- `IExamineExtensionsWithResults` — Interface for components that return search results

## Installation

```bash
dotnet add package SplatDev.Umbraco.Examine
```

## Dependencies

- SplatDev.Umbraco.Pagination (project reference)

## License

MIT © [SplatDev](https://github.com/SplatDev-Ltda)
