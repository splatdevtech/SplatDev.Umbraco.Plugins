# SplatDev.Umbraco.Markup

HTML markup helpers and extensions for Umbraco.
Targets Umbraco 13 (net8.0) and Umbraco 17 (net10.0).

## Installation

```bash
dotnet add package SplatDev.Umbraco.Markup
```

## Dependencies

- `Umbraco.Cms.Core`
- `HtmlAgilityPack`

## Features

- `MarkupHelper` — parse and transform HTML from Rich Text editors
- Grid and Block List HTML rendering helpers
- Lazy loading markup generation for images and iframes
- Responsive image helpers with srcset and sizes
- Link transformation (internal Umbraco links to URLs)
- HTML sanitization and clean-up

## Usage

```csharp
using SplatDev.Umbraco.Markup;

// Transform Grid editor JSON to HTML
var html = MarkupHelper.RenderGrid(content.Value<JObject>("grid"));

// Generate a responsive image
var img = MarkupHelper.ResponsiveImage(mediaItem, "hero", "banner");

// Convert internal links
var clean = MarkupHelper.ResolveInternalLinks(rawHtml);
```

## License

MIT
