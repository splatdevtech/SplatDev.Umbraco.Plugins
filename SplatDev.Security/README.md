# SplatDev.Security

General-purpose security utilities, external threat-intelligence API integrations, and IP-based access-control entity models. This package provides the building blocks for phishing detection, safe-browsing lookups, URL reputation scoring, and IP allow/block lists — it does not enforce any policy on its own. Reference it from your application or Umbraco plugin and wire the tools into your own middleware or background jobs.

## Install

```bash
dotnet add package SplatDev.Security
```

Multi-targets `net8.0` and `net10.0`. Published to nuget.org.

## What's implemented

### Threat-intelligence APIs

All three APIs are called through static methods on `SplatDev.Security.Tools`. API keys are passed per-call as method parameters.

| Method | Service | Returns |
|--------|---------|---------|
| `CheckPhish` | CheckPhish.ai | Submits a URL for phishing scan and polls for the result |
| `CheckPhishPendingJob` | CheckPhish.ai | Polls a previously submitted scan job by ID |
| `GoogleSafeBrowing` | Google Safe Browsing v4 | Threat-match lookup across malware, social engineering, unwanted software, and potentially harmful applications |
| `IpQualityScore` | IPQualityScore | URL reputation score with risk score, malware/phishing/spam flags, and domain-age metadata |

```csharp
using SplatDev.Security;

var result = await Tools.CheckPhish(apiKey: "cp_xxx", url: "https://example.com", insights: true);
if (result.disposition == "phishing")
    // block or flag the URL

var threats = await Tools.GoogleSafeBrowing(
    apiKey: "AIza...",
    urls: ["https://example.com", "https://example.org"]);
```

### Authentication helpers

```csharp
var encoded = Tools.EncodeAuthHeader("user", "pass");   // "dXNlcjpwYXNz"
var (username, password) = Tools.DecodeAuthHeader(encoded);
```

### Password utilities

```csharp
var password = await Tools.GeneratePasswordAsync();            // 10-char random
var hash = await Tools.EncrypPasswordAsync(password, "salt");  // SHA-256 hash
```

### IP access-control entity models

Three EF Core entities (in `SplatDev.Security.Models`) for building allow/block-list features:

| Entity | Table | Purpose |
|--------|-------|---------|
| `IpWhitelist` | `IpWhitelist` | Known-good IPs — `Id`, `Ip`, `AddedBy`, `AddedOn` |
| `IpBlacklist` | `IpBlacklist` | Banned IPs with time-bounded release — `Id`, `Ip`, `BannedOn`, `BannedBy`, `ReleaseOn` |
| `IpHistory` | `IpHistory` | Event log — `Id`, `Ip`, `Url`, `Event` (`Error` or `LoginFailure`), `OccurredOn`, `Exception` |

These are plain `[Table]`-annotated POCOs. Register them in your own `DbContext`:

```csharp
public DbSet<IpWhitelist> IpWhitelists { get; set; }
public DbSet<IpBlacklist> IpBlacklists { get; set; }
public DbSet<IpHistory> IpHistories { get; set; }
```

### Service base URLs

Constants are available on `SplatDev.Security.Constants`:

```csharp
Constants.CHECK_PHISH_URL      // "https://developers.checkphish.ai/"
Constants.GOOGLE_SAFE_BROWSING  // "https://safebrowsing.googleapis.com/"
Constants.IP_QUALITY_SCORE      // "https://ipqualityscore.com/api/"
```

## DI Registration

No DI registration needed. All methods on `Tools` are static. EF entities are plain models — add them to your own `DbContext`.

## Dependencies

| Package | Purpose |
|---------|---------|
| `Microsoft.EntityFrameworkCore` (8.0.13) | Entity annotations for IP models |
| `Microsoft.EntityFrameworkCore.SqlServer` (8.0.13) | SQL Server provider |
| `Google.Apis.Safebrowsing.v4` (1.68.0) | Google Safe Browsing API client |
| `RestSharp` (112.1.0) | HTTP client for CheckPhish + IPQualityScore |
| `Newtonsoft.Json` (13.0.3) | JSON serialization |

---

**SplatDev.Security** — part of the [SplatDev.Umbraco.Plugins](https://github.com/SplatDev-Ltda/SplatDev.Umbraco.Plugins) suite. Licensed under MIT. © SplatDev Ltda.
