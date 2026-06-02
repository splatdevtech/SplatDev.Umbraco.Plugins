# SplatDev.Security

Security utilities for .NET applications including IP validation, phishing detection, and safe browsing checks.

[![NuGet](https://img.shields.io/nuget/v/SplatDev.Security)](https://www.nuget.org/packages/SplatDev.Security)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

## Compatibility

| .NET Target | Package Version |
|---|---|
| net8.0 | 1.0.x |
| net10.0 | 1.0.x |

## Features

- **Google Safe Browsing** — Check URLs against the Google Safe Browsing API v4
- **CheckPhish** — Phishing URL detection via the CheckPhish API
- **IP Quality Score** — IP reputation and fraud scoring via IpQualityScore API
- **IP Blacklist/Whitelist** — Database-backed IP access control lists
- **IP History** — Track and audit IP address activity
- `Tools` — Security utility methods

## Installation

```bash
dotnet add package SplatDev.Security
```

## Dependencies

- Microsoft.EntityFrameworkCore 8.0.13
- Microsoft.EntityFrameworkCore.SqlServer 8.0.13
- Google.Apis.Safebrowsing.v4 1.68.0.2968
- RestSharp 112.1.0
- Newtonsoft.Json 13.0.3

## License

MIT © [SplatDev](https://github.com/SplatDev-Ltda)
