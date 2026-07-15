# SplatDev.Messaging.Smtp

`System.Net.Mail`-backed SMTP adapter for [`SplatDev.Messaging`](../SplatDev.Messaging). Sends single-recipient HTML mail through any standard SMTP server, with truly async I/O and `IOptions`-based configuration.

## Install

```bash
dotnet add package SplatDev.Messaging.Smtp
```

Targets: `net8.0`, `net10.0`. Published to nuget.org. Pulls in `SplatDev.Messaging` transitively; framework-references `Microsoft.AspNetCore.App` for DI, configuration, options, and the `HttpContext` used by `SendException`.

## What's implemented

### `SmtpController : IMessagingController<MailMessage, SmtpResult>`

- **Message type**: `System.Net.Mail.MailMessage`.
- **Result type**: `SmtpResult { bool Success, string Message, Exception? Exception }`.
- **HTML by default**: the string-based overloads set `IsBodyHtml = true`.
- **CC / BCC**: pass `IEnumerable<IAddress>` on the string-based overloads.
- **Event**: `event MailerSent OnMailerSent` fires after every attempt (success or failure) with an `SmtpResultEventArgs`.
- **Real async**: `SendMessageAsync` calls `SmtpClient.SendMailAsync` directly. `SendMessage` (sync) delegates to it.
- **Utility**: `SendException(Exception, HttpContext?, from, to, applicationName)` formats an exception (URL, message, inner, stack) as an HTML email.
- **Disposal**: full `IDisposable` pattern; disposes the underlying `SmtpClient`.

Two constructors:

- `SmtpController()` — parameterless, uses default `SmtpClient()`; kept for legacy callers.
- `SmtpController(SmtpOptions options)` — configures host, port, SSL, and credentials from options.

**Not implemented**: `IBulkMessagingController`. For mail-merge, use `SplatDev.Messaging.SendGrid`, `.Mailgun`, or `.SocketLabs`.

### `SmtpOptions`

Bindable POCO for `Microsoft.Extensions.Options`.

| Property | Purpose | Default |
| --- | --- | --- |
| `Host` | SMTP server hostname | *(required)* |
| `Port` | SMTP port | `587` |
| `EnableSsl` | Use SSL/STARTTLS | `true` |
| `User` | Auth username (null → no auth) | `null` |
| `Password` | Auth password | `null` |
| `DefaultFromAddress` | Fallback if `fromAddress` omitted on send | `null` |
| `DefaultFromName` | Fallback if `from` omitted on send | `null` |

Section constant: `SmtpOptions.DefaultSection` = `"SplatDev:Messaging:Smtp"`.

### `SmtpServiceCollectionExtensions.AddSplatDevSmtp`

One-line DI registration:

```csharp
services.AddSplatDevSmtp(builder.Configuration);
// or with a custom section:
services.AddSplatDevSmtp(builder.Configuration, "MyApp:Smtp");
```

Binds `SmtpOptions` from the given section and registers `SmtpController` as transient.

### `SmtpResult` / `SmtpResultEventArgs`

Plain DTOs — `Success`, `Message`, optional `Exception`.

## Usage

### Configuration (`appsettings.json`)

```json
{
  "SplatDev": {
    "Messaging": {
      "Smtp": {
        "Host": "smtp.example.com",
        "Port": 587,
        "EnableSsl": true,
        "User": "postmaster@example.com",
        "DefaultFromAddress": "no-reply@example.com",
        "DefaultFromName": "SplatDev"
      }
    }
  }
}
```

Keep secrets out of `appsettings.json`. Supply `SplatDev:Messaging:Smtp:Password` via environment variable (`SplatDev__Messaging__Smtp__Password`), user-secrets, or a secrets manager.

### Registration (`Program.cs`)

```csharp
using SplatDev.Messaging.Smtp.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSplatDevSmtp(builder.Configuration);
```

### Sending a message

```csharp
public sealed class WelcomeMailer(SmtpController smtp)
{
    public async Task SendAsync(string toName, string toAddress, CancellationToken ct = default)
    {
        smtp.OnMailerSent += (_, e) =>
        {
            if (!e.Success) log.LogError(e.Exception, "SMTP send failed: {Message}", e.Message);
        };

        var result = await smtp.SendMessageAsync(
            subject:      "Welcome",
            from:         "",                     // falls back to DefaultFromName
            fromAddress:  "",                     // falls back to DefaultFromAddress
            to:           toName,
            toAddress:    toAddress,
            message:      "<h1>Hi</h1>",
            plainMessage: "");

        if (!result.Success) throw result.Exception!;
    }
}
```

### Sending an exception report

```csharp
try { /* … */ }
catch (Exception ex)
{
    smtp.SendException(ex, HttpContext, "errors@splatdev.tech", "ops@splatdev.tech", "MyApp");
}
```

---

**SplatDev.Messaging.Smtp** — part of the [SplatDev.Umbraco.Plugins](https://github.com/SplatDev-Ltda/SplatDev.Umbraco.Plugins) suite. Licensed under MIT. © SplatDev Ltda.
