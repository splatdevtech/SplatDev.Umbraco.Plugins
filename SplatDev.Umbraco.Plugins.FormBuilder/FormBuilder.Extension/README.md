# SplatDev FormBuilder

A zero-license Umbraco Forms alternative for Umbraco 13 (net8.0) and 17 (net10.0). Provides form creation, field management, submission handling, and email workflows.

## Architecture

| Layer | Project | Purpose |
|-------|---------|---------|
| Extension | `FormBuilder.Extension` | Distributable NuGet package: NPoco entities, CRUD API, workflows, email |
| Harness | `FormBuilder` | Dev Umbraco host for local testing |
| Tests | `FormBuilder.Extension.Tests` | xUnit + Moq controller tests |

## Tech Stack

- **Persistence:** NPoco via `IScopeProvider`
- **API:** `UmbracoApiController` (dual-compatible v13/v17)
- **Migration:** FluentMigrator via `Upgrader` with `IComponent`
- **v17 Backoffice:** Lit 3 + Vite + `@umbraco-cms/backoffice`
- **Email:** SMTP via `IOptions<EmailServiceOptions>` (configurable)

## Quick Start

1. Reference `FormBuilder.Extension` in your Umbraco host.
2. Configure SMTP in `appsettings.json`:

```json
{
  "FormBuilder": {
    "Email": {
      "SmtpHost": "smtp.example.com",
      "SmtpPort": 587,
      "SmtpUsername": "user",
      "SmtpPassword": "pass",
      "EnableSsl": true,
      "DefaultFromAddress": "noreply@example.com",
      "DefaultFromName": "FormBuilder"
    }
  }
}
```

3. Build and run. Migration creates 6 tables on startup.
4. Build the Lit dashboards: `cd Client && npm install && npm run build`
5. Navigate to the **Form Builder** section in the Umbraco backoffice.

## REST API

All endpoints under `/umbraco/api/formbuilder/`:

### Forms

| Method | Path | Description |
|--------|------|-------------|
| GET | `/GetAllForms` | List all forms with field count |
| GET | `/GetForm?id={id}` | Get form with fields and workflows |
| POST | `/CreateForm` | Create form with fields |
| PUT | `/UpdateForm?id={id}` | Update form + replace fields |
| DELETE | `/DeleteForm?id={id}` | Delete form (fails if submissions exist) |
| POST | `/DuplicateForm?id={id}` | Clone form as "(copy)" |
| GET | `/GetFieldTypes` | List available field types |

### Public Submission

| Method | Path | Description |
|--------|------|-------------|
| POST | `/umbraco/api/formbuilder/submit` | Submit form (CSRF-protected) |

## Database Tables

| Table | Purpose |
|-------|---------|
| `FormBuilderForms` | Form definitions |
| `FormBuilderFormFields` | Fields per form (alias, type, validation) |
| `FormBuilderDropdownValues` | Dropdown/checkbox/radio values |
| `FormBuilderEmailTemplates` | Email template storage |
| `FormBuilderStatus` | Status tracking |
| `FormBuilderWorkflows` | Workflow associations |

## Backoffice Dashboards

### Form List (`#/formbuilder/formbuilder-list`)
- Table of all forms with CRUD actions
- New Form, Duplicate, Delete buttons
- Click Edit to open the form editor

### Form Editor (`#/formbuilder/formbuilder-editor`)
- Edit form name and category
- Add/remove/edit fields with type picker (12 field types)
- Validation settings (required, regex, min length, placeholder)
- Save to persist changes

### Welcome (`#/formbuilder/formbuilder-dashboard`)
- Quick-start landing page

## Internationalization

English and Spanish supported across all dashboards. Language files under `lang/`.

## Extension Points

- `IFormRepository` — swap persistence layer
- `IEmailService` — swap email provider (SMTP, Mailgun, SendGrid)
- `IFormSubmissionValidator` — custom validation logic
- `WorkflowSchema` — extend workflow engine

## Testing

```bash
# Build
dotnet build

# Unit tests
dotnet test FormBuilder.Extension.Tests/

# E2E (requires running Umbraco host)
dotnet test FormBuilder.Tests/
```

## Configuration

| Key | Default | Description |
|-----|---------|-------------|
| `FormBuilder:Email:SmtpHost` | localhost | SMTP server |
| `FormBuilder:Email:SmtpPort` | 25 | SMTP port |
| `FormBuilder:Email:EnableSsl` | false | TLS |
| `FormBuilder:Email:DefaultFromAddress` | noreply@example.com | Sender email |
| `FormBuilder:Email:DefaultFromName` | FormBuilder | Sender display name |

## License

MIT. Copyright SplatDev Ltda.
