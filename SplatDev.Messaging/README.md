# SplatDev.Messaging

Generic messaging abstractions and helpers for .NET applications. Defines provider-agnostic interfaces for email, SMS, and bulk messaging, plus canned message template support.

[![NuGet](https://img.shields.io/nuget/v/SplatDev.Messaging)](https://www.nuget.org/packages/SplatDev.Messaging)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

## Compatibility

| .NET Target | Package Version |
|---|---|
| net8.0 | 1.0.x |
| net10.0 | 1.0.x |

## Features

- `IMessagingController` — Single-message sending abstraction
- `IBulkMessagingController` — Bulk/batch message sending
- `IAddress` / `IBulkAddress` — Recipient addressing contracts
- `ICannedMessageTemplate` — Reusable message templates with placeholder substitution
- `CannedMessageHelpers` — Template rendering with placeholder replacement
- Provider-agnostic design: implement with SendGrid, SMTP, Twilio, etc.

## Installation

```bash
dotnet add package SplatDev.Messaging
```

## Provider Implementations

| Package | Provider |
|---------|----------|
| `SplatDev.Messaging.SendGrid` | SendGrid email |
| `SplatDev.Messaging.Smtp` | SMTP email |
| `SplatDev.Messaging.Mailgun` | Mailgun email |
| `SplatDev.Messaging.SocketLabs` | SocketLabs email |
| `SplatDev.Messaging.Twilio` | Twilio SMS |
| `SplatDev.Messaging.SMSTools` | SMSTools SMS |
| `SplatDev.Messaging.Newsletter` | Newsletter/bulk email |

## License

MIT © [SplatDev](https://github.com/SplatDev-Ltda)
