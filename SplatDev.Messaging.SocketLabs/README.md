# SplatDev.Messaging.SocketLabs

SocketLabs email provider for the `SplatDev.Messaging` abstraction layer. Send single and bulk emails through the SocketLabs Email Delivery API.

[![NuGet](https://img.shields.io/nuget/v/SplatDev.Messaging.SocketLabs)](https://www.nuget.org/packages/SplatDev.Messaging.SocketLabs)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

## Compatibility

| .NET Target | Package Version |
|---|---|
| net8.0 | 1.0.x |
| net10.0 | 1.0.x |

## Features

- `SocketLabsController` — Single-message sending via `IMessagingController`
- `SocketLabsBulkController` — Bulk message sending via `IBulkMessagingController`
- `BulkAddress` / `BulkMessageData` — Bulk recipient and message models

## Installation

```bash
dotnet add package SplatDev.Messaging.SocketLabs
```

## Dependencies

- SocketLabs.EmailDelivery 1.4.2
- SplatDev.Messaging (project reference)

## See Also

- [SplatDev.Messaging](../SplatDev.Messaging/) — Base messaging abstractions

## License

MIT © [SplatDev](https://github.com/SplatDev-Ltda)
