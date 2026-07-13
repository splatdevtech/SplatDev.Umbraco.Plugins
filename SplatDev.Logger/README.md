# SplatDev.Logger

Simple database-backed logging utility using Entity Framework Core.
Targets `net8.0` and `net10.0`.

## Installation

```bash
dotnet add package SplatDev.Logger
```

## Dependencies

- `Microsoft.EntityFrameworkCore`
- `Microsoft.EntityFrameworkCore.SqlServer`

## Features

- Static `Logger.Log()` methods for synchronous logging
- `LoggerDbContext` for EF Core database persistence
- `LogType` enum: `Info`, `Warning`, `Error`, `Debug`
- Captures message, details, user, timestamp, and exception info

## Usage

### Configure

```csharp
SplatDev.Logger.Logger.ConnectionString = "Server=.;Database=MyApp;...";
```

### Log messages

```csharp
Logger.Log("User registered", "userId=42", LogType.Info, "admin@example.com");
Logger.Log("Payment failed", exception, LogType.Error, "system");
```

### Database schema

The `Log` entity maps to a `Logs` table:

| Column | Type | Description |
|---|---|---|
| `Id` | `int` | Primary key |
| `Message` | `string` | Log message |
| `Details` | `string` | Additional context |
| `DateTime` | `DateTime` | Timestamp |
| `User` | `string` | Actor identifier |
| `LogType` | `LogType` | Severity level |

> **Note**: The `catch { }` in `Logger.Log()` silently swallows failures.
> In production, register the `LoggerDbContext` via DI and use `ILogger<T>`
> instead. This static logger is intended for simple scenarios and
> non-critical logging.

## Relationship to `ILogger`

This package provides a self-contained, database-backed logger. For structured
logging with dependency injection and multi-sink support, prefer `ILogger<T>`
with a provider like Serilog. The `SplatDev.Logger` package is a convenient
fallback when DI is not available.

## License

MIT
