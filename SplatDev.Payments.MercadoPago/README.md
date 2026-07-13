# SplatDev.Payments.MercadoPago

Mercado Pago payment provider for `SplatDev.Payments`. Targets `net8.0` and `net10.0`.

## Installation

```bash
dotnet add package SplatDev.Payments.MercadoPago
```

## Features

- Checkout (Web, Transparent)
- Credit card, debit card, boleto, PIX
- Subscription (plans, billing cycles)
- Webhook integration for payment status updates
- Refund and cancellation support
- Sandbox mode for testing

## Usage

```csharp
builder.Services.AddPayments(o => o.UseMercadoPago(config.GetSection("MercadoPago")));
```

Configure:

```json
{
  "MercadoPago": {
    "AccessToken": "",
    "PublicKey": "",
    "Sandbox": true
  }
}
```

## License

MIT
