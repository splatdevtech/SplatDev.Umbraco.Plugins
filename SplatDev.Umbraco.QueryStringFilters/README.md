# SplatDev.Umbraco.QueryStringFilters

Query string filter helpers for Umbraco content queries.
Targets Umbraco 13 (net8.0) and Umbraco 17 (net10.0).

## Installation

```bash
dotnet add package SplatDev.Umbraco.QueryStringFilters
```

## Features

- Parse query string parameters into Umbraco content filters
- Built-in filters: document type, date range, property value, tags
- Integrates with Umbraco `IPublishedContentQuery`
- Supports chaining multiple filters

## Usage

```csharp
var filters = QueryStringFilters.Parse(Request.Query);
var results = Umbraco.ContentAtRoot()
    .ApplyFilters(filters)
    .Take(20);
```

## License

MIT
