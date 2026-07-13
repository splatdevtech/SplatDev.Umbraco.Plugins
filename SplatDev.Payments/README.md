# SplatDev.Payments

Generic payment abstractions and interfaces for .NET payment processing.
Framework-independent — no external service dependency. Targets `net8.0` and `net10.0`.

## Installation

```bash
dotnet add package SplatDev.Payments
```

## Features

- `IPayment` — payment details with method, amount, and description
- `IPayer` — payer information (name, email, document)
- `IOrder` — order metadata (ID, items, total)
- `IPaymentMethod` — payment method details (card, token, type)
- `ICard` — credit card data (number, expiry, CVV)

These interfaces define the contract for payment provider implementations
(e.g. Stripe, PayPal, MercadoPago, PagSeguro).

## Usage

### Implement a payment model

```csharp
using SplatDev.Payments.Interfaces;

public class OrderPayment : IPayment
{
    public IPaymentMethod Details { get; set; }
    public string PaymentMethodId { get; set; } = "";
    public decimal? TransactionAmount { get; set; }
    public string Description { get; set; } = "";
}
```

### Implement a provider

```csharp
public class StripeProvider : IPaymentProvider
{
    public Task<IPayment> ProcessAsync(IOrder order, IPayer payer, IPaymentMethod method)
    {
        // Map interfaces to Stripe API calls
    }
}
```

## Provider Implementations

| Package | Provider |
|---|---|
| `SplatDev.Payments.Stripe` | Stripe |
| `SplatDev.Payments.PayPal` | PayPal |
| `SplatDev.Payments.MercadoPago` | Mercado Pago (Brazil) |
| `SplatDev.Payments.PagSeguro` | PagSeguro (Brazil) |
| `SplatDev.Payments.BancoInter` | Banco Inter (Brazil) |

## License

MIT
