# SplatDev.Mobile.Detection

Mobile device detection middleware for ASP.NET Core. Identifies mobile devices from HTTP request user-agent strings using a bundled device database.

[![NuGet](https://img.shields.io/nuget/v/SplatDev.Mobile.Detection)](https://www.nuget.org/packages/SplatDev.Mobile.Detection)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

## Compatibility

| .NET Target | Package Version |
|---|---|
| net8.0 | 1.0.x |
| net10.0 | 1.0.x |

## Features

- User-agent based mobile device detection via `MobileDetection`
- Bundled `device_list.json` database of known mobile devices
- `MobileDevice` model with device properties
- Works as middleware or standalone service

## Installation

```bash
dotnet add package SplatDev.Mobile.Detection
```

## Dependencies

- Microsoft.AspNetCore.App (framework reference)
- Newtonsoft.Json 13.0.3

## License

MIT © [SplatDev](https://github.com/SplatDev-Ltda)
