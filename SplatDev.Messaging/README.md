# SplatDev.Messaging

Provider-agnostic messaging abstractions used by every `SplatDev.Messaging.*` adapter (SMTP, SendGrid, Mailgun, SocketLabs, Twilio, SMSTools, Newsletter). This package ships the interfaces, address models, and the canned-template engine — it does **not** send anything on its own. Reference it directly when writing a new adapter, or transitively through one of the provider packages when consuming.

## Install

```bash
dotnet add package SplatDev.Messaging
```

Targets: `net8.0`, `net10.0`. Published to nuget.org.

## What's implemented

### Controller contracts

Every adapter implements one or more of these:

- `IMessagingController<TMessage, TResult>` — single-recipient email send. Sync and async overloads; **prefer the async overloads** (sync delegates to async in every SplatDev adapter).
- `IBulkMessagingController<TMessage, TResult>` — bulk email send with per-recipient placeholder substitution via `IBulkAddress.Data`.
- `ISmsMessagingController<TMessage, TResult>` — SMS send. Distinct interface from email: no `subject`, no `CC`/`BCC`, no HTML — just `from`, `to`, `body`. Implemented by `SplatDev.Messaging.Twilio` and `SplatDev.Messaging.SMSTools`.

All three are `IDisposable`; the adapter decides what disposal releases (HTTP client, SMTP client, etc.).

### Address model

- `IAddress` — `Name`, `Address` — a single recipient/sender.
- `IBulkAddress` — `IAddress` + `IEnumerable<IBulkMessageData> Data` — a recipient with a per-row placeholder set (used for mail-merge–style bulk sends).
- `IBulkMessageData` — `Placeholder` / `Value` pair.

### Canned message templates

Reusable message bodies with `#PLACEHOLDER#` markers.

- `ICannedMessageTemplate` — `TemplateName`, `Body`, `FormattedBody`, `Placeholders`.
- `CannedMessageTemplate` — default implementation.
- `CannedMessagePlaceholder` — `Key` / `Value`.
- `DefaultPlaceholders` — a set of pre-defined placeholder keys you can reuse across templates.
- `CannedMessageTemplates` — static registry. **Starts empty**; your app is responsible for seeding it at startup (e.g. from config, a CMS document, or an installer). Lookup helpers: `Templates`, `Template(key)`, `TemplatePlaceholder(template, key)`, `Placeholder(key)`.

### Helpers

- `CannedMessageHelpers.UsingTemplate(template, values)` — reflects the public properties of `values` into `#PROP#` markers.
- `CannedMessageHelpers.GenerateMessageFromTemplate(templateName, canned?)` — resolves a template from the registry (or the one passed in) and substitutes its `Placeholders`.
- `CannedMessageHelpers.GenerateMessageFromCanned(canned)` — substitutes an ad-hoc `ICannedMessageTemplate` without touching the registry.

## Usage

### Seeding templates at startup

```csharp
CannedMessageTemplates.Templates.Add(new CannedMessageTemplate
{
    TemplateName = "welcome",
    Body = "Hi #NAME#, welcome to #SITE#.",
    Placeholders = new[]
    {
        new CannedMessagePlaceholder { Key = "#NAME#", Value = "" },
        new CannedMessagePlaceholder { Key = "#SITE#", Value = "SplatDev" },
    },
});
```

### Rendering from an ad-hoc object

```csharp
var body = CannedMessageHelpers.UsingTemplate(
    "Hi #Name#, your order #Order# is ready.",
    new { Name = "Ada", Order = "1042" });
```

### Sending via an adapter

```csharp
using var mail = new SmtpMessagingController(options);   // or SendGrid/Mailgun/…
await mail.SendMessageAsync(
    subject:     "Welcome",
    from:        "SplatDev",
    fromAddress: "no-reply@splatdev.tech",
    to:          "Ada Lovelace",
    toAddress:   "ada@example.com",
    message:     body,
    plainMessage: "");
```

## Implementing a new provider

1. Reference `SplatDev.Messaging`.
2. Define your provider's message DTO (`TMessage`) and result type (`TResult`).
3. Implement `IMessagingController<TMessage, TResult>` — start with `SendMessageAsync`; have the sync overload block on it.
4. If the provider supports mail-merge, also implement `IBulkMessagingController<TMessage, TResult>`.
5. Dispose any HTTP/SMTP clients in `Dispose()`.

See `SplatDev.Messaging.Smtp` for the simplest reference adapter and `SplatDev.Messaging.SendGrid` for one with an HTTP-based provider client.

---

**SplatDev.Messaging** — part of the [SplatDev.Umbraco.Plugins](https://github.com/SplatDev-Ltda/SplatDev.Umbraco.Plugins) suite. Licensed under MIT. © SplatDev Ltda.
