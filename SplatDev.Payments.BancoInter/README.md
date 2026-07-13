# SplatDev.Payments.BancoInter

Banco Inter payment provider for `SplatDev.Payments`. Targets `net8.0` and `net10.0`.

## Installation

```bash
dotnet add package SplatDev.Payments.BancoInter
```

## Usage

```csharp
builder.Services.AddPayments(o => o.UseBancoInter(config.GetSection("BancoInter")));
```

## License

MIT
