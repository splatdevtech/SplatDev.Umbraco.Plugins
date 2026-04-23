# UmbracoCms.Plugins.VisitorCounter

Site visitor counter plugin for Umbraco 13 (net8.0) and Umbraco 17 (net10.0).

## Features
- Cookie-based session tracking (IP-hash fallback, privacy-preserving)
- Unique vs. total visit counts
- Daily aggregated visitor counts table
- View component for front-end display (odometer-style counter widget)
- Umbraco backoffice dashboard (Angular for U13, Lit 3 with uui-table for U17)

## Quick Start

### 1. Register the plugin
The `VisitorCounterComposer` is auto-discovered. It registers the DbContext, service, and middleware.

### 2. Run EF Core migrations
```bash
dotnet ef migrations add InitialCreate --context VisitorCounterDbContext
dotnet ef database update --context VisitorCounterDbContext
```

### 3. Use the view component in a Razor template
```cshtml
@* Show total visits + unique visitors for the last 30 days *@
@await Component.InvokeAsync("VisitorCounter", new { days = 30 })
```

## API Endpoints

| Method | Route | Description |
|--------|-------|-------------|
| GET | `/umbraco/api/visitorcounter/GetStats?days=30` | Total + unique visit counts |
| GET | `/umbraco/api/visitorcounter/GetDailyCounts?days=30` | Per-day visit breakdown |

## Session Tracking

Visitors are tracked by a 30-day `_vcid` cookie (HttpOnly, SameSite=Lax).
No raw IP addresses are stored in the database.

## Build the backoffice client (U17)
```bash
cd client
npm install
npm run build
```
