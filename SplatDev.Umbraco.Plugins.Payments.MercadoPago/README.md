# Payments.MercadoPago

MercadoPago payment integration for Umbraco. Supports preference creation, payment status lookup, and webhook handling. Supports Umbraco 13 (net8.0) and Umbraco 17 (net10.0).

[![NuGet](https://img.shields.io/nuget/v/SplatDev.Umbraco.Plugins.Payments.MercadoPago.svg)](https://www.nuget.org/packages/SplatDev.Umbraco.Plugins.Payments.MercadoPago)

## Compatibility

| Umbraco | .NET | Package Version |
|---------|------|-----------------|
| 13.x    | 8.0  | 2.0.0           |
| 17.x    | 10.0 | 2.0.0           |

## Installation

```sh
dotnet add package SplatDev.Umbraco.Plugins.Payments.MercadoPago
```

## Quick Start

Register in `Program.cs`:

```csharp
builder.CreateUmbracoBuilder()
    .AddBackOffice()
    .AddWebsite()
    .AddMercadoPago()   // <-- add this
    .Build();
```

## Configuration

Add to `appsettings.json`:

```json
{
  "MercadoPago": {
    "AccessToken": "",
    "PublicKey": "",
    "WebhookSecret": ""
  }
}
```

## License

MIT © [SplatDev](https://github.com/SplatDev-Ltda)
