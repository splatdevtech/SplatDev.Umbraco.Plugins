# Payments.PagSeguro

PagSeguro payment integration for Umbraco. Supports checkout session creation, transaction status lookup. Supports Umbraco 13 (net8.0) and Umbraco 17 (net10.0).

[![NuGet](https://img.shields.io/nuget/v/SplatDev.Umbraco.Plugins.Payments.PagSeguro.svg)](https://www.nuget.org/packages/SplatDev.Umbraco.Plugins.Payments.PagSeguro)

## Compatibility

| Umbraco | .NET | Package Version |
|---------|------|-----------------|
| 13.x    | 8.0  | 2.0.0           |
| 17.x    | 10.0 | 2.0.0           |

## Installation

```sh
dotnet add package SplatDev.Umbraco.Plugins.Payments.PagSeguro
```

## Quick Start

Register in `Program.cs`:

```csharp
builder.CreateUmbracoBuilder()
    .AddBackOffice()
    .AddWebsite()
    .AddPagSeguro()   // <-- add this
    .Build();
```

## Configuration

Add to `appsettings.json`:

```json
{
  "PagSeguro": {
    "Token": "",
    "Environment": "sandbox"
  }
}
```

## License

MIT © [SplatDev](https://github.com/SplatDev-Ltda)
