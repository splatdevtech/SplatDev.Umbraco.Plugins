# SplatDev.Messaging

Generic messaging abstractions for .NET applications.
Targets `net8.0` and `net10.0`.

## Installation

```bash
dotnet add package SplatDev.Messaging
```

## Dependencies

- `Microsoft.Extensions.DependencyInjection.Abstractions`

## Features

- `IMessageService` — unified interface for sending messages
- `IMessageProvider` — provider abstraction for pluggable backends
- Provider discovery via `IServiceProvider`
- Message model with sender, recipient, subject, body, and attachments

## Provider Implementations

| Package | Transport |
|---|---|
| `SplatDev.Messaging.Smtp` | SMTP (email) |
| `SplatDev.Messaging.SendGrid` | SendGrid API |
| `SplatDev.Messaging.Mailgun` | Mailgun API |
| `SplatDev.Messaging.Twilio` | Twilio (SMS) |
| `SplatDev.Messaging.SocketLabs` | SocketLabs API |
| `SplatDev.Messaging.SMSTools` | SMS Tools |
| `SplatDev.Messaging.Newsletter` | Newsletter campaigns |

## Usage

### Register providers

```csharp
builder.Services.AddMessaging(options =>
{
    options.UseSmtp();
    options.UseSendGrid();
});
```

### Send a message

```csharp
public class Notifier(IMessageService messaging)
{
    public async Task SendWelcome(string email)
    {
        await messaging.SendAsync(new Message
        {
            To = email,
            Subject = "Welcome!",
            Body = "<h1>Hello</h1>",
            IsHtml = true
        });
    }
}
```

## License

MIT
