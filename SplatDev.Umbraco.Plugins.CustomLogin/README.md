# UmbracoCms.Plugins.CustomLogin

Fully customizable Umbraco login page with branding support, SSO integration hooks, and 2FA readiness.

## Targets

- **Umbraco 13** (net8.0)
- **Umbraco 17** (net10.0)

## Features

- Branded login page: logo, colors, custom support email
- SSO hook endpoint ready for integration
- Member validation via Umbraco's IMemberService
- Backoffice settings dashboard (AngularJS for U13, Lit 3 Web Component for U17)

## Settings

| Property | Description |
|----------|-------------|
| BrandName | Company/app name shown on login page |
| LogoUrl | URL to the brand logo image |
| BackgroundColor | Page background color (hex) |
| AccentColor | Button and link accent color (hex) |
| SupportEmail | Support contact email |
| EnableSso | Toggle SSO redirect button |

## API Endpoints

| Method | Route | Description |
|--------|-------|-------------|
| GET | `/umbraco/api/customlogin/GetSettings` | Get current login settings |
| POST | `/umbraco/api/customlogin/SaveSettings` | Update login settings |
| POST | `/umbraco/api/customlogin/Login` | Authenticate a member |
| GET | `/umbraco/api/customlogin/ValidateMember?username=...` | Check if member exists and is not locked |

## Client Build

```bash
cd client
npm install
npm run build
```
