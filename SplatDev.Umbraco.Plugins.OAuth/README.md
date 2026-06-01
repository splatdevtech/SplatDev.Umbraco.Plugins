# OAuth

Umbraco OAuth social login plugin supporting Umbraco 13 (net8.0) and Umbraco 17 (net10.0).

[![NuGet](https://img.shields.io/nuget/v/SplatDev.Umbraco.Plugins.OAuth.svg)](https://www.nuget.org/packages/SplatDev.Umbraco.Plugins.OAuth)

## Compatibility

| Umbraco | .NET | Package Version |
|---------|------|-----------------|
| 13.x    | 8.0  | 2.0.0           |
| 17.x    | 10.0 | 2.0.0           |

## Installation

```sh
dotnet add package SplatDev.Umbraco.Plugins.OAuth
```

## Quick Start

Register in `Program.cs`:

```csharp
builder.CreateUmbracoBuilder()
    .AddBackOffice()
    .AddWebsite()
    .AddOAuth()   // <-- add this
    .Build();
```

## Configuration

Add to `appsettings.json`:

```json
{
  "OAuth": {
    "Providers": {
      "Google": {
        "ClientId": "",
        "ClientSecret": ""
      }
    }
  }
}
```

## License

MIT © [SplatDev](https://github.com/SplatDev-Ltda)
