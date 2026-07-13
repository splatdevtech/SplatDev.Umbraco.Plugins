# SplatDev.Umbraco.Common

Common Umbraco extension methods and helpers.
Targets Umbraco 13 (net8.0) and Umbraco 17 (net10.0).

## Installation

```bash
dotnet add package SplatDev.Umbraco.Common
```

## Dependencies

- `Umbraco.Cms.Core`
- `Umbraco.Cms.Web.Common`

## Features

- Extension methods for `IPublishedContent`, `IContentService`, and `IMediaService`
- `ContentHelper` — get content by alias, URL, or document type
- `MediaHelper` — resolve media URLs, crops, and focal points
- `SettingsHelper` — read configuration from Umbraco dictionary or content nodes
- `CachingHelper` — runtime cache wrappers with configurable TTL
- Utility methods for common Umbraco backoffice patterns

## Usage

```csharp
using SplatDev.Umbraco.Common;

// Get content by document type alias
var articles = ContentHelper
    .GetByDocumentType("article")
    .Take(10);

// Resolve a media crop URL
var imageUrl = MediaHelper
    .GetCropUrl(mediaItem, "hero", "banner");

// Cache a heavy operation
var result = CachingHelper
    .GetOrSet("dashboard-stats", () => ComputeStats(), TimeSpan.FromMinutes(5));
```

## License

MIT
