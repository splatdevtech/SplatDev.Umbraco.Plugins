# SplatDev.Api.Common.ApiVersioning

API versioning helpers for ASP.NET Core. Targets `net8.0` and `net10.0`.

## Installation

```bash
dotnet add package SplatDev.Api.Common.ApiVersioning
```

## Features

- URL path versioning (`/api/v1/`, `/api/v2/`)
- Header-based versioning
- Query string versioning
- Default version fallback

## Usage

```csharp
builder.Services.AddApiVersioning(o =>
{
    o.DefaultApiVersion = new ApiVersion(1, 0);
    o.AssumeDefaultVersionWhenUnspecified = true;
});
```

## License

MIT
