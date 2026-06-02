# SplatDev.Logger

Simple database-backed logger using EF Core. Stores log entries in SQL Server with structured log types.

[![NuGet](https://img.shields.io/nuget/v/SplatDev.Logger)](https://www.nuget.org/packages/SplatDev.Logger)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

## Compatibility

| .NET Target | Package Version |
|---|---|
| net8.0 | 1.0.x |
| net10.0 | 1.0.x |

## Features

- Database-persisted logging via EF Core
- Structured `LogType` enum for categorization
- `Log` entity for storing log entries
- `Logger` service for writing entries
- Helper utilities for log formatting

## Installation

```bash
dotnet add package SplatDev.Logger
```

## Dependencies

- Microsoft.EntityFrameworkCore 8.0.13
- Microsoft.EntityFrameworkCore.SqlServer 8.0.13

## License

MIT © [SplatDev](https://github.com/SplatDev-Ltda)
