# SplatDev FormBuilder — Design Spec

**Date:** 2026-05-28  
**Status:** Draft  
**Target:** Umbraco 13 (net8.0) + Umbraco 17 (net10.0)  
**Priority:** P0

---

## 1. Goals & Non-Goals

### Goals
- Replace Umbraco Forms with a zero-license, 100%-feature-parity alternative
- Support form creation, field management, submission handling, and email workflows
- Provide full backoffice UI for both Umbraco 13 (AngularJS) and 17 (Lit 3 / Bellissima)
- Deliver a public-facing form renderer (ViewComponent or macro) with CSRF protection
- Support multi-language (English + Spanish by default)

### Non-Goals (v1)
- Frontend drag-and-drop form builder on the public site
- Multi-step / wizard-style forms
- Payment gateway integration (use Payments plugin instead)
- Conditional field logic (show/hide based on values)

---

## 2. Comparison with Umbraco Forms

| Feature | Umbraco Forms | FormBuilder | Parity |
|---------|--------------|-------------|--------|
| Form CRUD | Yes | Yes | 100% |
| Field types (text, textarea, dropdown, checkbox, radio, date, file, recaptcha) | Yes | Yes | 100% |
| Pre-value sources (manual, SQL, member) | Yes | Yes | 100% |
| Validation (required, regex, email, min/max length) | Yes | Yes | 100% |
| Email workflows | Yes | Yes | 100% |
| Post-submission workflow engine | Yes | Yes | 100% |
| Submission storage + export | Yes | Yes | 100% |
| CSRF protection | Yes | Yes | 100% |
| Conditional fields | Yes | No | v2 |
| Multi-page forms | Yes | No | v2 |
| Payment workflows | Yes | No (delegated to Payments plugin) | N/A |

---

## 3. Architecture

### Solution Layout

```
SplatDev.Umbraco.Plugins.FormBuilder/
├── FormBuilder.sln
├── FormBuilder.Extension/          # Distributable NuGet package
│   ├── Composers/                  # DI registration, migration runner
│   ├── Controllers/                # Backoffice API + public submission
│   ├── Entities/                   # NPoco-mapped DB entities
│   ├── Enums/                      # WorkflowType, WorkflowExecutionStatus
│   ├── Extensions/                 # TemplateProcessor, helpers
│   ├── Interfaces/                 # IFormRepository, IWorkflow, etc.
│   ├── Migrations/                 # NPoco FluentMigrator auto-migration
│   ├── Models/                     # FormSubmissionModel, request/response DTOs
│   ├── Repositories/               # FormRepository (CRUD + submission storage)
│   ├── Services/                   # EmailService, config wiring
│   ├── Validators/                 # FormSubmissionValidator
│   ├── Workflows/                  # EmailWorkflow, SendNotificationWorkflow
│   └── Client/                     # Lit 3 (v17) backoffice dashboards (Vite)
│       ├── src/dashboards/
│       │   ├── form-list-dashboard.element.ts
│       │   ├── form-editor-dashboard.element.ts
│       │   ├── form-submissions-dashboard.element.ts
│       │   └── welcome-dashboard.element.ts
│       └── public/umbraco-package.json
├── FormBuilder/                    # Dev harness (Umbraco host)
└── FormBuilder.Tests/              # xUnit + Moq + Playwright
```

### Data Flow

```
Public site: ViewComponent → POST /umbraco/api/formbuilder/submit → Validator → Repository → Workflows
Backoffice:  Lit/AngularJS → GET/POST/PUT/DELETE /umbraco/api/formbuilder/* → Repository → NPoco DB

DB Tables:
- splatdev_formbuilder_forms
- splatdev_formbuilder_formfields
- splatdev_formbuilder_dropdownvalues
- splatdev_formbuilder_submissions
- splatdev_formbuilder_workflows
- splatdev_formbuilder_emailtemplates
```

### Tech Stack
- **Persistence:** NPoco via `IScopeProvider` (v13) / `IScopeProvider` (v17)
- **Backoffice API v13/v17:** `UmbracoApiController` (dual-compatible)
- **Migration:** FluentMigrator via `Upgrader` with `IComponent`
- **v17 Backoffice:** Lit 3 + Vite + `@umbraco-cms/backoffice`
- **v13 Backoffice:** AngularJS + `package.manifest`
- **Email:** `IEmailService` abstraction (SMTP, Mailgun, SendGrid) via `IHttpClientFactory`

---

## 4. API Surface

### Backoffice API (`/umbraco/api/formbuilder/`)

| Method | Path | Description |
|--------|------|-------------|
| GET | `/forms` | List all forms (paged) |
| GET | `/forms/{id}` | Get form with fields |
| POST | `/forms` | Create form |
| PUT | `/forms/{id}` | Update form + fields |
| DELETE | `/forms/{id}` | Delete form + cascade |
| POST | `/forms/{id}/duplicate` | Clone form |
| GET | `/forms/{id}/submissions` | Get submissions (paged) |
| GET | `/forms/{id}/submissions/export` | Export as CSV |
| DELETE | `/submissions/{id}` | Delete submission |
| GET | `/fieldtypes` | List available field types |
| GET | `/workflows` | List workflow types |
| POST | `/forms/{id}/workflows` | Add workflow to form |
| DELETE | `/forms/{id}/workflows/{workflowId}` | Remove workflow |
| GET | `/ping` | Health check (existing) |

### Public API

| Method | Path | Description |
|--------|------|-------------|
| POST | `/umbraco/api/formbuilder/submit` | Submit form (CSRF-protected) |

### Request/Response DTOs

```csharp
// CreateFormRequest
public record CreateFormRequest(string Name, List<FormFieldDto> Fields);
public record FormFieldDto(string Alias, string Label, string FieldType,
    bool Required, string? Placeholder, string? Regex, int? MinLength, int? MaxLength,
    List<DropdownValueDto>? DropdownValues, int SortOrder);
public record DropdownValueDto(string Value, string Label, int SortOrder);

// FormResponse
public record FormResponse(int Id, string Name, DateTime CreatedAt, DateTime UpdatedAt,
    List<FormFieldDto> Fields, List<WorkflowDto> Workflows);
public record WorkflowDto(int Id, string WorkflowType, string ConfigJson);

// SubmissionResponse
public record SubmissionResponse(int Id, int FormId, DateTime SubmittedAt,
    Dictionary<string, string> FieldValues);
```

---

## 5. Backoffice UI Spec

### v17 (Lit 3 / Bellissima) — Required Dashboards

#### Form List Dashboard (`/formbuilder`)
- Table of all forms with name, field count, submission count, created date
- "New Form" button → opens form editor
- Actions per row: Edit, Duplicate, Delete (with confirm)
- Empty state: "No forms yet. Create your first form."

#### Form Editor Dashboard (`/formbuilder/editor/{id}`)
- Form name input
- Field list (sortable via drag handles)
- "Add Field" button opens field type picker
- Field editor panel (label, alias, placeholder, required toggle, validation rules)
- Per-field-type options (dropdown values for dropdown/checkbox/radio)
- Workflow configuration section (add/remove email/Slack/webhook workflows)
- "Save" + "Save & Close" buttons
- Preview toggle (renders form in preview mode)

#### Submissions Dashboard (`/formbuilder/submissions/{formId}`)
- Table of submissions with date + field values
- Filter by date range
- Export as CSV button
- Delete individual or bulk delete

### v13 (AngularJS) — Required Directives
- Same functionality as v17 dashboards, implemented with AngularJS 1.x
- `package.manifest` referencing `formbuilder.controller.js` + `formbuilder.directive.js`
- Section + tree nodes for Forms, Submissions, Settings

---

## 6. Configuration

```json
// appsettings.json
{
  "FormBuilder": {
    "Email": {
      "Provider": "Smtp",          // Smtp | Mailgun | SendGrid
      "Smtp": {
        "Host": "smtp.example.com",
        "Port": 587,
        "Username": "user",
        "Password": "pass",
        "EnableSsl": true,
        "FromAddress": "noreply@example.com",
        "FromName": "FormBuilder"
      },
      "Mailgun": {
        "ApiKey": "key-xxx",
        "Domain": "mg.example.com"
      }
    },
    "Recaptcha": {
      "SiteKey": "",
      "SecretKey": ""
    },
    "MaxFileUploadSizeMb": 10
  }
}
```

---

## 7. Testing Strategy

### Unit Tests (xUnit + Moq)
- **Controller tests:** CRUD endpoints, validation, error paths — 20+ tests
- **Validator tests:** Required fields, regex, length, email format — 10+ tests
- **Workflow tests:** EmailWorkflow, SendNotificationWorkflow execution — 5+ tests
- **Repository tests:** NPoco query correctness (integration with InMemory DB or LocalDB) — 5+ tests

### E2E Tests (Playwright)
- Full form creation flow (backoffice)
- Form submission flow (public site)
- Submission export
- Form duplication

### Target Coverage
- Controller: 90%+
- Validator: 95%+
- Workflows: 80%+
- Repository: 70%+

---

## 8. Deliverables Checklist

### Spec & Docs
- [ ] Spec document (this file)
- [ ] Plan document (`docs/plans/2026-05-28-formbuilder-plan.md`)
- [ ] README.md with architecture, setup, API reference
- [ ] Integration guide

### Backend
- [ ] Full CRUD API controller (form/field/workflow)
- [ ] Configurable EmailService (IOptions<T>)
- [ ] Submission export (CSV)
- [ ] Form duplication
- [ ] Recaptcha field type support

### Backoffice UI (v17)
- [ ] Form List dashboard (Lit 3)
- [ ] Form Editor dashboard (Lit 3)
- [ ] Submissions dashboard (Lit 3)
- [ ] Vite build configuration

### Backoffice UI (v13)
- [ ] AngularJS form list + tree
- [ ] AngularJS form editor
- [ ] package.manifest

### Tests
- [ ] 30+ xUnit controller/validator/workflow tests
- [ ] 4+ Playwright E2E tests
- [ ] Regression tests for v13 compatibility

### Operations
- [ ] NuGet packaging (`dotnet pack`)
- [ ] Docker compose for dev/test
- [ ] CI pipeline verification

---

## 9. Risk Register

| Risk | Impact | Mitigation |
|------|--------|------------|
| NPoco vs EF Core inconsistency across v13/v17 | Medium | Use `IScopeProvider` consistently; `#if` guards for API differences |
| Hardcoded SMTP breaks in production | High | Replace with `IOptions<T>` configuration binding before merging |
| Board delays on feature parity decision | Medium | Proceed with 100% parity; scope can be cut later |
| Lit 3 dashboard complexity | Medium | Use existing Newsletter/CacheManager dashboards as templates |
| AngularJS maintenance burden | Low | Minimal v13 UI; focus investment on v17 Lit dashboards |
