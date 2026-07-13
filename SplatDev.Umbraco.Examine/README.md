# SplatDev.Umbraco.Examine

Examine / Lucene search extensions for Umbraco.
Targets Umbraco 13 (net8.0) and Umbraco 17 (net10.0).

## Installation

```bash
dotnet add package SplatDev.Umbraco.Examine
```

## Dependencies

- `Umbraco.Cms.Core` — Examine is bundled with Umbraco
- `SplatDev.Umbraco.Pagination` — paginated search results

## Features

- `ExamineSearcher` — high-level wrapper for Examine queries
- `SearchResult<T>` — typed search results with scores and highlights
- Content and media index searching
- Paginated search with `GetPagedResultsAsync`
- Faceted search support
- Multi-index querying
- Rebuild and optimize index triggers

## Usage

### Search content

```csharp
using SplatDev.Umbraco.Examine;

var results = await ExamineSearcher.SearchAsync<IPublishedContent>(
    "ExternalIndex",
    query: "lorem ipsum",
    page: 1,
    pageSize: 10);
```

### Search with filters

```csharp
var results = await ExamineSearcher.SearchAsync<IPublishedContent>(
    "ExternalIndex",
    query: "product",
    filters: new Dictionary<string, string>
    {
        ["category"] = "electronics",
        ["__NodeTypeAlias"] = "product"
    });
```

### Paginated results

```csharp
var paged = await searcher.GetPagedResultsAsync(query, page, pageSize);

foreach (var item in paged.Results)
    Console.WriteLine($"{item.Score}: {item.Value.Name}");
```

## Relationship to SplatDev.Search

`SplatDev.Search` (planned) provides a higher-level abstraction above Examine
with provider-agnostic interfaces. This package stays close to the Examine API
for Umbraco-native usage.

## License

MIT
