# SplatDev.Payments

Generic payment abstractions and helpers for .NET applications. Defines provider-agnostic interfaces for payment processing, subscriptions, and order management.

[![NuGet](https://img.shields.io/nuget/v/SplatDev.Payments)](https://www.nuget.org/packages/SplatDev.Payments)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

## Compatibility

| .NET Target | Package Version |
|---|---|
| net8.0 | 1.0.x |
| net10.0 | 1.0.x |

## Features

- `IPayment` — Payment processing contract
- `IPaymentRequest` — Payment request model
- `IPaymentMethod` — Payment method abstraction
- `IOrder` — Order management contract
- `IPayer` — Payer/customer model
- `ICard` — Credit/debit card abstraction
- `IShipment` — Shipping information contract
- `ISubscription` — Recurring payment/subscription model
- `SecureConnectionHelpers` — TLS/security utilities for payment connections

## Installation

```bash
dotnet add package SplatDev.Payments
```

## Provider Implementations

| Package | Provider |
|---------|----------|
| `SplatDev.Payments.MercadoPago` | MercadoPago (Latin America) |
| `SplatDev.Payments.BancoInter` | Banco Inter (Brazil — Pix, Boleto) |
| `SplatDev.Payments.PagSeguro` | PagSeguro (Brazil) |
| `SplatDev.Payments.Stripe` | Stripe (Global) |

## License

MIT © [SplatDev](https://github.com/SplatDev-Ltda)
