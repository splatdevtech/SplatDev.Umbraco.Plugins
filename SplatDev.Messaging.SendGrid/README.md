# SplatDev.Messaging.SendGrid

SendGrid transactional email adapter for the `SplatDev.Messaging` messaging framework. Sends single emails via the [SendGrid API v3](https://docs.sendgrid.com/api-reference/mail-send/mail-send) using the official `SendGrid` NuGet SDK.

## Install

```sh
dotnet add package SplatDev.Messaging.SendGrid
```

## What's implemented

- `SendGridController` — implements `IMessagingController<SendGridMessage, Response>` from `SplatDev.Messaging`. Supports:
  - Async send (`SendMessageAsync`) with full parameter set (subject, from, to, HTML body, plain-text fallback, BCC, CC).
  - Sync send (`SendMessage`) accepting a native `SendGridMessage` object for advanced features (attachments, dynamic templates, categories, custom args).
  - Both overloads return `Response` with status code and body.
- `IDisposable` — the controller wraps a `SendGridClient` under the hood. Dispose when done.

## Configuration

No `appsettings.json` binding. The API key is passed directly to the constructor:

```csharp
var controller = new SendGridController("SG.your-sendgrid-api-key");
```

For production, load the key from configuration:

```json
{
  "SplatDev": {
    "Messaging": {
      "SendGrid": {
        "ApiKey": ""
      }
    }
  }
}
```

Then bind it manually or via `IOptions<T>` in your own setup.

## DI registration

No built-in DI extension. Register manually in `Program.cs`:

```csharp
var apiKey = builder.Configuration["SplatDev:Messaging:SendGrid:ApiKey"]!;
builder.Services.AddSingleton<SendGridController>(_ => new SendGridController(apiKey));
```

## Usage

### Async send

```csharp
var sendGrid = new SendGridController(apiKey);
var response = await sendGrid.SendMessageAsync(
    subject: "Your Order Confirmation",
    from: "Acme Store",
    fromAddress: "orders@example.com",
    to: "Jane Doe",
    toAddress: "jane@example.com",
    message: "<h1>Order #1234 Confirmed</h1><p>Thank you!</p>",
    plainMessage: "Order #1234 Confirmed. Thank you!",
    bcc: new[] { "archive@example.com" },
    cc: new[] { "support@example.com" });
```

### Sync send with native SendGridMessage

```csharp
var msg = MailHelper.CreateSingleEmail(
    new EmailAddress("noreply@example.com", "Acme"),
    new EmailAddress("jane@example.com", "Jane"),
    "Your Order Update",
    "Your order has shipped.",
    "<strong>Your order has shipped.</strong>");

var response = sendGrid.SendMessage(msg);
```

---

**SplatDev.Messaging.SendGrid** — part of the [SplatDev.Umbraco.Plugins](https://github.com/SplatDev-Ltda/SplatDev.Umbraco.Plugins) suite. Licensed under MIT. © SplatDev Ltda.
