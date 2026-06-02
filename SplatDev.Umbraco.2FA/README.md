# SplatDev.Umbraco.2FA

Two-factor authentication utilities for Umbraco CMS. Provides extension methods to configure 2FA authorization in Umbraco backoffice.

[![NuGet](https://img.shields.io/nuget/v/SplatDev.Umbraco.2FA)](https://www.nuget.org/packages/SplatDev.Umbraco.2FA)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

## Compatibility

| Umbraco Version | .NET Target | Package Version |
|---|---|---|
| Umbraco 13.x | net8.0 | 2.0.x |
| Umbraco 17.x | net10.0 | 2.0.x |

## Features

- `TwoFactorAuthorizationExtensions` — Extension methods to register and configure 2FA in the Umbraco pipeline

## Installation

```bash
dotnet add package SplatDev.Umbraco.2FA
```

## Usage

```csharp
builder.CreateUmbracoBuilder()
    .AddBackOffice()
    .AddWebsite()
    .AddTwoFactorAuthorization()  // Add this
    .Build();
```

## See Also

- [SplatDev.Umbraco.Plugins.2fa](../SplatDev.Umbraco.Plugins.2fa/) — Full 2FA plugin with TOTP, backup codes, and backoffice dashboard

## License

MIT © [SplatDev](https://github.com/SplatDev-Ltda)
