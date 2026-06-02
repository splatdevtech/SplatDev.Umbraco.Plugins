# SplatDev.Payments.MercadoPago

MercadoPago payment provider for the `SplatDev.Payments` abstraction layer. Supports credit card, Pix, ticket (boleto), and subscription payments for Latin American markets.

[![NuGet](https://img.shields.io/nuget/v/SplatDev.Payments.MercadoPago)](https://www.nuget.org/packages/SplatDev.Payments.MercadoPago)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

## Compatibility

| .NET Target | Package Version |
|---|---|
| net8.0 | 1.0.x |
| net10.0 | 1.0.x |

## Features

- **Credit Card Payments** — `CardPaymentRequest` with card holder and issuer details
- **Pix Payments** — `PixPaymentRequest` for instant payments
- **Ticket/Boleto** — `TicketPaymentRequest` for boleto bancario
- **Subscriptions** — `SubscriptionRequest` with auto-recurring and free trial support
- **User Management** — `UserRequest` for payer registration
- **Payment Queries** — `PaymentsRequest` with search and `FindPaymentsResults` pagination
- Comprehensive enum set: `StatusTypes`, `PaymentMethodTypes`, `IdentificationTypes`, etc.

## Installation

```bash
dotnet add package SplatDev.Payments.MercadoPago
```

## Dependencies

- mercadopago-sdk 2.11.0
- Newtonsoft.Json 13.0.3
- RestSharp 112.1.0
- SplatDev.Payments (project reference)

## See Also

- [SplatDev.Payments](../SplatDev.Payments/) — Base payment abstractions

## License

MIT © [SplatDev](https://github.com/SplatDev-Ltda)
