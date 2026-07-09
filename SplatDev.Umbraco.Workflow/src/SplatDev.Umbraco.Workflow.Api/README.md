# SplatDev.Umbraco.Workflow.Api

Umbraco-authorised REST API controllers for the SplatDev Workflow plugin, with FluentValidation on every inbound DTO.

## Purpose

This project exposes the `Core` engine and `Persistence` repositories via Umbraco `UmbracoAuthorizedApiController` endpoints. It handles **only** HTTP concerns: routing, validation, and serialisation.

## Endpoints

All endpoints are prefixed with `/umbraco/backoffice/SplatDevWorkflow/` (via `[PluginController("SplatDevWorkflow")]`).

### Workflow Instances

| Method | Path | Description |
|--------|------|-------------|
| GET | `/WorkflowInstances/List` | Paged, filterable instance list (workflow, status, group, department, free text) |
| GET | `/WorkflowInstances/{id}` | Single instance detail |
| POST | `/WorkflowInstances/Create` | Start a new workflow instance |
| POST | `/WorkflowInstances/{id}/Transition` | Advance instance via action key |

### Workflow Definitions

| Method | Path | Description |
|--------|------|-------------|
| GET | `/WorkflowDefinitions/List` | All active definitions |
| GET | `/WorkflowDefinitions/{key}` | Highest-version definition by key |
| POST | `/WorkflowDefinitions/Save` | Create or update a definition |

### Workflow Tasks

| Method | Path | Description |
|--------|------|-------------|
| GET | `/WorkflowTasks/{instanceId}/tasks` | List tasks for an instance |
| POST | `/WorkflowTasks/{instanceId}/tasks` | Batch-set task completions |

### Themes

| Method | Path | Description |
|--------|------|-------------|
| GET | `/WorkflowThemes/List` | Available CSS themes |

## Validation

Every POST/PUT request body is validated via FluentValidation:

| Validator | Validates |
|-----------|-----------|
| `CreateInstanceRequestValidator` | `WorkflowKey` (required, max 64) |
| `TransitionRequestValidator` | `ActionKey` (required, max 64) |
| `SetTaskCompletionRequestValidator` | `Entries` (required, each `TaskId > 0`) |
| `WorkflowDefinitionDtoValidator` | `Key` (required, max 64), `Label` (required, max 256), `Version > 0`, `DefinitionJson` (required) |

## Response DTOs

All response types are in `Api.Contracts` — separate from `Persistence.Entities` to avoid leaking internals:

- `WorkflowDefinitionDto` — public definition surface
- `WorkflowInstanceDto` — public instance surface
- `PagedResult<T>` — generic pagination envelope
- `CreateInstanceRequest` / `TransitionRequest` — inbound DTOs with validation

## Security

- All controllers extend `UmbracoAuthorizedApiController` — Umbraco backoffice auth required
- No SQL from user input reaches the database: controllers delegate to `Core.Engine` and `Persistence.Repositories` which use NPoco parameterised queries
- Consistent error envelope via `ProblemDetails` on failures

## Getting Started

```bash
# Build
dotnet build src/SplatDev.Umbraco.Workflow.Api/

# Run E2E tests (requires running Umbraco host on localhost:5000)
dotnet test tests/SplatDev.Umbraco.Workflow.E2E.Tests/
```

## Extension Points

- Register custom `IActionMessageDispatcher` to wire notifications (email/SMS/Slack)
- Register custom `IWorkflowDataProvider` for alternative data sources
- DI wiring via `SplatDevWorkflowComposer` and `ServiceCollectionExtensions`

---

Copyright SplatDev
