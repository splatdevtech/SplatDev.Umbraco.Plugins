# SplatDev.Messaging.SendGrid

SendGrid email provider for the `SplatDev.Messaging` abstraction layer. Send transactional and marketing emails through the SendGrid API.

[![NuGet](https://img.shields.io/nuget/v/SplatDev.Messaging.SendGrid)](https://www.nuget.org/packages/SplatDev.Messaging.SendGrid)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

## Compatibility

| .NET Target | Package Version |
|---|---|
| net8.0 | 1.0.x |
| net10.0 | 1.0.x |

## Features

- `SendGridController` — Implements `IMessagingController` for SendGrid
- Supports transactional email sending via the SendGrid v3 API

## Installation

```bash
dotnet add package SplatDev.Messaging.SendGrid
```

## Dependencies

- SendGrid 9.29.3
- SplatDev.Messaging (project reference)

## See Also

- [SplatDev.Messaging](../SplatDev.Messaging/) — Base messaging abstractions

## License

MIT © [SplatDev](https://github.com/SplatDev-Ltda)
