# SplatDev.Umbraco.Workflow

Multi-step workflow engine for Umbraco 13 — enables content-approval pipelines, task routing, assignment strategies, and configurable action messaging. Workflows are defined via JSON seeds, executed as state machines backed by NPoco persistence, and surfaced in an AngularJS backoffice dashboard.

**Status: v1 design complete — implementation pending.** This package is currently a design artifact. The spec below describes the planned v1 surface.

## Planned install

```sh
dotnet add package SplatDev.Umbraco.Workflow
```

Then in `Program.cs`:

```csharp
services.AddSplatDevWorkflow();
```

## Planned features (v1)

### Core engine

| Interface | Purpose |
|---|---|
| `IWorkflow` | Named definition with ordered steps |
| `IWorkflowStep` | Step with actions, department/group routing, pre/post action messages |
| `IWorkflowAction` | User-facing action (Approve, Reject, Request Changes) with target step and assignment strategy |
| `IActionMessage` | Named notification per step (alias, audience — requester/assignee/group) |
| `IWorkflowInstance` | Runtime state: workflow key, current step, status, metadata JSON |
| `IWorkflowEngine` | `TransitionAsync` and `CreateAsync` — the state-machine core |
| `IAssignmentRouter` | Pluggable routing strategies (round-robin, least-loaded, explicit) |
| `IActionMessageDispatcher` | Host-provided transport for email/Slack notifications |
| `IWorkflowEventStore` | Full audit trail and event history |
| `IWorkflowDataProvider` | Host integration point for domain-data columns, search, rows |

### API surface

Planned REST endpoints:

| Method | Route | Purpose |
|---|---|---|
| GET | `/Workflow/definitions` | List all workflow definitions |
| GET | `/Workflow/definitions/{key}` | Get a single definition |
| POST | `/Workflow/definitions` | Create/update a definition |
| GET | `/Workflow/instances` | Paged queue of instances |
| POST | `/Workflow/instances` | Create a new instance |
| POST | `/Workflow/instances/{id}/transition` | Execute a state transition |
| GET | `/Workflow/themes` | List available theme tokens |

### Backoffice

- AngularJS section with three dashboards:
  - **Queue** — paged instance list with filters (status, assignee, workflow type).
  - **Definitions** — CRUD workflow definitions with step/action editors.
  - **Themes** — CSS variable tokens per theme, stored in `App_Plugins/SplatDev.Workflow/Themes/{name}/theme.css`.

### Persistence

Five tables via NPoco + FluentMigrator:

| Table | Purpose |
|---|---|
| `splatWorkflowDefinition` | Workflow definitions (JSON body for steps) |
| `splatWorkflowInstance` | Active/pending workflow instances |
| `splatWorkflowEvent` | Full audit trail |
| `splatWorkflowAssignment` | Current step assignments |
| `splatWorkflowTask` | Pending task queue |

### Integration models

Three integration approaches for domain-data access:

1. **JSON metadata bag** (zero-code default) — workflow instances carry a JSON `Metadata` column. No domain coupling.
2. **EF entity base class** — domain entities inherit `WorkflowEntity` and get automatic instance tracking.
3. **Custom `IWorkflowDataProvider`** — implement your own data-provider for custom schemas.

## Configuration

Definitions are seeded from `App_Data/SplatDevWorkflow/Seeds/*.json` on first app start:

```json
{
  "key": "content-approval",
  "label": "Content Approval",
  "steps": [
    {
      "key": "draft",
      "label": "Draft",
      "actions": [
        {
          "key": "submit",
          "label": "Submit for Review",
          "nextStep": "review",
          "assignment": { "strategy": "round_robin", "group": "editors" },
          "messages": []
        }
      ]
    }
  ]
}
```

## Planned target

Umbraco 13.x (.NET 8), AngularJS backoffice. Umbraco 14+ (Bellissima / Lit) deferred to v2.

---

**SplatDev.Umbraco.Workflow** — part of the [SplatDev.Umbraco.Plugins](https://github.com/SplatDev-Ltda/SplatDev.Umbraco.Plugins) suite. Licensed under MIT. © SplatDev Ltda.
