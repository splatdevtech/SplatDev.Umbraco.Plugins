# JsonRpc

JSON-RPC 2.0 API endpoint for Umbraco — expose content as JSON-RPC with API key management and request logging. Supports Umbraco 13 (net8.0) and Umbraco 17 (net10.0).

[![NuGet](https://img.shields.io/nuget/v/SplatDev.Umbraco.Plugins.JsonRpc.svg)](https://www.nuget.org/packages/SplatDev.Umbraco.Plugins.JsonRpc)

## Compatibility

| Umbraco | .NET | Package Version |
|---------|------|-----------------|
| 13.x    | 8.0  | 2.0.0           |
| 17.x    | 10.0 | 2.0.0           |

## Installation

```sh
dotnet add package SplatDev.Umbraco.Plugins.JsonRpc
```

## Quick Start

Register in `Program.cs`:

```csharp
builder.CreateUmbracoBuilder()
    .AddBackOffice()
    .AddWebsite()
    .AddJsonRpc()   // <-- add this
    .Build();
```

## Configuration

Add to `appsettings.json`:

```json
{
  "JsonRpc": {
    "ApiKeys": [ "" ],
    "EnableRequestLogging": true
  }
}
```

## License

MIT © [SplatDev](https://github.com/SplatDev-Ltda)
