# EmailTemplates

Email template engine with variable substitution, preview, and singleton style settings for Umbraco 17 (net10.0). Includes Bellissima Lit dashboards for managing templates and style configuration in the backoffice.

[![NuGet](https://img.shields.io/nuget/v/SplatDev.Umbraco.Plugins.EmailTemplates.svg)](https://www.nuget.org/packages/SplatDev.Umbraco.Plugins.EmailTemplates)

## Compatibility

| Umbraco | .NET | Package Version |
|---------|------|-----------------|
| 17.x    | 10.0 | 1.0.0           |

## Installation

```sh
dotnet add package SplatDev.Umbraco.Plugins.EmailTemplates
```

## Quick Start

Register in `Program.cs`:

```csharp
builder.CreateUmbracoBuilder()
    .AddBackOffice()
    .AddWebsite()
    .AddEmailTemplates()   // <-- add this
    .Build();
```

## Backoffice UI

The plugin adds an **Email Templates** section to the Umbraco backoffice with two dashboard tabs:

| Tab | Description |
|-----|-------------|
| **Templates** | Create, edit, delete, preview, and manage email templates. Supports `{{variable}}` substitution with auto-extraction. |
| **Style Settings** | Configure global email styles: header/footer HTML, CSS, logo URL, primary color, and company name. Applied to all template previews. |

The dashboards are built with Lit (Bellissima) and ship as embedded resources via the NuGet package.

## API Endpoints

### Templates

| Method | Path | Description |
|--------|------|-------------|
| GET | `/umbraco/management/api/v1/email-templates` | List all templates |
| GET | `/umbraco/management/api/v1/email-templates/{id}` | Get template by ID |
| GET | `/umbraco/management/api/v1/email-templates/{id}/preview` | Render preview with optional `?variables=` |
| GET | `/umbraco/management/api/v1/email-templates/{id}/variables` | Extract `{{variables}}` from template |
| POST | `/umbraco/management/api/v1/email-templates` | Create template |
| PUT | `/umbraco/management/api/v1/email-templates/{id}` | Update template |
| DELETE | `/umbraco/management/api/v1/email-templates/{id}` | Delete template |

### Style Settings

| Method | Path | Description |
|--------|------|-------------|
| GET | `/umbraco/management/api/v1/email-style` | Get style settings (singleton) |
| PUT | `/umbraco/management/api/v1/email-style` | Save style settings |

## Variable Substitution

Templates use `{{variableName}}` syntax for substitution. Example:

```html
<p>Hello {{name}},</p>
<p>Welcome to {{company}}!</p>
```

Use the **Auto-extract** button in the dashboard to detect variables from the template body and subject.

## Configuration

Add to `appsettings.json`:

```json
{
  "EmailTemplates": {
    "DefaultFromAddress": "noreply@example.com",
    "DefaultFromName": "My Site"
  }
}
```

## Development

The Lit frontend lives in `client/` and uses Vite to build into `App_Plugins/SplatDev.EmailTemplates/dist/`:

```sh
cd client
npm install
npm run build
```

## License

MIT © [SplatDev](https://github.com/SplatDev-Ltda)
