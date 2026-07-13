# SplatDev.Security

Security utilities for IP validation, phishing detection, and Safe Browsing lookups.
Targets `net8.0` and `net10.0`.

## Installation

```bash
dotnet add package SplatDev.Security
```

## Dependencies

- `Microsoft.EntityFrameworkCore` + `SqlServer`
- `Google.Apis.Safebrowsing.v4` — Google Safe Browsing API client

## Features

- **IP blacklist/whitelist** — EF Core entities for blocking/allowing IPs
- **IP history tracking** — log and query past visitor IPs
- **IPQualityScore integration** — fraud/proxy/VPN detection via IPQS API
- **CheckPhish** — phishing URL detection integration
- **Safe Browsing** — Google Safe Browsing API v4 lookup
- **Constants** — centralized API keys and endpoint URLs

## Usage

### IP blacklist check

```csharp
using SplatDev.Security;

bool isBlocked = await IpBlacklist.IsBlockedAsync(ip);
bool isAllowed = await IpWhitelist.IsAllowedAsync(ip);
```

### IP Quality Score

```csharp
var score = await IpQualityScore.CheckAsync(ip, apiKey);
if (score.IsProxy || score.IsVpn)
    return Forbid();
```

### Phishing check

```csharp
var result = await CheckPhish.ScanAsync(url, apiKey);
if (result.IsPhishing)
    return RedirectToAction("Blocked");
```

### Google Safe Browsing

```csharp
var threats = await SafeBrowsing.LookupAsync(url, apiKey);
if (threats.Any())
    return Content("This URL has been flagged.");
```

## Configuration

Configure API keys via environment variables or `appsettings.json`:

```json
{
  "Security": {
    "IpQualityScoreApiKey": "",
    "CheckPhishApiKey": "",
    "SafeBrowsingApiKey": ""
  }
}
```

Default API endpoints and keys are defined in `Constants.cs`.

## Models

| Entity | Purpose |
|---|---|
| `IpBlacklist` | EF Core entity — blocked IPs |
| `IpWhitelist` | EF Core entity — always-allowed IPs |
| `IpHistory` | EF Core entity — visitor IP log |
| `IpQualityScoreResponse` | IPQS API response model |
| `CheckPhishResponse` | CheckPhish API response model |

## License

MIT
