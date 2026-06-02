# SplatDev.Api.Common.ApiVersioning

API versioning extension methods for ASP.NET Core applications. Provides a streamlined setup for `Microsoft.AspNetCore.Mvc.Versioning`.

## Target Framework

- net8.0

## Features

- One-call registration of API versioning via `ApiVersioningExtensions`
- Configures default API version, version reporting, and API explorer integration

## Usage

```csharp
builder.Services.AddApiVersioningDefaults();
```

## Dependencies

- Microsoft.AspNetCore.Mvc.Versioning 5.0.0
- Microsoft.AspNetCore.Mvc.ApiExplorer 8.0.0

## License

MIT © [SplatDev](https://github.com/SplatDev-Ltda)
