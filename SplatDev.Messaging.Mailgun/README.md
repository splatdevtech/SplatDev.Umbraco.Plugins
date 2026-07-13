# SplatDev.Messaging.Mailgun

Mailgun email provider for `SplatDev.Messaging`. Targets `net8.0` and `net10.0`.

## Installation

```bash
dotnet add package SplatDev.Messaging.Mailgun
```

## Usage

```csharp
builder.Services.AddMessaging(o => o.UseMailgun(config.GetSection("Mailgun")));
```

Configure via `appsettings.json`:

```json
{ "Mailgun": { "ApiKey": "", "Domain": "", "FromAddress": "" } }
```

## License

MIT
