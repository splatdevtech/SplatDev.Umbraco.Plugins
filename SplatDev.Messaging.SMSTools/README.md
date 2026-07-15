# SplatDev.Messaging.SMSTools

SMSTools24 SMS adapter for [`SplatDev.Messaging`](../SplatDev.Messaging). Sends transactional SMS via the SMSTools24 REST API.

## Install

```bash
dotnet add package SplatDev.Messaging.SMSTools
```

Targets: `net8.0`, `net10.0`. Published to nuget.org. References `RestSharp` 112.x for HTTP, `SplatDev.Messaging`, and framework-references `Microsoft.AspNetCore.App` for DI/options.

## What's implemented

### `SmsToolsController : ISmsMessagingController<Sms, SmsToolsResult>`

Implements the SMS-shaped [`ISmsMessagingController<TMessage, TResult>`](../SplatDev.Messaging/Interfaces/ISmsMessagingController.cs) — no `subject`, no `CC`/`BCC`, no HTML: just `from`, `to`, `body`.

- **Message type**: `SplatDev.Messaging.Models.Sms` — string-based `Body`, `From`, `To` (provider-neutral, no vendor lock-in).
- **Result type**: `SmsToolsResult { bool Success, string? MessageId, string? Status, string? Message, string? RawResponse }`.
- **Async is real**: `SendMessageAsync` calls `RestClient.ExecuteAsync` → `POST /sms/send` with bearer-token auth. `SendMessage` (sync) delegates to it.
- **Per-instance `RestClient`**: constructed in the ctor from `SmsToolsOptions.BaseUrl`. No static/mutable state — safe for multi-tenant apps.

### `SmsToolsOptions`

Bindable POCO for `Microsoft.Extensions.Options`.

| Property | Purpose | Default |
| --- | --- | --- |
| `ApiKey` | SMSTools24 API key (bearer token) | *(required)* |
| `BaseUrl` | SMSTools24 API base URL | `"https://api.smstools24.com"` |
| `DefaultFrom` | Fallback `from` phone number if omitted on send | `null` |

Section constant: `SmsToolsOptions.DefaultSection` = `"SplatDev:Messaging:SMSTools"`.

### `SmsToolsServiceCollectionExtensions.AddSplatDevSmsTools`

```csharp
services.AddSplatDevSmsTools(builder.Configuration);
// or with a custom section:
services.AddSplatDevSmsTools(builder.Configuration, "MyApp:SMSTools");
```

Binds `SmsToolsOptions` and registers `SmsToolsController` as transient.

### `SmsToolsResult`

```csharp
public class SmsToolsResult
{
    public bool    Success      { get; set; }
    public string? MessageId    { get; set; }
    public string? Status       { get; set; }
    public string? Message      { get; set; }
    public string? RawResponse  { get; set; }
}
```

## Usage

### Configuration (`appsettings.json`)

```json
{
  "SplatDev": {
    "Messaging": {
      "SMSTools": {
        "ApiKey": "your-smstools24-api-key",
        "BaseUrl": "https://api.smstools24.com",
        "DefaultFrom": "+15551234567"
      }
    }
  }
}
```

Keep `ApiKey` out of `appsettings.json`. Supply `SplatDev:Messaging:SMSTools:ApiKey` via environment variable (`SplatDev__Messaging__SMSTools__ApiKey`), user-secrets, or a secrets manager.

### Registration (`Program.cs`)

```csharp
using SplatDev.Messaging.SMSTools.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSplatDevSmsTools(builder.Configuration);
```

### Sending an SMS

```csharp
public sealed class OtpSender(SmsToolsController sms)
{
    public async Task<string> SendOtpAsync(string phoneE164, string code)
    {
        var result = await sms.SendMessageAsync(
            from: "",
            to:   phoneE164,
            body: $"Your code is {code}");

        if (!result.Success)
            throw new InvalidOperationException(result.Message ?? "SMS send failed");

        return result.MessageId ?? "unknown";
    }
}
```

Or with the typed `Sms` DTO:

```csharp
var result = await sms.SendMessageAsync(new Sms
{
    Body = "Hi there",
    From = "+15551234567",
    To   = "+15559876543",
});

if (result.Success)
    Console.WriteLine($"SMS queued: {result.MessageId}");
```

---

**SplatDev.Messaging.SMSTools** — part of the [SplatDev.Umbraco.Plugins](https://github.com/SplatDev-Ltda/SplatDev.Umbraco.Plugins) suite. Licensed under MIT. © SplatDev Ltda.
