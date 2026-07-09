# SplatDev.Umbraco.Markup

HTML markup generation, Block Grid rendering, and content helper extensions for Umbraco CMS. Generates `<img>`, `<video>`, `<audio>`, and download link markup from Block Grid items, renders Razor views to strings, and provides property access helpers with automatic alias deduction.

## Install

```bash
dotnet add package SplatDev.Umbraco.Markup
```

Multi-targets `net8.0` (Umbraco 13) and `net10.0` (Umbraco 17).

## What's implemented

### Block Grid markup generation

`GridExtensions` generates HTML from Block Grid items, auto-detecting the media type from the block's content type alias.

```csharp
@using SplatDev.Umbraco.Markup.Extensions
@using SplatDev.Umbraco.Markup.Models

// From a Block Grid area — renders the first block
@Html.Raw(blockGridArea.GetMarkup())

// From a single Block Grid item
@Html.Raw(blockGridItem.GetMarkup())

// Explicit tag type and property alias
@Html.Raw(mediaItem.GetMarkup("mediaFile", TagType.Image, UrlMode.Absolute))
```

**Generated HTML by `TagType`:**

| Tag | Output |
|-----|--------|
| `Image` | `<img src='{url}' alt='{altText}'/>` + optional `<span class="caption">` and `<p class="source">` |
| `Video` | `<video width="320" height="240" style="width:100%;height:100vh" controls>` |
| `Audio` | `<audio controls src="{url}"></audio>` |
| `File` | `<a href='{url}' target='_blank' download>{name}</a>` |
| `RichText` | Raw string value of the property (no wrapping) |

### Razor view rendering

`RazorExtensions.RenderViewAsync<TModel>()` renders any Razor view to a string — useful for email templates, PDF generation, or API responses.

```csharp
using SplatDev.Umbraco.Markup.Extensions;

public class EmailController : Controller
{
    public async Task<IActionResult> Send()
    {
        var model = new WelcomeModel { Name = "User" };
        var html = await this.RenderViewAsync("Emails/Welcome", model, partial: true);

        await _emailService.SendAsync("user@example.com", "Welcome", html);
        return Ok();
    }
}
```

### Published content property helpers

```csharp
// Auto-deduced property alias from calling property name
public string Heading => this.GetPropertyValue<string>();

// With explicit fallback
public string Summary => this.GetPropertyWithDefaultValue("No summary available.");

// Lazy fallback via factory
public int Count => this.GetPropertyWithDefaultValue(() => ComputeDefaultCount());
```

### Content traversal

```csharp
// Typed children/descendants with predicate filters
var articles = homePage.Children<ArticlePage>(a => a.IsFeatured);
var allNews = siteRoot.Descendants<NewsPage>(n => n.IsPublished);

// Grid inspection
bool hasContent = page.HasGridValue("bodyGrid");
```

### HTML and media helpers

```csharp
// Strip HTML tags, handle line breaks
string plain = html.ConvertHtmlToPlainText();

// Remove outer <p> wrapper if present
string unwrapped = text.RemoveParagraphWrapperTags();

// Media alt text with fallback to Name
string alt = image.AltText();

// Remove accent marks
string ascii = HtmlExtensions.GetWithoutAccent(htmlHelper, "café"); // "cafe"
```

### Type-safe content lookup (Umbraco 13 only)

```csharp
// Find first HomePage in the content tree
var home = publishedContentCache.GetPublishedContentOfType<HomePage>();

// By document type alias
var settings = publishedContentCache.GetPublishedContentOfType("siteSettings");
```

## Dependencies

| Package | Purpose |
|---------|---------|
| `Umbraco.Cms.Core` (13.12 / 17.3) | `IPublishedContent`, Block Grid types |
| `Umbraco.Cms.Web.Common` / `Umbraco.Cms.Web.Website` | Razor view rendering, routing |
| `Newtonsoft.Json` (13.0.3) | JSON parsing for grid inspection |

## Note on Common vs Markup

The `SplatDev.Umbraco.Markup` package shares several extension methods with `SplatDev.Umbraco.Common` (`HasGridValue`, `Children<T>`, `Descendants<T>`, `GetPublishedContentOfType`). These methods exist in **both** packages for historical reasons. If you reference both packages from the same project, you may need to use fully qualified names or `using static` aliases to resolve ambiguities.

---


**SplatDev.Umbraco.Markup** — part of the [SplatDev.Umbraco.Plugins](https://github.com/SplatDev-Ltda/SplatDev.Umbraco.Plugins) suite. Licensed under MIT. © SplatDev Ltda.
