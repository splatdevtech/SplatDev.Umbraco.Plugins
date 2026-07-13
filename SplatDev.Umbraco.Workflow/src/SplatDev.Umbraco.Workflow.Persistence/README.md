# SplatDev.Umbraco.Workflow.Persistence

NPoco persistence layer for `SplatDev.Umbraco.Workflow`.
Targets Umbraco 13 (net8.0) and Umbraco 17 (net10.0).

## Installation

```bash
dotnet add package SplatDev.Umbraco.Workflow.Persistence
```

## Dependencies

- `SplatDev.Umbraco.Workflow.Core`
- `Umbraco.Cms.Core` + `Umbraco.Cms.Infrastructure`

## Features

- NPoco entities: `WorkflowDefinitionEntity`, `WorkflowInstanceEntity`, `WorkflowEventEntity`, `WorkflowAssignmentEntity`
- Repository implementations: `WorkflowDefinitionStore`, `WorkflowInstanceStore`
- FluentMigrator migration: `CreateWorkflowTables`
- `WorkflowComposer` — auto-registers services via `IComposer`

## Registration

```csharp
// Program.cs
builder.CreateUmbracoBuilder()
    .AddBackOffice()
    .AddWebsite()
    .AddDeliveryApi()
    .AddComposers()
    .Build();
```

The `WorkflowComposer` automatically registers all persistence services.

## Schema

| Table | Purpose |
|---|---|
| `splatWorkflowDefinition` | Workflow definitions (key, label, version, JSON) |
| `splatWorkflowInstance` | Runtime instances (step, status, metadata) |
| `splatWorkflowEvent` | Append-only audit log |
| `splatWorkflowAssignment` | User/group/department assignments |

## License

MIT
