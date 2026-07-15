# SplatDev.Payments.PagSeguro

PagSeguro payment provider for the SplatDev.Payments abstraction layer — intended to implement `IPayment<T>` and related interfaces.

[![NuGet](https://img.shields.io/nuget/v/SplatDev.Payments.PagSeguro.svg)](https://www.nuget.org/packages/SplatDev.Payments.PagSeguro)

## Compatibility

| .NET | Package Version |
|------|-----------------|
| 8.0  | 1.0.0           |
| 10.0 | 1.0.0           |

## Installation

```sh
dotnet add package SplatDev.Payments.PagSeguro
```

## What's implemented

This package is a **scaffold** — it references `SplatDev.Payments` (the base abstraction package defining `IPayment<T>`, `IPayment`, `ICard`, `IPayer`, `IOrder`, `IPaymentMethod`, `IShipment`, and `ISubscription`), along with `RestSharp` and `Newtonsoft.Json` as HTTP/JSON dependencies, but contains **no implementation code**. It is a placeholder for a future PagSeguro payment adapter.

> For a working, Umbraco-integrated PagSeguro solution, see **`SplatDev.Umbraco.Plugins.Payments.PagSeguro`** (v2.0.1), which integrates directly with Umbraco via `IComposer` and EF Core.

## Configuration

No configuration is required. This package does not bind any options POCO, appsettings keys, or DI registration extensions.

## Usage

Not yet available. Once implemented, usage will follow the `SplatDev.Payments` provider pattern:

```csharp
// Planned pattern — not functional yet
services.AddSplatDevPayments()
        .AddPagSeguro(options => { options.Token = "..."; });
```

## Dependencies

| Package | Purpose |
|---------|---------|
| `SplatDev.Payments` | Base payment abstractions (`IPayment<T>`, `IOrder`, etc.) |
| `Newtonsoft.Json` 13.0.3 | JSON serialization |
| `RestSharp` 112.1.0 | HTTP client for PagSeguro API |

## Caveats

- **Empty assembly.** This package produces a DLL with no public types. Do not install expecting a working payment provider.
- **Development status.** Awaiting assignment and implementation. Track progress in the [Umbraco Plugins repository](https://github.com/SplatDev-Ltda/SplatDev.Umbraco.Plugins).

---

**SplatDev.Payments.PagSeguro** — part of the [SplatDev.Umbraco.Plugins](https://github.com/SplatDev-Ltda/SplatDev.Umbraco.Plugins) suite. Licensed under MIT. &copy; SplatDev Ltda.
