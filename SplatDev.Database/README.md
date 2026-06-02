# SplatDev.Database

Generic database service abstractions built on [NPoco](https://github.com/schotime/NPoco) for .NET 8 / .NET 10 applications.

## What it provides

- `IDatabaseService` — repository-pattern interface for CRUD operations
- `DatabaseService` — NPoco-backed implementation with connection pooling
- Reflection helpers for mapping CLR types to database columns automatically

## Target frameworks

| Framework | Typical host |
|-----------|-------------|
| net8.0    | Umbraco 13, .NET 8 apps |
| net10.0   | Umbraco 17, .NET 10 apps |

## Installation

```sh
dotnet add package SplatDev.Database
```

## Quick Start

Register in `Program.cs` / `Startup.cs`:

```csharp
builder.Services.AddSplatDevDatabase(builder.Configuration.GetConnectionString("umbracoDbDSN"));
```

Then inject into your services:

```csharp
public class MyService(IDatabaseService db)
{
    public Task<IEnumerable<MyRecord>> GetAllAsync() =>
        db.FetchAsync<MyRecord>("SELECT * FROM MyTable");
}
```

## Dependencies

- [NPoco](https://www.nuget.org/packages/NPoco) 5.3.0
- `Microsoft.Extensions.DependencyInjection.Abstractions`
- `SplatDev.Reflection.Helpers` (internal)

## License

MIT © [SplatDev](https://github.com/SplatDev-Ltda)
