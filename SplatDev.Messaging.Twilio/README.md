# SplatDev.Messaging.Twilio

Twilio SMS adapter for [`SplatDev.Messaging`](../SplatDev.Messaging). Sends transactional SMS via the Twilio REST API.

## Install

```bash
dotnet add package SplatDev.Messaging.Twilio
```

Targets: `net8.0`, `net10.0`. Published to nuget.org. References the official [`Twilio`](https://www.nuget.org/packages/Twilio) SDK 7.x, `SplatDev.Messaging`, and framework-references `Microsoft.AspNetCore.App` for DI/options.

## What's implemented

### `TwilioSmsController : ISmsMessagingController<Sms, MessageResource>`

Implements the SMS-shaped [`ISmsMessagingController<TMessage, TResult>`](../SplatDev.Messaging/Interfaces/ISmsMessagingController.cs) — no `subject`, no `CC`/`BCC`, no HTML: just `from`, `to`, `body`.

- **Message type**: `Sms { Body, From, To }` — `From`/`To` are `Twilio.Types.PhoneNumber` (E.164 like `+15551234567`).
- **Result type**: `Twilio.Rest.Api.V2010.Account.MessageResource` — the SDK's response object (SID, status, error code, etc.).
- **Async is real**: `SendMessageAsync` calls `MessageResource.CreateAsync` directly; `SendMessage` (sync) delegates to it.
- **Per-instance `TwilioRestClient`**: constructed in the ctor from `TwilioOptions.AccountSid` + `AuthToken`. **No** global `TwilioClient.Init(...)` — safe for multi-tenant apps and multiple Twilio subaccounts.

### `TwilioOptions`

Bindable POCO for `Microsoft.Extensions.Options`.

| Property | Purpose | Default |
| --- | --- | --- |
| `AccountSid` | Twilio Account SID | *(required)* |
| `AuthToken` | Twilio Auth Token | *(required)* |
| `DefaultFrom` | Fallback `from` phone number if omitted on send | `null` |

Section constant: `TwilioOptions.DefaultSection` = `"SplatDev:Messaging:Twilio"`.

### `TwilioServiceCollectionExtensions.AddSplatDevTwilio`

```csharp
services.AddSplatDevTwilio(builder.Configuration);
// or with a custom section:
services.AddSplatDevTwilio(builder.Configuration, "MyApp:Twilio");
```

Binds `TwilioOptions` and registers `TwilioSmsController` as transient.

### `Sms` model

```csharp
public class Sms
{
    public string      Body { get; set; }
    public PhoneNumber From { get; set; }
    public PhoneNumber To   { get; set; }
}
```

## Usage

### Configuration (`appsettings.json`)

```json
{
  "SplatDev": {
    "Messaging": {
      "Twilio": {
        "AccountSid": "AC00000000000000000000000000000000",
        "DefaultFrom": "+15551234567"
      }
    }
  }
}
```

Keep `AuthToken` out of `appsettings.json`. Supply `SplatDev:Messaging:Twilio:AuthToken` via environment variable (`SplatDev__Messaging__Twilio__AuthToken`), user-secrets, or a secrets manager.

### Registration (`Program.cs`)

```csharp
using SplatDev.Messaging.Twilio.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSplatDevTwilio(builder.Configuration);
```

### Sending an SMS

```csharp
public sealed class OtpSender(TwilioSmsController sms)
{
    public async Task<string> SendOtpAsync(string phoneE164, string code)
    {
        var msg = await sms.SendMessageAsync(
            from: "",                  // falls back to DefaultFrom
            to:   phoneE164,
            body: $"Your code is {code}");

        return msg.Sid;
    }
}
```

Or with the typed `Sms` DTO when you need finer control over the request:

```csharp
var msg = await sms.SendMessageAsync(new Sms
{
    Body = "Hi there",
    From = new PhoneNumber("+15551234567"),
    To   = new PhoneNumber("+15559876543"),
});
```

Inspect `msg.Status`, `msg.ErrorCode`, `msg.Sid`.

---

**SplatDev.Messaging.Twilio** — part of the [SplatDev.Umbraco.Plugins](https://github.com/SplatDev-Ltda/SplatDev.Umbraco.Plugins) suite. Licensed under MIT. © SplatDev Ltda.
