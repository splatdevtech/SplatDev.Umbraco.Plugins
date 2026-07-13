# SplatDev.Umbraco.Workflow.Core

Workflow engine core — contracts, models, state-transition engine, and extension points.
Framework-independent (no Umbraco, NPoco, or HTTP dependency).

## Installation

```bash
dotnet add package SplatDev.Umbraco.Workflow.Core
```

## Target Frameworks

- `net8.0` (Umbraco 13)
- `net10.0` (Umbraco 17)

## Features

- **State machine engine** — `IWorkflowEngine` with `CreateAsync` and `TransitionAsync`
- **Workflow definitions** — steps, actions, branching, assignment strategy
- **Assignment routing** — `AssignToGroup`, `AssignToSubmitter`, `Manual`
- **Action messages** — pre/post-transition notification hooks
- **Event store** — append-only audit log for all transitions
- **Data provider** — `IWorkflowDataProvider` for host data integration
- **JSON metadata provider** — zero-code integration via `IWorkflowInstance.MetadataJson`
- Store abstractions — persistence layer implements `IWorkflowDefinitionStore`, `IWorkflowInstanceStore`

## Architecture

```
Host App
    |
    v
IWorkflowEngine  -----------------> IWorkflowDefinitionStore
    |                                     (persistence layer)
    +--> IWorkflowInstanceStore
    +--> IWorkflowEventStore               IWorkflowDataProvider
    +--> IAssignmentRouter                 (host implements)
    +--> IActionMessageDispatcher
```

## Usage

### Create a workflow instance

```csharp
var instance = await engine.CreateAsync("onboarding", metadataJson, "admin", ct);
```

### Execute a transition

```csharp
var result = await engine.TransitionAsync(instance.Id, "approve", "manager", ct);
// result.Success, result.FromStep, result.ToStep, result.NewStatus
```

## Contracts

| Interface | Purpose |
|---|---|
| `IWorkflow` | Workflow definition (key, label, steps) |
| `IWorkflowStep` | Step in the workflow (key, label, actions, department, group) |
| `IWorkflowAction` | User-facing button (key, label, next step, assignment strategy) |
| `IWorkflowInstance` | Runtime instance (id, workflow key, current step, status) |
| `IWorkflowEngine` | State machine — `CreateAsync`, `TransitionAsync` |
| `IWorkflowDefinitionStore` | Persistence: CRUD for definitions |
| `IWorkflowInstanceStore` | Persistence: CRUD for instances |
| `IWorkflowEventStore` | Persistence: append-only event log |
| `IWorkflowDataProvider` | Host data: display rows, searchable fields, columns |
| `IAssignmentRouter` | Routes instance to a user/group post-transition |
| `IActionMessageDispatcher` | Host implements: email, Slack, SignalR notifications |

## License

MIT
