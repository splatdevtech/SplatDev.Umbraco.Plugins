# SplatDev.Umbraco.Workflow — v1 Design

**Status:** Draft for review
**Date:** 2026-05-26
**Author:** Carlos Casalicchio (with Claude)
**Target:** Umbraco 13.x · .NET 8 · AngularJS backoffice

---

## 1. Goals & Non-Goals

### Goals
- Provide a **drop-in workflow engine** for Umbraco 13 projects: state machine, transitions, history, assignments.
- Provide a **pizza-delivery progress chart** (the visual step indicator used in Findlay Auto's onboarding) as a generic, themeable component.
- Provide a **queue UI** (filtered list of workflow instances assigned to the current user / group / department) as a generic, themeable component.
- Provide a **Config Editor** dashboard for admins to define workflows, steps, transitions, themes, and queue columns at runtime.
- Support **three integration models** for a host's domain data via a single `IWorkflowDataProvider` contract:
  1. JSON metadata bag (default, zero-integration)
  2. EF entity (host extends a base class)
  3. Host-supplied provider (host implements the interface against its own schema, no plugin coupling)
- Ship **assignments, action-message hooks (host-provided transport), and tasks-to-perform** sub-checklists.
- Ship **3 reference themes** for the chart and queue.
- Be installable into Findlay Auto with no schema migration of Findlay's existing data (a separate follow-up plan covers that migration).

### Non-Goals (v1)
- Bundled email/SMS/Slack transport. Host implements `IActionMessageDispatcher`.
- File-attachment storage. Host owns its own file columns.
- Umbraco Forms workflow integration as a bundled package (host writes a thin Forms workflow that calls the plugin's API).
- Auto-assign rule DSL. v1 supports only a fixed set of strategies (`assignToGroup`, `assignToSubmitter`, `manual`). DSL is a v2 candidate.
- Tags. Host can store tags in the JSON metadata bag.
- Umbraco 14+ / Bellissima support. Targets 13 only. Forward-port is a v2 candidate.

---

## 2. Architecture

### Solution layout

A **new standalone solution** at the target path. Not added to `SplatDev.Core.sln` — this is a UI-bearing Umbraco plugin and lives as its own deliverable.

```
SplatDev.Umbraco.Workflow/
├── SplatDev.Umbraco.Workflow.sln
├── src/
│   ├── SplatDev.Umbraco.Workflow.Core/           # engine, contracts, domain types — no Umbraco refs
│   ├── SplatDev.Umbraco.Workflow.Persistence/    # NPoco entities, migrations, repos — Umbraco refs
│   ├── SplatDev.Umbraco.Workflow.Api/            # Umbraco-authorised API controllers + DTOs
│   ├── SplatDev.Umbraco.Workflow.Backoffice/     # App_Plugins: dashboard, config editor, queue, chart
│   └── SplatDev.Umbraco.Workflow.Themes/         # css tokens + template variants
├── tests/
│   ├── SplatDev.Umbraco.Workflow.Core.Tests/     # xUnit, engine and contract tests
│   └── SplatDev.Umbraco.Workflow.E2E.Tests/      # Playwright, backoffice UI flows
├── samples/
│   └── SplatDev.Umbraco.Workflow.Sample/         # minimal Umbraco 13 host that demos the plugin
└── docs/
    ├── specs/                                    # this folder
    ├── plans/                                    # implementation plans
    └── integration-guide.md                      # written during v1 build
```

### Project responsibilities & boundaries

| Project | Knows about | Does NOT know about |
|---|---|---|
| `Core` | C# primitives, the workflow contracts, the engine's state-transition logic | Umbraco, HTTP, NPoco, AngularJS |
| `Persistence` | Core contracts, NPoco, Umbraco's `IScopeProvider`, FluentMigrator | HTTP, UI |
| `Api` | Core, Persistence (via interfaces), Umbraco controllers, FluentValidation | UI internals |
| `Backoffice` | Umbraco App_Plugins manifest, AngularJS, the Api's REST contracts | Core / Persistence internals |
| `Themes` | CSS variable tokens, AngularJS templates | engine, persistence |

**Contracts only flow downward** (Core defines `IWorkflowEngine`; Api consumes it). No upward dependencies. Each project can be reasoned about in isolation.

### Key contracts (`Core`)

```csharp
// Definitions (immutable at runtime once published)
public interface IWorkflow { string Key { get; } string Label { get; } IReadOnlyList<IWorkflowStep> Steps { get; } }
public interface IWorkflowStep {
    string Key { get; } string Label { get; }
    IReadOnlyList<IWorkflowAction> Actions { get; }     // user-facing buttons (Approve, Reject, …)
    string? Department { get; }                          // for queue routing
    string? Group { get; }                               // for queue routing
    IReadOnlyList<IActionMessage> PreActionMessages { get; }
    IReadOnlyList<IActionMessage> PostActionMessages { get; }
}
public interface IWorkflowAction { string Key { get; } string Label { get; } string NextStepKey { get; } AssignmentStrategy Assignment { get; } }
public enum AssignmentStrategy { AssignToGroup, AssignToSubmitter, Manual }

// An action message is a *named notification intent* attached to a step
// (pre = fires before the transition is committed, post = after).
// The plugin does not send the message itself — it builds a WorkflowEvent
// and hands it to IActionMessageDispatcher, which the host implements
// (email, Slack, SignalR, …).
public interface IActionMessage { string Alias { get; } string Label { get; } ActionMessageAudience Audience { get; } }
public enum ActionMessageAudience { Default, Submitter, AssignedGroup, Custom }

// Runtime data
public interface IWorkflowInstance {
    long Id { get; }
    string WorkflowKey { get; }
    string CurrentStepKey { get; }
    WorkflowStatus Status { get; }                       // Open, Completed, Cancelled
    DateTime CreatedAt { get; } DateTime UpdatedAt { get; }
    string CreatedBy { get; } string UpdatedBy { get; }
    string? MetadataJson { get; }                        // for JSON-bag integration
}

// Host extension points
public interface IWorkflowDataProvider {
    IReadOnlyList<WorkflowDisplayRow> GetDisplayRows(WorkflowQueryFilter filter);
    string? GetSearchableValue(long instanceId, string fieldKey);
    IReadOnlyList<DisplayColumn> GetColumns(string workflowKey);
}
public interface IAssignmentRouter { Assignment Route(IWorkflowInstance instance, IWorkflowAction action, string actorUsername); }
public interface IActionMessageDispatcher { Task DispatchAsync(WorkflowEvent evt, CancellationToken ct); }
public interface IWorkflowEventStore { Task AppendAsync(WorkflowEvent evt, CancellationToken ct); IReadOnlyList<WorkflowEvent> GetHistory(long instanceId); }

// The engine
public interface IWorkflowEngine {
    Task<TransitionResult> TransitionAsync(long instanceId, string actionKey, string actorUsername, CancellationToken ct);
    Task<IWorkflowInstance> CreateAsync(string workflowKey, string? metadataJson, string actorUsername, CancellationToken ct);
}
```

The **JSON-bag** default provider (`JsonMetadataDataProvider`) implements `IWorkflowDataProvider` by parsing `IWorkflowInstance.MetadataJson` with a host-supplied schema (registered in DI). Zero plumbing to use.

The **EF base class** option ships as an optional NuGet sub-package (`SplatDev.Umbraco.Workflow.Persistence.EntityFramework`) that hosts can opt into; it provides `WorkflowItem` (abstract entity), `EntityFrameworkDataProvider<TItem>`, and migration helpers. Out-of-the-box NPoco does NOT depend on EF — keeps the default install lean.

---

## 3. Persistence

NPoco, matching Umbraco 13's convention. FluentMigrator for schema migrations (Umbraco's standard).

### Schema (default)

```sql
-- Workflow definitions (one row per published workflow version)
CREATE TABLE splatWorkflowDefinition (
    id              INT IDENTITY(1,1) PRIMARY KEY,
    [key]           NVARCHAR(64)   NOT NULL,
    label           NVARCHAR(256)  NOT NULL,
    version         INT            NOT NULL,
    definitionJson  NVARCHAR(MAX)  NOT NULL,         -- serialized IWorkflow
    isActive        BIT            NOT NULL,
    createdAt       DATETIME       NOT NULL,
    createdBy       NVARCHAR(256)  NOT NULL,
    CONSTRAINT UQ_splatWorkflowDefinition_key_version UNIQUE ([key], version)
);

-- Workflow instances
CREATE TABLE splatWorkflowInstance (
    id              BIGINT IDENTITY(1,1) PRIMARY KEY,
    workflowKey     NVARCHAR(64)   NOT NULL,
    workflowVersion INT            NOT NULL,
    currentStepKey  NVARCHAR(64)   NOT NULL,
    status          TINYINT        NOT NULL,         -- 0=Open, 1=Completed, 2=Cancelled
    metadataJson    NVARCHAR(MAX)  NULL,
    createdAt       DATETIME       NOT NULL,
    createdBy       NVARCHAR(256)  NOT NULL,
    updatedAt       DATETIME       NOT NULL,
    updatedBy       NVARCHAR(256)  NOT NULL,
    INDEX IX_splatWorkflowInstance_workflow (workflowKey, status, currentStepKey)
);

-- Append-only event log (history + audit)
CREATE TABLE splatWorkflowEvent (
    id              BIGINT IDENTITY(1,1) PRIMARY KEY,
    instanceId      BIGINT         NOT NULL,
    eventType       TINYINT        NOT NULL,         -- 0=Created, 1=Transition, 2=Comment, 3=Assignment, 4=ActionMessage
    fromStepKey     NVARCHAR(64)   NULL,
    toStepKey       NVARCHAR(64)   NULL,
    actionKey       NVARCHAR(64)   NULL,
    payloadJson     NVARCHAR(MAX)  NULL,
    actorUsername   NVARCHAR(256)  NOT NULL,
    occurredAt      DATETIME       NOT NULL,
    INDEX IX_splatWorkflowEvent_instance (instanceId, occurredAt)
);

-- Current assignment(s) — one row per active assignment per instance
CREATE TABLE splatWorkflowAssignment (
    id              BIGINT IDENTITY(1,1) PRIMARY KEY,
    instanceId      BIGINT         NOT NULL,
    assignedTo      NVARCHAR(256)  NULL,             -- username; NULL = group/department only
    assignedToGroup NVARCHAR(64)   NULL,
    department      NVARCHAR(64)   NULL,
    assignedAt      DATETIME       NOT NULL,
    isActive        BIT            NOT NULL,
    INDEX IX_splatWorkflowAssignment_active (isActive, instanceId)
);
-- assignedToGroup vs department: a "group" is a *role* (manager, payroll, IT)
-- whereas "department" is an *organizational scope* (Lincoln dealership, Spokane).
-- Queue filters can be applied independently. Either or both may be NULL.

-- Tasks-to-perform sub-checklists per instance
CREATE TABLE splatWorkflowTask (
    id              BIGINT IDENTITY(1,1) PRIMARY KEY,
    instanceId      BIGINT         NOT NULL,
    alias           NVARCHAR(64)   NOT NULL,
    name            NVARCHAR(256)  NOT NULL,
    description     NVARCHAR(MAX)  NULL,
    isCompleted     BIT            NOT NULL,
    completedAt     DATETIME       NULL,
    completedBy     NVARCHAR(256)  NULL,
    departmentId    INT            NULL,
    INDEX IX_splatWorkflowTask_instance (instanceId)
);
```

All table names use `splatWorkflow*` prefix. All schema changes go through FluentMigrator migrations under `SplatDev.Umbraco.Workflow.Persistence/Migrations/`. No raw DDL outside migrations.

### Repository layer

One `IRepository<T>`-style interface per entity, NPoco-backed default implementation. Repos are scoped (DI lifetime: scoped). All queries via NPoco fluent or `Sql.Builder` — no string concatenation.

---

## 4. Public API

All endpoints under `/umbraco/api/Workflow/` — `UmbracoAuthorizedApiController` base. Problem Details (RFC 7807) for errors. Pagination on list endpoints (`page`, `pageSize`, response includes total).

| Method | Route | Purpose |
|---|---|---|
| `GET`  | `/Workflow/definitions` | List active workflow definitions |
| `GET`  | `/Workflow/definitions/{key}` | Get a single definition |
| `POST` | `/Workflow/definitions` | Create/version a definition (admin) |
| `PUT`  | `/Workflow/definitions/{key}/activate` | Mark a version active (admin) |
| `GET`  | `/Workflow/instances` | Paged queue for current user — accepts `workflowKey`, `status`, `assignedToMe`, `group`, `department`, custom filter keys |
| `GET`  | `/Workflow/instances/{id}` | Full instance + history + tasks |
| `POST` | `/Workflow/instances` | Create instance — body: `{workflowKey, metadataJson}` |
| `POST` | `/Workflow/instances/{id}/transition` | Transition — body: `{actionKey}` |
| `POST` | `/Workflow/instances/{id}/tasks` | Bulk-set task completion |
| `GET`  | `/Workflow/themes` | List installed themes |
| `GET`  | `/Workflow/themes/{name}` | Get a theme's tokens + active templates |

DTOs live in `SplatDev.Umbraco.Workflow.Api/Contracts/`. Input validation: Data Annotations for simple, FluentValidation for the transition / definition payloads.

---

## 5. Backoffice UI

A new Umbraco section, `Workflow`, with three trees:
- **Queues** — one node per workflow, opens the queue dashboard for that workflow.
- **Definitions** — list of workflow definitions, opens the Config Editor.
- **Themes** — list of installed themes, preview + token editor.

All under `App_Plugins/SplatDev.Workflow/`. AngularJS module name: `splatWorkflow`. No mixing with the host's AngularJS — the plugin's controllers, services, and directives are namespaced (`splat-workflow-queue`, `splatWorkflow.dashResources`, etc.).

### Queue dashboard
- Filter bar (workflow, status, mine/all, group, department, free-text search).
- Sortable, paged table whose columns are driven by the active `IWorkflowDataProvider.GetColumns()` + theme overrides.
- Row click → instance detail flyout (history, tasks, transition buttons).

### Pizza-delivery chart
- Renders the current workflow's steps as a sequence of segments.
- Each segment shows label, status (pending/active/done/skipped), optional badge (assignee, deadline).
- Theme controls colors, typography, and layout (horizontal stepper vs vertical donut vs compact strip).

### Config Editor
- One dashboard per workflow: edit steps, actions, next-step branching, assignment strategy, action messages.
- One panel per theme: editable CSS variable tokens (live preview), choice of layout template per component.
- Saves via `POST /Workflow/definitions` (versioned — old instances keep using the version they were created with).

### i18n
- Resource files (`splatWorkflow.en.xml`, `splatWorkflow.es.xml`) for all backoffice strings. Default English + Spanish per `dotnet-fullstack-standards`. Host can add more locales by dropping files into `App_Plugins/SplatDev.Workflow/Lang/`.

---

## 6. Theming model

**Hybrid** — CSS variable tokens for color/typography/spacing; template-swap for major layout variants.

### Token layer (default for all themes)
A theme is a `.css` file under `App_Plugins/SplatDev.Workflow/Themes/{name}/theme.css` declaring CSS custom properties on `[data-swf-theme="{name}"]`:

```css
[data-swf-theme="classic"] {
    /* Chart */
    --swf-chart-step-pending-bg:   #e5e7eb;
    --swf-chart-step-active-bg:    #2563eb;
    --swf-chart-step-done-bg:      #16a34a;
    --swf-chart-step-skipped-bg:   #94a3b8;
    --swf-chart-step-radius:       4px;
    --swf-chart-step-font:         "Inter", sans-serif;

    /* Queue */
    --swf-queue-row-hover-bg:      #f1f5f9;
    --swf-queue-row-selected-bg:   #dbeafe;
    --swf-queue-density:           "comfortable";    /* read by JS to choose padding */
}
```

Host overrides by declaring the same custom properties at a higher specificity, or by editing tokens via the Themes dashboard (persisted as a theme override row).

### Layout layer (per-component template variants)
Each themeable component declares a set of template variants. The theme manifest selects one:

```json
// App_Plugins/SplatDev.Workflow/Themes/classic/manifest.json
{
    "name": "classic",
    "label": "Classic",
    "templates": {
        "chart": "horizontal-stepper",
        "queue": "table"
    }
}
```

Available chart templates (v1): `horizontal-stepper`, `vertical-donut`, `compact-strip`.
Available queue templates (v1): `table`, `cards`.

A new theme is either:
- (cheap) a token-only theme — just a `.css` file with overrides, reuses existing templates.
- (full) a layout theme — provides one or more template variant directives + a manifest.

### Shipped themes (v1)
1. **Classic** — horizontal stepper + table queue, modeled on the current Findlay onboarding dashboard. Light theme, neutral palette.
2. **Modern** — horizontal stepper + cards queue, denser typography, indigo/emerald palette, dark-mode-aware.
3. **Compact** — compact-strip chart + table queue with tighter row density, designed for ops dashboards on smaller monitors.

---

## 7. Integration walkthrough (a host adopting the plugin)

**Minimal install (JSON-bag mode):**

1. `dotnet add package SplatDev.Umbraco.Workflow` (Core + Persistence + Api + Backoffice + Themes).
2. Add `services.AddSplatDevWorkflow()` in `Program.cs` (registers all services, runs migrations on boot).
3. Define a workflow JSON file under `App_Data/SplatDevWorkflow/Seeds/onboarding.json` — first boot seeds it as version 1.
4. Configure columns + searchable fields for the queue via `services.AddSplatDevWorkflow().UseJsonMetadataProvider(opts => { opts.Columns(...) })`.
5. (Optional) Implement `IActionMessageDispatcher` for emails / Slack / etc.

**Strongly-typed mode:**

1. As above, plus `dotnet add package SplatDev.Umbraco.Workflow.EntityFramework`.
2. Define `class JobApplication : WorkflowItem` with your strongly-typed fields, register the `DbContext`.
3. `services.AddSplatDevWorkflow().UseEntityFramework<JobApplication>()`.

**Custom provider:**

1. Implement `IWorkflowDataProvider` against your existing schema.
2. `services.AddSplatDevWorkflow().UseDataProvider<MyProvider>()`.

The Findlay migration (separate plan) replaces the existing `SplatDev.Umbraco.Sections.Onboarding` with the plugin in custom-provider mode, mapping the existing `hireologyApplications` table through a `HireologyDataProvider : IWorkflowDataProvider`.

---

## 8. v1 deliverables

**In:**
- All 5 src projects, all interfaces, the engine, the 3 themes, the Config Editor, queue dashboard, chart, history view, tasks-to-perform.
- JSON-metadata provider (default).
- EntityFramework sub-package (sibling NuGet, optional install).
- Sample host project that demos all 3 integration modes.
- Integration guide doc (`docs/integration-guide.md`).
- xUnit unit tests on `Core` (state machine, validation), Playwright E2E on the sample host (queue load, transition, theme switch, config edit).

**Out (deferred to v2):**
- Umbraco 14+ / Bellissima support (Lit-based backoffice rewrite).
- Bundled email/SMS transport.
- File-attachment column.
- Auto-assign rule DSL.
- Bundled Umbraco Forms workflow.
- Bulk import / export of workflow definitions.
- Multi-tenant scoping.

---

## 9. Testing strategy

Per `testing-standards` and `webapp-testing`:

**Unit tests (`SplatDev.Umbraco.Workflow.Core.Tests`)** — xUnit:
- Engine state-transition validation (allowed/disallowed actions per step).
- Assignment router strategies.
- JSON-metadata schema mapping.
- Event store append + replay.

**Integration tests (`SplatDev.Umbraco.Workflow.Persistence.Tests`)** — xUnit + LocalDB:
- Each migration applies cleanly on a fresh DB.
- Repos round-trip correctly.
- **No mocked DB** — real MSSQL LocalDB per `testing-standards`.

**E2E tests (`SplatDev.Umbraco.Workflow.E2E.Tests`)** — Playwright against the sample host:
- Queue loads and paginates.
- Transition from a step succeeds and re-renders the chart.
- Theme switch updates tokens and template variant.
- Config Editor saves a new version; old instances still resolve via their pinned version.
- i18n: `?culture=es` swaps strings.

**Build gates:**
- `dotnet build` clean (0 warnings, 0 errors, StyleCop enabled).
- All test suites pass on CI before merge.

---

## 10. Open questions for follow-ups

These do NOT block v1 implementation; they're tracked for after first release.

- **Workflow versioning UX:** when an admin edits a published definition, do we hard-version (instances stay on old version) or migrate them? v1 hard-versions; UX for migration is a v2 question.
- **Findlay migration path:** Findlay will eventually adopt the plugin in custom-provider mode. The migration plan is a **separate doc** (`docs/specs/2026-XX-XX-findlay-migration.md`) once v1 ships.
- **Themes marketplace:** can hosts ship their own themes as NuGets? Probably yes via a `[ThemeRegistration]` attribute scanned at startup; defer concrete design until a real second-host need exists.
- **Localization beyond en/es:** ship hooks now; community can contribute resource files.

---

## Approval

If this matches your intent, the next step is to write the implementation plan (`docs/plans/2026-05-26-workflow-plugin-v1-plan.md`) breaking v1 into concrete, ordered tasks with verifiable acceptance criteria per task.
