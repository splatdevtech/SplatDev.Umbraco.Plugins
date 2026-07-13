# SplatDev.GeoLocation

IP-based geolocation utilities for .NET applications.
Targets `net8.0` and `net10.0`.

## Installation

```bash
dotnet add package SplatDev.GeoLocation
```

## Features

- IP-to-location lookup via external geolocation APIs
- Country, region, city, postal code, and coordinate resolution
- Timezone offset detection from coordinates
- Caching support for repeated lookups

## Usage

```csharp
using SplatDev.GeoLocation;

var location = await GeoLocationService.LookupAsync(clientIp);
Console.WriteLine($"{location.City}, {location.CountryCode}");
```

## License

MIT
