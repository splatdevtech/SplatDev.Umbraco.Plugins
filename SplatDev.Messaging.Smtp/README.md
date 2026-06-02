# SplatDev.Messaging.Smtp

SMTP email provider for the `SplatDev.Messaging` abstraction layer. Send emails through any standard SMTP server.

[![NuGet](https://img.shields.io/nuget/v/SplatDev.Messaging.Smtp)](https://www.nuget.org/packages/SplatDev.Messaging.Smtp)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

## Compatibility

| .NET Target | Package Version |
|---|---|
| net8.0 | 1.0.x |
| net10.0 | 1.0.x |

## Features

- `SmtpController` — Implements `IMessagingController` for SMTP
- `SmtpResult` / `SmtpResultEventArgs` — Delivery result models with event support
- Built on `System.Net.Mail.SmtpClient`

## Installation

```bash
dotnet add package SplatDev.Messaging.Smtp
```

## Dependencies

- Microsoft.AspNetCore.App (framework reference)
- SplatDev.Messaging (project reference)

## See Also

- [SplatDev.Messaging](../SplatDev.Messaging/) — Base messaging abstractions

## License

MIT © [SplatDev](https://github.com/SplatDev-Ltda)
