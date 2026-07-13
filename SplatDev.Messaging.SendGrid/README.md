# SplatDev.Messaging.SendGrid

SendGrid email provider for `SplatDev.Messaging`. Targets `net8.0` and `net10.0`.

## Installation

```bash
dotnet add package SplatDev.Messaging.SendGrid
```

## Usage

```csharp
builder.Services.AddMessaging(o => o.UseSendGrid(config.GetSection("SendGrid")));
```

## License

MIT
