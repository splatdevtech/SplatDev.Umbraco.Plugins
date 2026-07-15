# SplatDev.Payments.Stripe

Stripe payment provider for the SplatDev.Payments abstraction layer — intended to implement `IPayment<T>` and related interfaces.

[![NuGet](https://img.shields.io/nuget/v/SplatDev.Payments.Stripe.svg)](https://www.nuget.org/packages/SplatDev.Payments.Stripe)

## Compatibility

| .NET | Package Version |
|------|-----------------|
| 8.0  | 1.0.0           |
| 10.0 | 1.0.0           |

## Installation

```sh
dotnet add package SplatDev.Payments.Stripe
```

## What's implemented

This package is a **scaffold** — it references `SplatDev.Payments` (the base abstraction package defining `IPayment<T>`, `IPayment`, `ICard`, `IPayer`, `IOrder`, `IPaymentMethod`, `IShipment`, and `ISubscription`) and the official `Stripe.net` SDK (v46.2.1), but contains **no implementation code**. It is a placeholder for a future Stripe payment adapter.

## Configuration

No configuration is required. This package does not bind any options POCO, appsettings keys, or DI registration extensions.

## Usage

Not yet available. Once implemented, usage will follow the `SplatDev.Payments` provider pattern:

```csharp
// Planned pattern — not functional yet
services.AddSplatDevPayments()
        .AddStripe(options => { options.ApiKey = "..."; });
```

## Dependencies

| Package | Purpose |
|---------|---------|
| `SplatDev.Payments` | Base payment abstractions (`IPayment<T>`, `IOrder`, etc.) |
| `Stripe.net` 46.2.1 | Official Stripe .NET SDK |
| `Newtonsoft.Json` 13.0.3 | JSON serialization |

## Caveats

- **Empty assembly.** This package produces a DLL with no public types. Do not install expecting a working payment provider.
- **Development status.** Awaiting assignment and implementation. Track progress in the [Umbraco Plugins repository](https://github.com/SplatDev-Ltda/SplatDev.Umbraco.Plugins).

---

**SplatDev.Payments.Stripe** — part of the [SplatDev.Umbraco.Plugins](https://github.com/SplatDev-Ltda/SplatDev.Umbraco.Plugins) suite. Licensed under MIT. &copy; SplatDev Ltda.
