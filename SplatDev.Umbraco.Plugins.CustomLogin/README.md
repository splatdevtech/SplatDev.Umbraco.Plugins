# SplatDev.Umbraco.Plugins.CustomLogin

[![NuGet](https://img.shields.io/nuget/v/SplatDev.Umbraco.Plugins.CustomLogin)](https://www.nuget.org/packages/SplatDev.Umbraco.Plugins.CustomLogin)

Complete Umbraco login page customization plugin. Configure branding, colors, layout, typography, greetings, CSS custom properties, timeout screens, and security settings — all from the backoffice dashboard.

## Compatibility

| Umbraco | .NET | Status |
|---------|------|--------|
| 13.x | net8.0 | Supported |
| 17.x | net10.0 | Supported |

## Features

### Branding
- Custom brand name, logo, alternative logo, and background image
- Support email displayed on member login pages

### Colors (CSS Custom Properties)
| Setting | CSS Property | Default |
|---------|-------------|---------|
| Background Color | `--umb-login-background` | `#f4f4f4` |
| Primary Color | `--umb-login-primary-color` | `#283a97` |
| Text Color | `--umb-login-text-color` | `#000` |
| Curves Color | `--umb-login-curves-color` | `#f5c1bc` |

### Layout
| Setting | CSS Property | Default |
|---------|-------------|---------|
| Show Image Panel | `--umb-login-image-display` | `flex` |
| Image Border Radius | `--umb-login-image-border-radius` | `38px` |
| Content Background | `--umb-login-content-background` | `none` |
| Content Width | `--umb-login-content-width` | `100%` |
| Content Height | `--umb-login-content-height` | `100%` |
| Content Border Radius | `--umb-login-content-border-radius` | `0` |
| Align Items | `--umb-login-align-items` | `unset` |
| Show Curves | `--umb-login-curves-display` | `inline` |

### Typography
| Setting | CSS Property | Default |
|---------|-------------|---------|
| Header Font Size | `--umb-login-header-font-size` | `3rem` |
| Header Font Size (Large) | `--umb-login-header-font-size-large` | `4rem` |
| Secondary Header Font Size | `--umb-login-header-secondary-font-size` | `2.4rem` |

### Buttons
| Setting | CSS Property | Default |
|---------|-------------|---------|
| Button Border Radius | `--umb-login-button-border-radius` | `45px` |

### Greetings
- Custom greeting text per day of week (Sunday–Saturday)
- English and Spanish support
- Overrides Umbraco's default greetings on the login screen

### Session Timeout Screen
- Custom background image for the timeout/session-expired view
- Custom instruction text (English and Spanish)

### Security
- Toggle password reset link visibility (maps to `Umbraco:CMS:Security:AllowPasswordReset`)
- SSO hook toggle for member login pages

### Custom CSS
- Raw CSS override textarea for advanced customization
- Appended after generated styles

## How It Works

### CSS Injection (Umbraco 14+)
The plugin registers an `appEntryPoint` extension with `allowPublicAccess: true` in `umbraco-package.json`. A JavaScript entry point dynamically injects a `<link>` tag pointing to the generated CSS endpoint. All CSS custom properties are served from `/App_Plugins/CustomLogin/generated/login-overrides.css` via middleware.

### CSS Injection (Umbraco 13)
The `package.manifest` includes the generated CSS URL in its `css` array, which Umbraco loads on all backoffice pages including the login screen.

### Localization
Custom greetings are served as ES module localization files at `/App_Plugins/CustomLogin/generated/en.js` and `/App_Plugins/CustomLogin/generated/es.js`, registered as `localization` extensions in `umbraco-package.json`.

### Appsettings Override
The plugin uses `IPostConfigureOptions<ContentSettings>` and `IPostConfigureOptions<SecuritySettings>` to programmatically override `LoginLogoImage`, `LoginLogoImageAlternative`, `LoginBackgroundImage`, and `AllowPasswordReset`. These take effect on application startup.

### Settings Persistence
Settings are stored as JSON at `{ContentRoot}/umbraco/Data/CustomLogin/settings.json` and cached in memory for fast access.

## Installation

```bash
dotnet add package SplatDev.Umbraco.Plugins.CustomLogin
```

## Configuration

Navigate to **Settings > Custom Login** in the Umbraco backoffice to configure all options via the dashboard.

## API Endpoints

| Method | Route | Description |
|--------|-------|-------------|
| GET | `/umbraco/api/customlogin/GetSettings` | Get current login settings |
| POST | `/umbraco/api/customlogin/SaveSettings` | Update login settings |
| GET | `/umbraco/api/customlogin/PreviewCss` | Preview generated CSS |
| GET | `/umbraco/api/customlogin/PreviewLocalization?culture=en` | Preview generated localization JS |
| POST | `/umbraco/api/customlogin/Login` | Authenticate a member |
| GET | `/umbraco/api/customlogin/ValidateMember?username=...` | Check if member exists and is not locked |

## Dependencies

- Umbraco.Cms.Core
- Umbraco.Cms.Web.Common

## License

MIT
