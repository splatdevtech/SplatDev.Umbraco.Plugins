# SplatDev.Messaging.SocketLabs

SocketLabs email delivery adapter for the `SplatDev.Messaging` messaging framework. Supports both single and bulk transactional email via the [SocketLabs Injection API](https://www.socketlabs.com/api-reference/injection-api/) using the official `SocketLabs.EmailDelivery` SDK.

## Install

```sh
dotnet add package SplatDev.Messaging.SocketLabs
```

## What's implemented

- `SocketLabsController` — implements `IMessagingController<BasicMessage, SendResponse>`:
  - Sync send (`SendMessage`) with full parameters (subject, from, to, HTML/plain body, attachments).
  - Async send (`SendMessageAsync`) — wraps the sync call via `Task.FromResult`. Real async is pending a future update.
- `SocketLabsBulkController` — implements `IBulkMessagingController<BulkMessage, SendResponse>`:
  - Send bulk email with per-recipient merge data (`%%Placeholder%%` → value).
  - Recipients are `IBulkAddress` instances with `Name`, `Address`, and `Data` (a list of `IBulkMessageData`).
- `BulkAddress` — POCO implementing `IBulkAddress` with merge-field support.
- `BulkMessageData` — POCO implementing `IBulkMessageData` (placeholder → value pairs).
- Both controllers implement `IDisposable`. SocketLabs SDK client is managed internally.

## Configuration

No `appsettings.json` binding. Server ID and Injection API Key are passed to the constructor:

```csharp
var controller = new SocketLabsController(
    serverId: 12345,
    injectionApiKey: "your-socketlabs-api-key");
```

For production, load credentials from configuration and create the controller at startup.

## DI registration

No built-in DI extension. Register manually:

```csharp
var serverId = int.Parse(builder.Configuration["SplatDev:Messaging:SocketLabs:ServerId"]!);
var apiKey = builder.Configuration["SplatDev:Messaging:SocketLabs:InjectionApiKey"]!;
builder.Services.AddSingleton<SocketLabsController>(_ => new SocketLabsController(serverId, apiKey));
builder.Services.AddSingleton<SocketLabsBulkController>(_ => new SocketLabsBulkController(serverId, apiKey));
```

## Usage

### Single email

```csharp
var socketLabs = new SocketLabsController(serverId, apiKey);
var response = socketLabs.SendMessage(
    subject: "Welcome!",
    from: "Acme Corp",
    fromAddress: "hello@example.com",
    to: "Jane Doe",
    toAddress: "jane@example.com",
    htmlMessage: "<h1>Welcome to Acme</h1><p>Thanks for signing up.</p>");
```

### Bulk email with merge fields

```csharp
var bulk = new SocketLabsBulkController(serverId, apiKey);
var recipients = new List<IBulkAddress>
{
    new BulkAddress
    {
        Name = "Jane Doe",
        Address = "jane@example.com",
        Data = new[]
        {
            new BulkMessageData { Placeholder = "Name", Value = "Jane" },
            new BulkMessageData { Placeholder = "OrderId", Value = "1234" }
        }
    }
};

var response = bulk.BulkSendMessage(
    recipients: recipients,
    fromAddress: new EmailAddress("noreply@example.com"),
    subject: "Your Order %%OrderId%% has Shipped",
    htmlBody: "<p>Hello %%Name%%, your order %%OrderId%% is on its way.</p>",
    plainTextBody: "Hello %%Name%%, your order %%OrderId%% is on its way.");
```

---

**SplatDev.Messaging.SocketLabs** — part of the [SplatDev.Umbraco.Plugins](https://github.com/SplatDev-Ltda/SplatDev.Umbraco.Plugins) suite. Licensed under MIT. © SplatDev Ltda.
