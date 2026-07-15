# SplatDev.Payments.MercadoPago

[Mercado Pago](https://www.mercadopago.com.br/developers) payment integration for the `SplatDev.Payments` framework. Supports credit/debit card, Pix, and boleto payments, plus subscriptions and payment queries. Uses the official `mercadopago-sdk` v2 NuGet package.

## Install

```sh
dotnet add package SplatDev.Payments.MercadoPago
```

## What's implemented

### Payment gateways
- `CardPaymentRequest` — credit/debit card: tokenize, create, capture, cancel. Implements `IPayment<Payment>`.
- `PixPaymentRequest` — Pix instant payment. Implements `IPayment<Payment>`.
- `TicketPaymentRequest` — boleto / bank slip. Implements `IPayment<Payment>`.

### Queries & subscriptions
- `PaymentRequests` — search payments by sort/criteria, get single payment, list identification types and available payment methods.
- `SubscriptionRequest` — full subscription lifecycle: create, search, pause/activate/cancel, create preapproval plan.
- `UserRequest` — user-related API operations.

### Models
- `Payment`, `Pix`, `Ticket` — full payment models implementing `IPayment` from `SplatDev.Payments`.
- `CreditCard` — card tokenization model implementing `ICard` + `IPaymentMethod`.
- `Payer`, `Order`, `Subscription`, `PreApprovalPlan` — domain types.
- `TransactionDetails`, `PayerCosts`, `FeeDetails`, `Shipment`, `CardHolder`, `Issuer` — supporting POCOs.

### Enums with helpers
- `StatusTypes` — payment status (Authorized, Rejected, Refunded, ChargedBack…).
- `PaymentMethodTypes` — gateway types (AccountMoney, Pix, CreditCard, DebitCard, Boleto…).
- `IdentificationTypes`, `EntityTypes`, `OrderTypes`, `PayerTypes`, `FrequencyTypes`, `BarcodeTypes`.
- Each enum has a `*Helper` class providing `StringName()` and `EnumName()` for serialization.

### Constants
- `Constants` — API base URLs, JSON serialization defaults (snake_case naming, ISO dates).

## Configuration

No built-in `appsettings.json` binding. API keys are passed to constructors:

```csharp
var cardRequest = new CardPaymentRequest(
    publicKey: "TEST-...",
    accessToken: "TEST-...",
    referrer: "https://your-site.com");
```

For production, load credentials from configuration:

```json
{
  "SplatDev": {
    "Payments": {
      "MercadoPago": {
        "PublicKey": "",
        "AccessToken": ""
      }
    }
  }
}
```

## DI registration

No built-in DI extensions. Register your payment service manually:

```csharp
var publicKey = builder.Configuration["SplatDev:Payments:MercadoPago:PublicKey"]!;
var accessToken = builder.Configuration["SplatDev:Payments:MercadoPago:AccessToken"]!;

builder.Services.AddTransient<CardPaymentRequest>(_ =>
    new CardPaymentRequest(publicKey, accessToken));
builder.Services.AddTransient<PixPaymentRequest>(_ =>
    new PixPaymentRequest(publicKey, accessToken));
```

## Usage

### Credit card payment

```csharp
var card = new CardPaymentRequest(publicKey, accessToken);
var payment = new Payment
{
    TransactionAmount = 99.90m,
    Description = "Widget Pro Annual",
    PaymentMethodId = "mastercard",
    Payer = new Payer { Email = "jane@example.com" }
};
var result = await card.CreatePaymentRequestAsync(payment);
```

### Pix payment

```csharp
var pix = new PixPaymentRequest(publicKey, accessToken);
var pixPayment = new Payment
{
    TransactionAmount = 49.90m,
    Description = "Widget Monthly",
    Payer = new Payer { Email = "jane@example.com" }
};
var pixResult = await pix.CreatePaymentRequestAsync(pixPayment);
```

### Query payments

```csharp
var client = new PaymentRequests(publicKey, accessToken);
var recent = await client.GetPaymentsAsync(sort: "date_created", criteria: "desc");
```

### Subscriptions

```csharp
var subs = new SubscriptionRequest(publicKey, accessToken);
var plan = new PreApprovalPlan
{
    Reason = "Widget Pro Monthly",
    AutoRecurring = new AutoRecurring
    {
        Frequency = 1,
        FrequencyType = "months",
        TransactionAmount = 49.90m
    }
};
await subs.CreateSubscriptionAsync(new Subscription { PreApprovalPlan = plan, PayerEmail = "jane@example.com" });
```

---

**SplatDev.Payments.MercadoPago** — part of the [SplatDev.Umbraco.Plugins](https://github.com/SplatDev-Ltda/SplatDev.Umbraco.Plugins) suite. Licensed under MIT. © SplatDev Ltda.
