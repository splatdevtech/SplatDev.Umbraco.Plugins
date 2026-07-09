# SplatDev.Umbraco.Plugins.ShortUrls

Umbraco plugin providing a generic short-URL redirect engine and backoffice management dashboard. Define your own EF Core entity implementing `IShortUrl`, call `AddShortUrls<T>()` in DI, and you get a `GET /s/{code}` redirect endpoint plus a Lit-based CRUD dashboard under the Settings section.

## Install

```bash
dotnet add package SplatDev.Umbraco.Plugins.ShortUrls
```

Multi-targets `net8.0` (Umbraco 13) and `net10.0` (Umbraco 17). Published to nuget.org.

## What's implemented

### `IShortUrl` (entity contract)

Your EF Core entity must implement this interface:

```csharp
public interface IShortUrl
{
    string? ShortUrl { get; set; }   // short code (e.g. "abc123")
    string Url { get; set; }         // target URL
}
```

### `ShortUrlService<T>` : IShortUrlService

The generic service handles code generation and lookup:

```csharp
public class ShortUrlService<T> : IShortUrlService where T : class, IShortUrl
```

| Method | Description |
|--------|-------------|
| `GenerateShortUrl()` | Generates a unique 7-char alphanumeric code (collision-safe) |
| `Get(shortUrl)` | Looks up the target URL by short code |
| `Exists(random)` | Checks if a code already exists |

### `ShortUrlController<IShortUrl>` : UmbracoPageController

Handles `GET /s/{shortCode}`:

```csharp
[HttpGet]
public IActionResult GetFromShortUrl(string shortUrl)
{
    var redirectUrl = _shortUrlService.Get(shortUrl);
    return Redirect(redirectUrl);
}
```

### Backoffice Dashboard (Lit)

Appears under **Settings → Short URLs** with English and Portuguese (Brazil) localization:

| Feature | Status |
|---------|--------|
| Table with Short Code, Target URL, Created date | Renders |
| Create/Edit form (Short Code + Target URL) | UI wired |
| Edit/Delete buttons per row | UI wired |
| Backend API (`/umbraco/management/api/v1/short-urls`) | Pending Phase 3 deployment |

The dashboard renders a notice when the backend API is not yet available.

## Usage

### 1. Define your entity

```csharp
public class MyShortUrl : IShortUrl
{
    public int Id { get; set; }
    public string? ShortUrl { get; set; }
    public string Url { get; set; }
    public DateTime Created { get; set; } = DateTime.UtcNow;
}
```

### 2. Register in DI

```csharp
public class StartupComposer : IComposer
{
    public void Compose(IUmbracoBuilder builder)
    {
        builder.AddShortUrls<MyShortUrl>();
    }
}
```

### 3. Create short URLs

```csharp
var svc = serviceProvider.GetRequiredService<IShortUrlService>();
var code = svc.GenerateShortUrl(); // e.g. "aB3_xY7"
```

### 4. Access them

```
GET /s/aB3_xY7 → 302 Redirect to your target URL
```

## Configuration

No appsettings keys. The plugin uses your existing `DbContext` with the entity implementing `IShortUrl`.

## DI Registration

```csharp
builder.AddShortUrls<YourEntity>();  // where YourEntity : class, IShortUrl
```

This registers `IShortUrlService` → `ShortUrlService<YourEntity>` as scoped.

## Dependencies

| Package | Purpose |
|---------|---------|
| `Umbraco.Cms.Core` (13.12 / 17.3.4) | Composers, controllers, DI |
| `Umbraco.Cms.Infrastructure` | Infrastructure services |
| `Umbraco.Cms.Web.Common` | Web hosting |
| `Umbraco.Cms.Web.Website` | Website routing |
| `Microsoft.EntityFrameworkCore` + `.SqlServer` | Entity persistence |

---

**SplatDev.Umbraco.Plugins.ShortUrls** — part of the [SplatDev.Umbraco.Plugins](https://github.com/SplatDev-Ltda/SplatDev.Umbraco.Plugins) suite. Licensed under MIT. © SplatDev Ltda.
