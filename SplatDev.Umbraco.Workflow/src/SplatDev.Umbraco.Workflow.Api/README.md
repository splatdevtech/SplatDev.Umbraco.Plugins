# SplatDev.Umbraco.Workflow.Api

Umbraco-authorised REST API controllers for `SplatDev.Umbraco.Workflow`.
Targets Umbraco 13 (net8.0) and Umbraco 17 (net10.0).

## Installation

```bash
dotnet add package SplatDev.Umbraco.Workflow.Api
```

## Endpoints

All endpoints under `/umbraco/api/Workflow/` — `UmbracoAuthorizedApiController`.

| Method | Route | Purpose |
|---|---|---|
| `GET` | `/Workflow/definitions` | List active workflow definitions |
| `GET` | `/Workflow/definitions/{key}` | Get definition with steps |
| `POST` | `/Workflow/definitions` | Create/version a definition |
| `GET` | `/Workflow/instances` | Paged queue (filters: workflowKey, status, assignedToMe) |
| `POST` | `/Workflow/instances` | Create instance |
| `GET` | `/Workflow/instances/{id}` | Instance detail + definition steps |
| `POST` | `/Workflow/instances/{id}/transition` | Execute transition |

## Dependencies

- `SplatDev.Umbraco.Workflow.Core`
- `SplatDev.Umbraco.Workflow.Persistence`
- `Umbraco.Cms.Web.Common`
- `FluentValidation`

## License

MIT
