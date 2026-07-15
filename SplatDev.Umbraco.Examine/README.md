# SplatDev.Umbraco.Examine

Examine/Lucene search extensions for Umbraco CMS. Provides a no-stop-words analyzer, fuzzy/boost search helpers, typed paginated search models, and a turnkey `Search<T>()` method that queries the External Examine index and resolves hits to `IPublishedContent` — sorted by publish date descending.

## Install

```bash
dotnet add package SplatDev.Umbraco.Examine
```

Multi-targets `net8.0` (Umbraco 13) and `net10.0` (Umbraco 17).

## What's implemented

### Search engine — `ExamineExtensionsPlus.Search<T>()`

The main entry point. Queries the Umbraco **ExternalIndex** via Examine, auto-computes pagination, and resolves results to `IPublishedContent`. Works with any controller or view model implementing `IExamineExtensionsWithResults`.

```csharp
var results = model.Search(examineManager, logger, umbracoContextFactory,
    propertyAliases: new[] { "title", "bodyText", "summary" },
    documentTypeAlias: "blogPost");
```

**Query behaviour:**
- Single word → wildcarded match (`term*`)
- Multiple words → grouped OR query
- Optional document type filtering via `documentTypeAlias`
- Results sorted by `publishDate` descending
- Exceptions caught and logged — returns empty results on failure

### Term boosting and fuzzy matching

```csharp
var boosted = new[] { "important", "urgent" }.Boost(2.0f);
var fuzzy = new[] { "helo", "wrld" }.Fuzzy();
```

| Method | Description |
|--------|-------------|
| `terms.Boost(float)` | Applies a numeric boost to every term |
| `terms.Fuzzy()` | Applies fuzzy matching to every term |

### No-stop-words analyzer

`NoStopWordsAnalyzer` extends Lucene's `Analyzer` with standard tokenization and an **empty** stop-word set (size 0). All tokens pass through — no common words (the, and, of, etc.) are stripped during indexing or querying.

```csharp
// Assign to an Examine index
index.FieldDefinitionCollection.TryAdd(
    new FieldDefinition("bodyText", new NoStopWordsAnalyzer()));
```

### Models

#### `IExamineExtensionsWithResults` — search contract

Interface for any search view model or controller. Implement to receive paginated results.

```csharp
public interface IExamineExtensionsWithResults
{
    string Keywords { get; set; }
    int CurrentPage { get; set; }
    int ItemsPerPage { get; set; }
    int TotalPages { get; set; }
    int TotalItems { get; set; }
    int[] PageRange { get; set; }
    IList<IPublishedContent> Results { get; set; }
}
```

#### `PagedResults<TEntity>` / `PagedResultsViewModel`

Generic paginated container with result metadata for view rendering.

| Property | Type | Description |
|----------|------|-------------|
| `Results` | `IList<TEntity>?` | Result items for the current page |
| `Pagination` | `Pagination` | Pagination metadata |
| `FullQueryString` | `bool` | Whether to output the full query string in the UI |
| `ShowSearchTerm` | `bool` | Whether to display the search term in the UI |

Includes an explicit cast from `PagedResults<TEntity>` → `PagedResultsViewModel`.

#### `Pagination`

| Property | Type | Description |
|----------|------|-------------|
| `PageSize` | `int` | Items per page |
| `Page` | `int` | Current page number |
| `TotalPages` | `int` | Total number of pages |
| `TotalResults` | `long` | Total matching results across all pages |
| `SearchTerm` | `string?` | The search query |
| `SearchDataType` | `string?` | Label for the data type searched |

#### `SearchModel`

An Umbraco `PublishedContentWrapped` subclass for strongly-typed search result pages. Carries `Query`, `TotalResults`, `SearchDataType`, and `PagedResults<SearchResultItem>`.

#### `SearchResultItem` / `SearchResultItemDto`

| Class | Role |
|-------|------|
| `SearchResultItem` | Wraps a raw `ISearchResult` with its `SearchResultItemDto` and Lucene `Score` |
| `SearchResultItemDto` | Lightweight DTO implementing `IEntity`, `IDescription`, `IText` for generic pagination rendering |

## Usage

### Controller example

```csharp
public class SearchController : Controller
{
    private readonly IExamineManager _examineManager;
    private readonly IUmbracoContextFactory _umbracoContextFactory;
    private readonly ILogger<SearchController> _logger;

    public async Task<IActionResult> Index(SearchViewModel model)
    {
        model.Search(_examineManager, _logger, _umbracoContextFactory,
            propertyAliases: new[] { "title", "body" },
            documentTypeAlias: "article");

        return View(model);
    }
}

public class SearchViewModel : IExamineExtensionsWithResults
{
    public string Keywords { get; set; } = "";
    public int CurrentPage { get; set; } = 1;
    public int ItemsPerPage { get; set; } = 10;
    public int TotalPages { get; set; }
    public int TotalItems { get; set; }
    public int[] PageRange { get; set; } = [];
    public IList<IPublishedContent> Results { get; set; } = new List<IPublishedContent>();
}
```

### Custom analyzer

```csharp
// In an Examine index composer or config:
var analyzer = new NoStopWordsAnalyzer();
index.FieldDefinitionCollection.AddOrUpdate(
    new FieldDefinition("content", analyzer));
```

## Search options in the SplatDev suite

Both this package and [`SplatDev.Search.*`](/SplatDev.Search) coexist in the SplatDev ecosystem. They serve different purposes and can run together in the same host.

| Use case | Choose |
|----------|--------|
| Index Umbraco content nodes for backoffice/frontend search | **This package** — in-process Lucene, tightly integrated with Umbraco's content pipeline |
| Index domain data (products, tickets, cross-site aggregates) | **[`SplatDev.Search.*`](/SplatDev.Search)** — out-of-process (Elasticsearch / OpenSearch / Meilisearch) |
| Multi-tenant search across many Umbraco sites from one index | **[`SplatDev.Search.*`](/SplatDev.Search)** — Examine is per-instance |
| Zero-ops single-site search on Umbraco content | **This package** — no external service to provision |

## Dependencies

| Package | Purpose |
|---------|---------|
| `Umbraco.Cms.Core` (13.12 / 17.3) | `IExamineManager`, `IPublishedContent` |
| `Umbraco.Cms.Infrastructure` | Examine infrastructure |
| `SplatDev.Umbraco.Pagination` | `IEntity`, `IDescription`, `IText` interfaces; `PublishedContentWrapped` base class |

---

**SplatDev.Umbraco.Examine** — part of the [SplatDev.Umbraco.Plugins](https://github.com/SplatDev-Ltda/SplatDev.Umbraco.Plugins) suite. Licensed under MIT. © SplatDev Ltda.
