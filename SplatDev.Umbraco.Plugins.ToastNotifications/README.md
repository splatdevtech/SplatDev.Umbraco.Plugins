# ToastNotifications

Toast/snackbar notification system for Umbraco — configurable messages shown to site visitors, stored in database. Supports Umbraco 13 (net8.0) and Umbraco 17 (net10.0).

[![NuGet](https://img.shields.io/nuget/v/SplatDev.Umbraco.Plugins.ToastNotifications.svg)](https://www.nuget.org/packages/SplatDev.Umbraco.Plugins.ToastNotifications)

## Compatibility

| Umbraco | .NET | Package Version |
|---------|------|-----------------|
| 13.x    | 8.0  | 2.0.0           |
| 17.x    | 10.0 | 2.0.0           |

## Installation

```sh
dotnet add package SplatDev.Umbraco.Plugins.ToastNotifications
```

## Quick Start

Register in `Program.cs`:

```csharp
builder.CreateUmbracoBuilder()
    .AddBackOffice()
    .AddWebsite()
    .AddToastNotifications()   // <-- add this
    .Build();
```


## Dashboard

![Toast Notifications dashboard](../docs/screenshots/toast-notifications.png)
## License

MIT © [SplatDev](https://github.com/SplatDev-Ltda)
