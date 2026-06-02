# SplatDev.Payments.BancoInter

Banco Inter payment gateway models, settings, and API contracts for Pix, Boleto, and Banking integrations in .NET applications.

[![NuGet](https://img.shields.io/nuget/v/SplatDev.Payments.BancoInter)](https://www.nuget.org/packages/SplatDev.Payments.BancoInter)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

## Compatibility

| .NET Target | Package Version |
|---|---|
| net8.0 | 1.0.x |
| net10.0 | 1.0.x |

## Features

- **Pix** — `InterPixCharge`, `InterPixChargeResponse`, `InterPixPayment` models
- **Boleto** — `InterBoleto`, `InterBoletoResponse` models
- **Banking** — `InterBankingBalance` for account balance queries
- **Authentication** — `InterTokenResponse` for OAuth token management
- **Webhooks** — `InterWebhookPayload` for payment event notifications
- `BancoInterSettings` — Configuration model for API credentials and endpoints

## Installation

```bash
dotnet add package SplatDev.Payments.BancoInter
```

## Configuration

```json
{
  "BancoInter": {
    "ClientId": "your-client-id",
    "ClientSecret": "your-client-secret",
    "CertificatePath": "/path/to/certificate.pfx"
  }
}
```

## See Also

- [SplatDev.Payments](../SplatDev.Payments/) — Base payment abstractions

## License

MIT © [SplatDev](https://github.com/SplatDev-Ltda)
