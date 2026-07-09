# SplatDev.Umbraco.Common

Common extension methods, utilities, content finders, and shared models for Umbraco applications. A curated collection of 13+ extension classes covering content, queries, strings, HTML minification, cookies, pagination, and more — shared across the SplatDev Umbraco plugin ecosystem.

## Install

```bash
dotnet add package SplatDev.Umbraco.Common
```

Multi-targets `net8.0` (Umbraco 13) and `net10.0` (Umbraco 17). Requires the matching Umbraco CMS NuGet packages.

## What's implemented

### Published Content extensions

**`PublishedContentPropertyExtensions`** — caller-member-name-aware property access. When called from a model property getter, `[CallerMemberName]` auto-derives the Umbraco property alias, eliminating magic strings.

```csharp
public string Heading => this.GetPropertyValue<string>(); // alias = "Heading"
public string Summary => this.GetPropertyWithDefaultValue("No summary available.");
public int Count => this.GetPropertyWithDefaultValue(() => ComputeDefaultCount());
```

**`FallbackExtensions`** — ancestor-based and hardcoded fallback values for `IPublishedContent`.

| Method | Behaviour |
|--------|-----------|
| `ValueFallbackNodeName(alias)` | Returns property value, or `Name` if empty |
| `ValueWithFallback(alias)` | Falls back to ancestor values via `Fallback.ToAncestors` |
| `ValueWithFallback(alias, defaultText)` | Returns property value or the supplied default string |

**`PublishedContentExtensions`** — typed traversal helpers and grid inspection.

| Method | Behaviour |
|--------|-----------|
| `HasGridValue(alias)` | Detects whether the Umbraco Grid property has at least one control in any area |
| `Children<T>(predicate)` | Typed children filtered by predicate |
| `Descendants<T>(predicate)` | Typed descendants filtered by predicate |
| `DescendantsOrSelf<T>(predicate)` | Typed descendants-or-self filtered by predicate |
| `GetPublishedContentOfType<T>(name)` | Finds the first descendant of a given content type by string alias or generic type (net8.0 only; Umbraco 17 has built-in equivalents) |

### String and text utilities

**`StringExtensions`** — 33 methods covering slug generation, casing, encoding, hashing, date formatting, and content tokenization.

| Category | Methods |
|----------|---------|
| **Slug/ID generation** | `GenerateSlug`, `NameToSlug`, `NameToId`, `NameToSnake`, `Slugify`, `TextToId` |
| **Case conversion** | `CamelCaseToDashed`, `DashCaseToSpacedWords`, `SnakeToCamel`, `SnakeToCapitalized`, `FirstCharToUpper`, `LowerCaseFirstLetter`, `CapitalizeFirstLetters`, `CapitalizeFirstLettersDash` |
| **Encoding** | `Encode`, `ToBase64`, `ToHash` (MD5) |
| **URL helpers** | `ApplyTrailingSlash`, `GenerateRandomUrl`, `ToMailLink`, `ToPhoneLink` |
| **Content tools** | `CountWords`, `StripHtmlTags`, `Tokenize` (double-quote-aware tokenizer), `GetTwoCharLangCode` |
| **Date formatting** | `FormatDateString`, `DateSlug`, `FormattedDate`, `GetMonthName`, `ParseMonthNameToInt` |
| **Misc** | `ComputeFourDigitStringHash`, `GetXmlAsUtf8String` |

All slug/ID methods include ASCII transliteration of diacritics and regex-based sanitization powered by `[GeneratedRegex]` source generators.

### Queryable dynamic ordering

**`QueryableExtensions`** — runtime `OrderBy`/`ThenBy` by property name string. Supports dot-notation for nested properties.

```csharp
var sorted = db.Products
    .OrderBy("Category.Name")
    .ThenByDescending("Price");
```

Four methods: `OrderBy`, `OrderByDescending`, `ThenBy`, `ThenByDescending`. All build `LambdaExpression` trees via `System.Linq.Expressions` and return `null` if the property is not found.

### Pagination

**`PaginationExtensions`** — page math and Fisher-Yates shuffle.

| Method | Description |
|--------|-------------|
| `GetTotalPages(totalResults, pageSize)` | Ceiling division, minimum 1 |
| `PagedResults(page, pageSize)` | 1-based `Skip/Take` over `IEnumerable<T>` |
| `Shuffle()` | In-place Fisher-Yates shuffle |

```csharp
var totalPages = PaginationExtensions.GetTotalPages(247, 20); // 13
var pageTwo = products.PagedResults(2, 20);
var randomized = products.ToList().Shuffle();
```

### Runtime HTML minification

**`RuntimeMinifierExtensions`** — post-processes HTML string fragments containing `<script>` or `<link>` tags to add or update attributes via source-generated regex.

```csharp
@Html.Raw(myScriptTag.Defer())
@Html.Raw(myLinkTag.PreloadCss())
@Html.Raw(myTag.AddAttributes(new Dictionary<string, string> { ["crossorigin"] = "anonymous" }))
```

Methods: `Defer()`, `Async()`, `PreloadJs()`, `PreloadCss()`, `AddAttributes(Dictionary<string,string>)`.

### Cookie and TempData helpers

**`CookieExtensions`** — read/write cookies on `HttpContext`.

```csharp
context.SaveCookie("theme", "dark", new CookieOptions { Expires = DateTime.UtcNow.AddDays(30) });
var theme = context.ReadCookie("theme");
```

**`TempDataExtensions`** — JSON-serialized strongly-typed TempData storage.

```csharp
tempData.PutJson("searchFilters", myFilters);
var filters = tempData.GetJson<SearchFilters>("searchFilters");
```

### Enum and type checking

**`EnumExtensions`** — reads `[Display(Name = "...")]` from enum members.

```csharp
Status.Active.GetDisplayName(); // "Active"
```

**`TypeCheckingExtensions`** — alias comparison helpers.

| Method | Description |
|--------|-------------|
| `alias.AliasEquals<T>()` | Case-insensitive alias match against a `PublishedElementModel` type name |
| `alias.ExcludedAliases(string[])` | Checks if alias is in an exclusion list |

### Data type helpers (net8.0 only)

**`DataTypeExtensions`** — reads dropdown/value-list prevalues from `IDataTypeService`. Excluded on net10.0 because Umbraco 17 changed the management API.

```csharp
var items = dataTypeService.GetDropdownPreValues(dataTypeId);
var dict = DataTypeExtensions.GetByDataTypeName(dataTypeService, "myDropdown", keyId: 0);
```

### Other utilities

**`AlphabetExtensions`** — returns a 26-element `char[]` of uppercase letters A–Z.

## Content finders

**`SanitizedUrlContentFinder`** — implements `IContentFinder`. Intercepts `/quote/by-*` URL paths, strips query strings, and issues a redirect to the clean path. Registered alongside other content finders in Umbraco's routing pipeline.

```csharp
// In a composer:
builder.ContentFinders().Insert<SanitizedUrlContentFinder>();
```

## Models

| Model | Purpose |
|-------|---------|
| `SimpleSelectListItem` | General-purpose select list item with `Text`, `Alias`, `Value` (object), and optional `Group`. Annotated with `[JsonPropertyName]` for Management API serialization. |
| `SimpleSelectListItemInteger` | Strongly-typed variant with `Value` as `int`. |

```csharp
var items = new[]
{
    new SimpleSelectListItemInteger { Text = "One", Alias = "opt1", Value = 1 },
    new SimpleSelectListItemInteger { Text = "Two", Alias = "opt2", Value = 2, Group = "Group A" },
};
```

## DI Registration

No DI registration needed. All extension classes are static. Use directly via `using SplatDev.Umbraco.Common.Extensions;` (and sub-namespaces for models and content finders).

## Dependencies

| Package | Purpose |
|---------|---------|
| `Umbraco.Cms.Core` (13.12 / 17.3) | Core Umbraco types |
| `Umbraco.Cms.Infrastructure` | `IDataTypeService` and related services |
| `Umbraco.Cms.Web.Common` / `Umbraco.Cms.Web.Website` | HTTP context, routing, published content |
| `Umbraco.Cms.Web.BackOffice` (13) / `Umbraco.Cms.Api.Management` (17) | Back-office / management API |
| `Joonasw.AspNetCore.SecurityHeaders` (6.0.0) | Security header middleware |
| `NWebsec.AspNetCore.Middleware` (3.0.0) | Web security middleware |

---


**SplatDev.Umbraco.Common** — part of the [SplatDev.Umbraco.Plugins](https://github.com/SplatDev-Ltda/SplatDev.Umbraco.Plugins) suite. Licensed under MIT. © SplatDev Ltda.
