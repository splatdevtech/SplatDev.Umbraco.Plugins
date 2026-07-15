# SplatDev.Api.Common.ApiVersioning

Opinionated ASP.NET Core API versioning setup in a single `IServiceCollection` extension. Wraps `Asp.Versioning.Mvc` with pre-baked defaults: default version `1.0`, query-string and header version reading, Swagger explorer configured, and version-reporting headers enabled. Drop it into any ASP.NET Core project and start versioning endpoints.

## Install

```bash
dotnet add package SplatDev.Api.Common.ApiVersioning
```

Multi-targets `net8.0` and `net10.0`. Published to nuget.org.

## What's implemented

### `AddSplatApiVersioning`

A single extension method that wires up everything:

```csharp
using SplatDev.Api.Common.ApiVersioning;

// In Program.cs:
builder.Services.AddSplatApiVersioning();
```

Under the hood it calls two `Asp.Versioning` services:

#### Core versioning (`AddApiVersioning`)

| Option | Value |
|--------|-------|
| `DefaultApiVersion` | `1.0` |
| `AssumeDefaultVersionWhenUnspecified` | `true` |
| `ReportApiVersions` | `true` |
| `ApiVersionReader` | Query string (`?api-version=2.0`) AND custom header (`X-API-Version: 2.0`) |

URL path-segment versioning (`/api/v2/controller`) is **not** enabled. Clients select the version via query string or header.

#### API explorer (`AddApiExplorer`)

| Option | Value |
|--------|-------|
| `GroupNameFormat` | `'v'VVVV` — Swagger documents named `v1`, `v2`, etc. |
| `SubstituteApiVersionInUrl` | `true` — route template placeholders are expanded in OpenAPI |

## Usage

Annotate controllers with `[ApiVersion]` and `[MapToApiVersion]`:

```csharp
[ApiController]
[Route("api/reports")]
[ApiVersion("1.0")]
public class ReportsV1Controller : ControllerBase
{
    [HttpGet]
    public IActionResult Get() => Ok(new { version = 1 });
}

[ApiController]
[Route("api/reports")]
[ApiVersion("2.0")]
public class ReportsV2Controller : ControllerBase
{
    [HttpGet]
    public IActionResult Get() => Ok(new { version = 2, extraField = true });
}
```

Clients call:
```
GET /api/reports                    → defaults to 1.0
GET /api/reports?api-version=2.0    → 2.0
GET /api/reports                    → 2.0 (with header: X-API-Version: 2.0)
```

Response headers will include:
```
api-supported-versions: 1.0, 2.0
api-deprecated-versions: 0.9
```

## Dependencies

| Package | Purpose |
|---------|---------|
| `Asp.Versioning.Mvc` (8.1.0) | Core API versioning middleware and conventions |
| `Asp.Versioning.Mvc.ApiExplorer` (8.1.0) | Swagger/OpenAPI document grouping and URL substitution |
| `Microsoft.AspNetCore.App` (framework reference) | ASP.NET Core hosting and `IServiceCollection` |

---

**SplatDev.Api.Common.ApiVersioning** — part of the [SplatDev.Umbraco.Plugins](https://github.com/SplatDev-Ltda/SplatDev.Umbraco.Plugins) suite. Licensed under MIT. © SplatDev Ltda.
