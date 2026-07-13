# SplatDev.Payments.Stripe

Stripe payment provider for `SplatDev.Payments`. Targets `net8.0` and `net10.0`.

## Installation

```bash
dotnet add package SplatDev.Payments.Stripe
```

## Usage

```csharp
builder.Services.AddPayments(o => o.UseStripe(config.GetSection("Stripe")));
```

Configure:

```json
{ "Stripe": { "SecretKey": "", "PublishableKey": "", "WebhookSecret": "" } }
```

## License

MIT
