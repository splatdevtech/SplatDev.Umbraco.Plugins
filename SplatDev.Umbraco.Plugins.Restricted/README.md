# UmbracoCms.Plugins.Restricted

Content restriction plugin for Umbraco 13 (net8.0) and Umbraco 17 (net10.0).

## Features

- Member-only content gates using Umbraco's built-in `IPublicAccessService`
- Role-based access: restrict nodes to specific member groups
- Paywall support: redirect unauthorized visitors to a login or error page
- Backoffice dashboard to manage restricted nodes
- View component for rendering access status in Razor views

## No EF Core Required

This plugin uses Umbraco's own public access infrastructure — no additional database tables needed.

## API Endpoints

- `GET /umbraco/api/restricted/GetRestrictedNodes`
- `POST /umbraco/api/restricted/RestrictNode`
- `DELETE /umbraco/api/restricted/UnrestrictNode?nodeId={id}`
- `GET /umbraco/api/restricted/GetRequiredGroups?nodeId={id}`
- `POST /umbraco/api/restricted/SetRequiredGroups`

## Usage in Razor

```cshtml
@await Component.InvokeAsync("Restricted", new { nodeId = Model.Id })
```
