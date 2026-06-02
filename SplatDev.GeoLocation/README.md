# SplatDev.GeoLocation

IP-based geolocation utilities for .NET applications. Look up geographic information (country, city, region, coordinates) from IP addresses.

[![NuGet](https://img.shields.io/nuget/v/SplatDev.GeoLocation)](https://www.nuget.org/packages/SplatDev.GeoLocation)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

## Compatibility

| .NET Target | Package Version |
|---|---|
| net8.0 | 1.0.x |
| net10.0 | 1.0.x |

## Features

- IP address to geographic location lookup via `GeoLocator`
- Returns structured `GeoLocationResult` with country, region, city, and coordinates
- REST API-based lookup using RestSharp

## Installation

```bash
dotnet add package SplatDev.GeoLocation
```

## Usage

```csharp
var result = await GeoLocator.LocateAsync("8.8.8.8");
// result.Country, result.City, result.Latitude, etc.
```

## Dependencies

- RestSharp 112.1.0

## License

MIT © [SplatDev](https://github.com/SplatDev-Ltda)
