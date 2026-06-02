# SplatDev.Messaging.Twilio

Twilio SMS provider for the `SplatDev.Messaging` abstraction layer. Send SMS messages through the Twilio API.

[![NuGet](https://img.shields.io/nuget/v/SplatDev.Messaging.Twilio)](https://www.nuget.org/packages/SplatDev.Messaging.Twilio)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

## Compatibility

| .NET Target | Package Version |
|---|---|
| net8.0 | 1.0.x |
| net10.0 | 1.0.x |

## Features

- `TwilioSmsController` — Implements `IMessagingController` for Twilio SMS
- `Sms` — SMS message model

## Installation

```bash
dotnet add package SplatDev.Messaging.Twilio
```

## Dependencies

- Twilio 7.4.1
- SplatDev.Messaging (project reference)

## See Also

- [SplatDev.Messaging](../SplatDev.Messaging/) — Base messaging abstractions

## License

MIT © [SplatDev](https://github.com/SplatDev-Ltda)
