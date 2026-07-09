# SplatDev.Umbraco.Workflow.Core

Domain contracts, enums, and the `WorkflowEngine` state machine for the SplatDev Umbraco Workflow plugin.

## Purpose

This project defines the **pure C# boundaries** that every other layer (persistence, API, backoffice) depends on. It has **no Umbraco, HTTP, or database dependencies** — only .NET primitives and interfaces.

## Key Types

| Type | Role |
|------|------|
| `IWorkflow` | Ordered sequence of steps that an instance progresses through |
| `IWorkflowStep` | A single step with available actions and assignment metadata |
| `IWorkflowAction` | User-facing button that transitions to a target step |
| `IWorkflowInstance` | Runtime instance tracking current step, status, and metadata |
| `IWorkflowEngine` | State machine: `CreateAsync` / `TransitionAsync` |
| `IWorkflowDataProvider` | Host extension point for mapping instances to display data |
| `IActionMessageDispatcher` | Host-implemented transport (email/SMS/Slack are out of scope) |
| `IAssignmentRouter` | Decides who/which group the next assignment goes to |
| `IWorkflowResolver` | Looks up workflow definitions by key + version |
| `IWorkflowEventStore` | Append-only audit trail |
| `IWorkflowInstanceStore` | Persistence boundary for workflow instances |

## Getting Started

```bash
# Build
dotnet build src/SplatDev.Umbraco.Workflow.Core/

# Run unit tests
dotnet test tests/SplatDev.Umbraco.Workflow.Core.Tests/
```

## Integration

See the [main README](../../README.md) for architecture overview, REST API, and database schema.

See the [integration guide](../../docs/integration-guide.md) for host wiring instructions.

## Extension Points

All host integrations are defined as interfaces in this project:

- Implement `IWorkflowDataProvider` for custom queue dashboards
- Implement `IActionMessageDispatcher` for notifications
- Implement `IAssignmentRouter` for custom assignment logic
- Use `AssignmentStrategy` enum (`AssignToGroup`, `AssignToSubmitter`, `Manual`)

---

Copyright SplatDev
