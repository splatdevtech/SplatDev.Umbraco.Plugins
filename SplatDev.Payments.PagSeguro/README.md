# SplatDev.Payments.PagSeguro

PagSeguro payment provider for `SplatDev.Payments`. Targets `net8.0` and `net10.0`.

## Installation

```bash
dotnet add package SplatDev.Payments.PagSeguro
```

## Usage

```csharp
builder.Services.AddPayments(o => o.UsePagSeguro(config.GetSection("PagSeguro")));
```

## License

MIT
