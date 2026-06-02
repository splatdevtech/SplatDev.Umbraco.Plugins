# Smtp

SMTP email configuration UI for Umbraco backoffice — test and configure SMTP settings. Supports Umbraco 13 (net8.0) and Umbraco 17 (net10.0).

[![NuGet](https://img.shields.io/nuget/v/SplatDev.Umbraco.Plugins.Smtp.svg)](https://www.nuget.org/packages/SplatDev.Umbraco.Plugins.Smtp)

## Compatibility

| Umbraco | .NET | Package Version |
|---------|------|-----------------|
| 13.x    | 8.0  | 2.0.0           |
| 17.x    | 10.0 | 2.0.0           |

## Installation

```sh
dotnet add package SplatDev.Umbraco.Plugins.Smtp
```

## Quick Start

Register in `Program.cs`:

```csharp
builder.CreateUmbracoBuilder()
    .AddBackOffice()
    .AddWebsite()
    .AddSmtp()   // <-- add this
    .Build();
```

## Configuration

Add to `appsettings.json`:

```json
{
  "Smtp": {
    "Host": "smtp.example.com",
    "Port": 587,
    "Username": "",
    "Password": "",
    "EnableSsl": true,
    "FromAddress": "noreply@example.com"
  }
}
```


## Dashboard

![SMTP Settings dashboard](../docs/screenshots/smtp-settings.png)
## License

MIT © [SplatDev](https://github.com/SplatDev-Ltda)
