# SplatDev.Messaging.Newsletter

Newsletter campaign provider for `SplatDev.Messaging`. Targets `net8.0` and `net10.0`.

## Installation

```bash
dotnet add package SplatDev.Messaging.Newsletter
```

## Usage

```csharp
builder.Services.AddMessaging(o => o.UseNewsletter(config.GetSection("Newsletter")));
```

## License

MIT
