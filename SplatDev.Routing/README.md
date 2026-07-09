# SplatDev.Routing

Convention-based route registration for ASP.NET Core. Define routes as `IRoute` implementations and register them in bulk with a single `MapSplatDevRoutes()` call — no repetitive `MapControllerRoute` boilerplate.

## Install

```bash
dotnet add package SplatDev.Routing
```

Targets `net8.0` and `net10.0`. References `Microsoft.AspNetCore.Routing`.

## How it works

### Define routes as classes

Implement `IRoute` for each MVC controller route you want to register:

```csharp
using SplatDev.Routing.Interfaces;

public class ProductsRoute : IRoute
{
    public string RouteAlias => "products";
    public string Url        => "products/{action}/{id?}";
    public string Controller => "Products";
    public string Action     => "Index";
    public object? Defaults  => null;
    public int? RootId       => null;
    public string? RootAlias => null;
    public bool IsPluginController => false;
}
```

| Property | Description |
|----------|-------------|
| `RouteAlias` | Unique route name |
| `Url` | URL pattern (no leading `/`) |
| `Controller` | Controller name (without `Controller` suffix) |
| `Action` | Action method name |
| `Defaults` | Route defaults — return `null` for automatic `{controller, action}` |
| `RootId` | Optional Umbraco root node ID |
| `RootAlias` | Optional Umbraco root node alias |
| `IsPluginController` | `true` if the controller is in a plugin/area |

### Register in Program.cs

```csharp
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();

var app = builder.Build();
app.UseRouting();

// Scan a specific assembly
app.MapSplatDevRoutes(typeof(ProductsRoute).Assembly);

// Or scan all loaded assemblies
app.MapSplatDevRoutes();

// Chain with other middleware
app.MapSplatDevRoutes().MapRazorPages();

app.Run();
```

`MapSplatDevRoutes` scans the given assemblies for concrete classes implementing `IRoute`, instantiates each, and calls `MapControllerRoute` with the route's properties.

## API

### `IRoute` interface

```csharp
public interface IRoute
{
    string RouteAlias { get; }
    string Url { get; }
    string Controller { get; }
    string Action { get; }
    object? Defaults { get; }
    int? RootId { get; }
    string? RootAlias { get; }
    bool IsPluginController { get; }
}
```

### `RouteServiceExtensions.MapSplatDevRoutes`

```csharp
public static IEndpointRouteBuilder MapSplatDevRoutes(
    this IEndpointRouteBuilder endpoints,
    params Assembly[] assemblies)
```

If no assemblies are provided, all assemblies in the current `AppDomain` are scanned.

## Dependencies

- `Microsoft.AspNetCore.Routing` — `IEndpointRouteBuilder`, `MapControllerRoute`

---


**SplatDev.Routing** — part of the [SplatDev.Umbraco.Plugins](https://github.com/SplatDev-Ltda/SplatDev.Umbraco.Plugins) suite. Licensed under MIT. © SplatDev Ltda.
