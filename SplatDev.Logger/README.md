# SplatDev.Logger

Simple database-backed logger using EF Core and SQL Server for .NET 8 / .NET 10 applications.

[![NuGet](https://img.shields.io/nuget/v/SplatDev.Logger.svg)](https://www.nuget.org/packages/SplatDev.Logger)

## What it provides

- `Logger` — static class with `Log()` method; writes structured log entries to SQL Server via EF Core
- `Log` — entity with `DateTime`, `LogType`, `Message`, `Details`, and `User` fields
- `LogType` — `Error`, `Info`, `Warning` enum
- `LoggerDbContext` — EF Core `DbContext`; connect it to any SQL Server database

## Target frameworks

| Framework | Typical host |
|-----------|-------------|
| net8.0    | Umbraco 13, .NET 8 apps |
| net10.0   | Umbraco 17, .NET 10 apps |

## Installation

```sh
dotnet add package SplatDev.Logger
```

## Quick Start

Set the connection string before first use:

```csharp
SplatDev.Logger.Logger.ConnectionString =
    builder.Configuration.GetConnectionString("DefaultConnection")!;
```

Log from anywhere in your code:

```csharp
Logger.Log("Backup completed", details: "10 files archived", type: LogType.Info, user: "admin");
Logger.Log("Disk full",        details: ex.Message,          type: LogType.Error);
```

## Schema

The logger auto-creates its table via EF Core migrations. Columns:

| Column    | Type     | Notes                        |
|-----------|----------|------------------------------|
| Id        | int PK   | Auto-increment               |
| DateTime  | DateTime | UTC timestamp                |
| LogType   | int      | 0 Error / 1 Info / 2 Warning |
| Message   | string   | Short description            |
| Details   | string   | Stack trace or extra context |
| User      | string   | Defaults to "System"         |

## Dependencies

- `Microsoft.EntityFrameworkCore` 8.0
- `Microsoft.EntityFrameworkCore.SqlServer` 8.0

## License

MIT © [SplatDev](https://github.com/SplatDev-Ltda)
