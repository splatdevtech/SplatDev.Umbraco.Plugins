# SplatDev.Umbraco.Workflow.Persistence

NPoco-based persistence layer for the SplatDev Workflow plugin, providing entities, FluentMigrator migrations, and repository implementations of the Core contracts.

## Purpose

This project bridges the `Core` domain contracts to Umbraco's persistence stack (NPoco + FluentMigrator). It knows about Umbraco's `IScopeProvider` but has **no HTTP or UI concerns**.

## Key Types

| Type | Role |
|------|------|
| `WorkflowDefinitionEntity` | NPoco-mapped DTO for versioned workflow definitions |
| `WorkflowInstanceEntity` | NPoco-mapped DTO for runtime workflow instances |
| `WorkflowEventEntity` | NPoco-mapped DTO for the append-only audit trail |
| `WorkflowAssignmentEntity` | NPoco-mapped DTO for per-instance assignments |
| `WorkflowTaskEntity` | NPoco-mapped DTO for checklist tasks within instances |
| `M001_CreateSchema` | FluentMigrator initial schema (5 tables + indexes) |
| `MigrationPlan` | Umbraco `IMigrationPlan` wiring for startup execution |
| `WorkflowInstanceRepository` | Implements `IWorkflowInstanceStore` (Core contract) |
| `WorkflowDefinitionRepository` | CRUD for workflow definitions |
| `WorkflowEventRepository` | Implements `IWorkflowEventStore` (Core contract) |
| `WorkflowAssignmentRepository` | Assignment record store |
| `WorkflowTaskRepository` | Task record store |
| `WorkflowDefinitionResolver` | Implements `IWorkflowResolver` (resolves by key + version) |
| `DefaultAssignmentRouter` | Implements `IAssignmentRouter` (Core contract) |
| `JsonMetadataDataProvider` | Default `IWorkflowDataProvider` using JSON metadata columns |

## Database Schema

Created by `M001_CreateSchema`:

| Table | Purpose |
|-------|---------|
| `splatWorkflowDefinition` | JSON workflow definitions (versioned, key + version unique) |
| `splatWorkflowInstance` | Active workflow instances (indexed by workflow + status + step) |
| `splatWorkflowEvent` | Append-only audit trail (indexed by instance + timestamp) |
| `splatWorkflowAssignment` | Per-instance assignment records (active flag + instance index) |
| `splatWorkflowTask` | Checklist tasks within an instance (indexed by instance) |

## SQL Safety

All queries use NPoco parameterized placeholders (`@0`, `@1`, etc.) or NPoco's ORM methods (`Insert`, `SingleOrDefault`). No string concatenation for SQL. This follows SPL-2491/SPL-2492 remediation patterns.

## Integration Modes

The `IWorkflowDataProvider` interface (from Core) supports three integration modes:

1. **JSON metadata bag** (default) — `JsonMetadataDataProvider` uses `WorkflowInstanceEntity.MetadataJson`
2. **EF entity** — host provides `DbContext` + mapping (host-implemented)
3. **Host-supplied** — host implements `IWorkflowDataProvider` directly (no Persistence code needed)

## Getting Started

```bash
# Build
dotnet build src/SplatDev.Umbraco.Workflow.Persistence/

# Run persistence tests (requires LocalDB)
dotnet test tests/SplatDev.Umbraco.Workflow.Persistence.Tests/
```

## Extension Points

- Implement `IWorkflowDataProvider` in your host project for custom data sources
- Wire via DI: `builder.AddSplatDevWorkflowJsonProvider()` for the default JSON-backed provider
- Custom resolution: register your own `IWorkflowResolver` implementation

---

Copyright SplatDev
