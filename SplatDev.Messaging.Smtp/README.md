# SplatDev.Messaging.Smtp

SMTP email provider for `SplatDev.Messaging`.
Targets `net8.0` and `net10.0`.

## Installation

```bash
dotnet add package SplatDev.Messaging.Smtp
```

## Dependencies

- `SplatDev.Messaging`
- `MailKit` — SMTP client library

## Features

- SMTP provider implementing `IMessageProvider`
- TLS/SSL support
- HTML and plain-text email
- Attachment support
- Configurable timeout and retry

## Usage

### Configure

```json
{
  "Smtp": {
    "Host": "smtp.example.com",
    "Port": 587,
    "Username": "noreply@example.com",
    "Password": "",
    "UseSsl": true,
    "FromAddress": "noreply@example.com",
    "FromName": "My App"
  }
}
```

### Register

```csharp
builder.Services.AddMessaging(options =>
{
    options.UseSmtp(builder.Configuration.GetSection("Smtp"));
});
```

## License

MIT
