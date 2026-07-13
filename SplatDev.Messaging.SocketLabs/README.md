# SplatDev.Messaging.SocketLabs

SocketLabs email provider for `SplatDev.Messaging`. Targets `net8.0` and `net10.0`.

## Installation

```bash
dotnet add package SplatDev.Messaging.SocketLabs
```

## Usage

```csharp
builder.Services.AddMessaging(o => o.UseSocketLabs(config.GetSection("SocketLabs")));
```

## License

MIT
