# SplatDev.Messaging.Twilio

Twilio SMS provider for `SplatDev.Messaging`. Targets `net8.0` and `net10.0`.

## Installation

```bash
dotnet add package SplatDev.Messaging.Twilio
```

## Usage

```csharp
builder.Services.AddMessaging(o => o.UseTwilio(config.GetSection("Twilio")));
```

## License

MIT
