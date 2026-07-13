# SplatDev.Messaging.SMSTools

SMS tools provider for `SplatDev.Messaging`. Targets `net8.0` and `net10.0`.

## Installation

```bash
dotnet add package SplatDev.Messaging.SMSTools
```

## Usage

```csharp
builder.Services.AddMessaging(o => o.UseSmsTools(config.GetSection("SMSTools")));
```

## License

MIT
