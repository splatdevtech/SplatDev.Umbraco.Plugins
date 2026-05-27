# SplatDev.Umbraco.Workflow v1 Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Build a generic Umbraco 13 workflow plugin (engine + queue + themeable pizza-delivery chart + config editor) ready to drop into any host project, including Findlay Auto.

**Architecture:** 5 src projects + tests + sample, NPoco persistence, AngularJS backoffice, hybrid CSS-token + template-variant theming. See `docs/specs/2026-05-26-workflow-plugin-design.md`.

**Tech Stack:** .NET 8 · Umbraco 13.x · NPoco · FluentMigrator · xUnit · Playwright · AngularJS (backoffice constraint of Umbraco 13).

---

## File Structure

Before any code is written, the file layout below is the source of truth. Every later task references paths from here.

```
SplatDev.Umbraco.Workflow/
├── SplatDev.Umbraco.Workflow.sln
├── Directory.Build.props                         # StyleCop, treat warnings as errors, common props
├── .editorconfig                                 # tab/space rules, line endings
├── .gitignore
├── .gitattributes                                # forces *.cs as CRLF (matches Findlay convention)
├── src/
│   ├── SplatDev.Umbraco.Workflow.Core/
│   │   ├── SplatDev.Umbraco.Workflow.Core.csproj
│   │   ├── Contracts/
│   │   │   ├── IWorkflow.cs
│   │   │   ├── IWorkflowStep.cs
│   │   │   ├── IWorkflowAction.cs
│   │   │   ├── IActionMessage.cs
│   │   │   ├── IWorkflowInstance.cs
│   │   │   ├── IWorkflowDataProvider.cs
│   │   │   ├── IAssignmentRouter.cs
│   │   │   ├── IActionMessageDispatcher.cs
│   │   │   ├── IWorkflowEventStore.cs
│   │   │   └── IWorkflowEngine.cs
│   │   ├── Enums/
│   │   │   ├── WorkflowStatus.cs
│   │   │   ├── AssignmentStrategy.cs
│   │   │   ├── ActionMessageAudience.cs
│   │   │   └── WorkflowEventType.cs
│   │   ├── Models/
│   │   │   ├── WorkflowEvent.cs
│   │   │   ├── WorkflowQueryFilter.cs
│   │   │   ├── WorkflowDisplayRow.cs
│   │   │   ├── DisplayColumn.cs
│   │   │   ├── Assignment.cs
│   │   │   └── TransitionResult.cs
│   │   └── Engine/
│   │       └── WorkflowEngine.cs                 # default IWorkflowEngine impl
│   ├── SplatDev.Umbraco.Workflow.Persistence/
│   │   ├── SplatDev.Umbraco.Workflow.Persistence.csproj
│   │   ├── Entities/
│   │   │   ├── WorkflowDefinitionEntity.cs
│   │   │   ├── WorkflowInstanceEntity.cs
│   │   │   ├── WorkflowEventEntity.cs
│   │   │   ├── WorkflowAssignmentEntity.cs
│   │   │   └── WorkflowTaskEntity.cs
│   │   ├── Migrations/
│   │   │   ├── _Initial/M001_CreateSchema.cs
│   │   │   └── MigrationPlan.cs                  # IMigrationPlan registration
│   │   ├── Repositories/
│   │   │   ├── WorkflowDefinitionRepository.cs
│   │   │   ├── WorkflowInstanceRepository.cs
│   │   │   ├── WorkflowEventRepository.cs        # implements IWorkflowEventStore
│   │   │   ├── WorkflowAssignmentRepository.cs
│   │   │   └── WorkflowTaskRepository.cs
│   │   ├── Providers/
│   │   │   └── JsonMetadataDataProvider.cs       # default IWorkflowDataProvider
│   │   └── Routing/
│   │       └── DefaultAssignmentRouter.cs        # default IAssignmentRouter
│   ├── SplatDev.Umbraco.Workflow.Api/
│   │   ├── SplatDev.Umbraco.Workflow.Api.csproj
│   │   ├── Composition/
│   │   │   ├── SplatDevWorkflowComposer.cs       # registers DI
│   │   │   └── ServiceCollectionExtensions.cs    # AddSplatDevWorkflow()
│   │   ├── Controllers/
│   │   │   ├── WorkflowDefinitionsController.cs
│   │   │   ├── WorkflowInstancesController.cs
│   │   │   ├── WorkflowTasksController.cs
│   │   │   └── WorkflowThemesController.cs
│   │   ├── Contracts/
│   │   │   ├── WorkflowDefinitionDto.cs
│   │   │   ├── WorkflowInstanceDto.cs
│   │   │   ├── CreateInstanceRequest.cs
│   │   │   ├── TransitionRequest.cs
│   │   │   ├── SetTaskCompletionRequest.cs
│   │   │   └── PagedResult.cs
│   │   └── Validators/
│   │       ├── TransitionRequestValidator.cs
│   │       └── WorkflowDefinitionValidator.cs
│   ├── SplatDev.Umbraco.Workflow.Backoffice/
│   │   ├── SplatDev.Umbraco.Workflow.Backoffice.csproj
│   │   ├── package.manifest                      # Umbraco App_Plugin manifest
│   │   ├── Sections/
│   │   │   └── WorkflowSection.cs                # ISection registration
│   │   ├── Trees/
│   │   │   ├── QueuesTreeController.cs
│   │   │   ├── DefinitionsTreeController.cs
│   │   │   └── ThemesTreeController.cs
│   │   └── App_Plugins/SplatDev.Workflow/
│   │       ├── package.manifest
│   │       ├── dashboards/
│   │       │   ├── queue.html
│   │       │   ├── queue.controller.js
│   │       │   ├── definitionEditor.html
│   │       │   ├── definitionEditor.controller.js
│   │       │   ├── themes.html
│   │       │   └── themes.controller.js
│   │       ├── directives/
│   │       │   ├── pizzaChart.directive.js
│   │       │   ├── pizzaChart.template-horizontal.html
│   │       │   ├── pizzaChart.template-vertical.html
│   │       │   ├── pizzaChart.template-compact.html
│   │       │   ├── queueTable.directive.js
│   │       │   ├── queueTable.template-table.html
│   │       │   └── queueTable.template-cards.html
│   │       ├── services/
│   │       │   ├── workflowResource.js           # $http API client
│   │       │   └── themeService.js
│   │       ├── lang/
│   │       │   ├── splatWorkflow.en.xml
│   │       │   └── splatWorkflow.es.xml
│   │       └── styles/
│   │           └── base.css                      # structural CSS, no colors
│   ├── SplatDev.Umbraco.Workflow.Themes/
│   │   ├── SplatDev.Umbraco.Workflow.Themes.csproj
│   │   └── App_Plugins/SplatDev.Workflow/Themes/
│   │       ├── classic/
│   │       │   ├── manifest.json
│   │       │   └── theme.css
│   │       ├── modern/
│   │       │   ├── manifest.json
│   │       │   └── theme.css
│   │       └── compact/
│   │           ├── manifest.json
│   │           └── theme.css
├── tests/
│   ├── SplatDev.Umbraco.Workflow.Core.Tests/
│   │   ├── SplatDev.Umbraco.Workflow.Core.Tests.csproj
│   │   └── Engine/WorkflowEngineTests.cs
│   ├── SplatDev.Umbraco.Workflow.Persistence.Tests/
│   │   ├── SplatDev.Umbraco.Workflow.Persistence.Tests.csproj
│   │   └── MigrationTests.cs
│   └── SplatDev.Umbraco.Workflow.E2E.Tests/
│       ├── SplatDev.Umbraco.Workflow.E2E.Tests.csproj
│       └── QueueFlowTests.cs
├── samples/
│   └── SplatDev.Umbraco.Workflow.Sample/
│       ├── SplatDev.Umbraco.Workflow.Sample.csproj
│       ├── Program.cs
│       └── App_Data/SplatDevWorkflow/Seeds/onboarding.json
└── docs/
    ├── specs/2026-05-26-workflow-plugin-design.md
    ├── plans/2026-05-26-workflow-plugin-v1-plan.md
    └── integration-guide.md
```

---

## Phase 1 — Scaffold the solution

### Task 1: Initialize the repo, .sln, and shared build props

**Files:**
- Create: `SplatDev.Umbraco.Workflow.sln`
- Create: `Directory.Build.props`
- Create: `.editorconfig`
- Create: `.gitignore`
- Create: `.gitattributes`

- [ ] **Step 1: Initialize git and create base files**

```bash
cd "/mnt/e/Source/Repos/Umbraco Projects/SplatDev.Umbraco.Workflow"
git init
```

- [ ] **Step 2: Create `.gitattributes` to lock down line endings**

`.gitattributes`:
```
* text=auto
*.cs    text eol=crlf working-tree-encoding=UTF-8
*.csproj text eol=crlf
*.sln   text eol=crlf
*.md    text eol=lf
*.json  text eol=lf
*.js    text eol=lf
*.html  text eol=lf
*.css   text eol=lf
*.xml   text eol=crlf
```

- [ ] **Step 3: Create `.gitignore`**

`.gitignore`:
```
bin/
obj/
.vs/
*.user
*.suo
node_modules/
TestResults/
*.received.*
publish_out/
App_Data/TEMP/
App_Data/Logs/
App_Data/umbraco-net.lck
```

- [ ] **Step 4: Create `.editorconfig`**

`.editorconfig`:
```
root = true

[*]
indent_style = space
indent_size = 4
end_of_line = crlf
charset = utf-8
insert_final_newline = true
trim_trailing_whitespace = true

[*.{md,yml,yaml,json,js,html,css}]
end_of_line = lf
indent_size = 2

[*.csproj]
indent_size = 2
```

- [ ] **Step 5: Create `Directory.Build.props`**

`Directory.Build.props`:
```xml
<Project>
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <LangVersion>latest</LangVersion>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
    <TreatWarningsAsErrors>true</TreatWarningsAsErrors>
    <EnforceCodeStyleInBuild>true</EnforceCodeStyleInBuild>
    <GenerateDocumentationFile>true</GenerateDocumentationFile>
    <NoWarn>$(NoWarn);1591</NoWarn>
    <Authors>SplatDev</Authors>
    <Company>SplatDev</Company>
    <Product>SplatDev Umbraco Workflow</Product>
    <Version>1.0.0</Version>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="StyleCop.Analyzers" Version="1.2.0-beta.556" PrivateAssets="all" />
  </ItemGroup>
</Project>
```

- [ ] **Step 6: Create an empty solution**

```bash
dotnet new sln -n SplatDev.Umbraco.Workflow
```

- [ ] **Step 7: First commit**

```bash
git add .gitattributes .gitignore .editorconfig Directory.Build.props SplatDev.Umbraco.Workflow.sln
git commit -m "chore: scaffold solution and shared build props"
```

---

### Task 2: Create the 5 src projects and 3 test projects

**Files:**
- Create: `src/SplatDev.Umbraco.Workflow.Core/SplatDev.Umbraco.Workflow.Core.csproj`
- Create: `src/SplatDev.Umbraco.Workflow.Persistence/SplatDev.Umbraco.Workflow.Persistence.csproj`
- Create: `src/SplatDev.Umbraco.Workflow.Api/SplatDev.Umbraco.Workflow.Api.csproj`
- Create: `src/SplatDev.Umbraco.Workflow.Backoffice/SplatDev.Umbraco.Workflow.Backoffice.csproj`
- Create: `src/SplatDev.Umbraco.Workflow.Themes/SplatDev.Umbraco.Workflow.Themes.csproj`
- Create: `tests/SplatDev.Umbraco.Workflow.Core.Tests/SplatDev.Umbraco.Workflow.Core.Tests.csproj`
- Create: `tests/SplatDev.Umbraco.Workflow.Persistence.Tests/SplatDev.Umbraco.Workflow.Persistence.Tests.csproj`
- Create: `tests/SplatDev.Umbraco.Workflow.E2E.Tests/SplatDev.Umbraco.Workflow.E2E.Tests.csproj`

- [ ] **Step 1: Create Core project (no external refs)**

`src/SplatDev.Umbraco.Workflow.Core/SplatDev.Umbraco.Workflow.Core.csproj`:
```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <RootNamespace>SplatDev.Umbraco.Workflow.Core</RootNamespace>
  </PropertyGroup>
</Project>
```

- [ ] **Step 2: Create Persistence project (refs Umbraco + Core)**

`src/SplatDev.Umbraco.Workflow.Persistence/SplatDev.Umbraco.Workflow.Persistence.csproj`:
```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <RootNamespace>SplatDev.Umbraco.Workflow.Persistence</RootNamespace>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="Umbraco.Cms.Infrastructure" Version="13.13.1" />
  </ItemGroup>
  <ItemGroup>
    <ProjectReference Include="..\SplatDev.Umbraco.Workflow.Core\SplatDev.Umbraco.Workflow.Core.csproj" />
  </ItemGroup>
</Project>
```

- [ ] **Step 3: Create Api project (refs Umbraco web + Persistence)**

`src/SplatDev.Umbraco.Workflow.Api/SplatDev.Umbraco.Workflow.Api.csproj`:
```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <RootNamespace>SplatDev.Umbraco.Workflow.Api</RootNamespace>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="Umbraco.Cms.Web.BackOffice" Version="13.13.1" />
    <PackageReference Include="FluentValidation.AspNetCore" Version="11.3.0" />
  </ItemGroup>
  <ItemGroup>
    <ProjectReference Include="..\SplatDev.Umbraco.Workflow.Core\SplatDev.Umbraco.Workflow.Core.csproj" />
    <ProjectReference Include="..\SplatDev.Umbraco.Workflow.Persistence\SplatDev.Umbraco.Workflow.Persistence.csproj" />
  </ItemGroup>
</Project>
```

- [ ] **Step 4: Create Backoffice project (Razor + static App_Plugins assets)**

`src/SplatDev.Umbraco.Workflow.Backoffice/SplatDev.Umbraco.Workflow.Backoffice.csproj`:
```xml
<Project Sdk="Microsoft.NET.Sdk.Razor">
  <PropertyGroup>
    <RootNamespace>SplatDev.Umbraco.Workflow.Backoffice</RootNamespace>
    <AddRazorSupportForMvc>true</AddRazorSupportForMvc>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="Umbraco.Cms.Web.BackOffice" Version="13.13.1" />
  </ItemGroup>
  <ItemGroup>
    <ProjectReference Include="..\SplatDev.Umbraco.Workflow.Api\SplatDev.Umbraco.Workflow.Api.csproj" />
  </ItemGroup>
  <ItemGroup>
    <Content Include="App_Plugins\**\*" CopyToOutputDirectory="PreserveNewest" />
  </ItemGroup>
</Project>
```

- [ ] **Step 5: Create Themes project (static content only)**

`src/SplatDev.Umbraco.Workflow.Themes/SplatDev.Umbraco.Workflow.Themes.csproj`:
```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <RootNamespace>SplatDev.Umbraco.Workflow.Themes</RootNamespace>
    <IncludeBuildOutput>false</IncludeBuildOutput>
  </PropertyGroup>
  <ItemGroup>
    <Content Include="App_Plugins\**\*" CopyToOutputDirectory="PreserveNewest" Pack="true" PackagePath="contentFiles/any/any/App_Plugins" />
  </ItemGroup>
</Project>
```

- [ ] **Step 6: Create the three test projects**

For each test project, create `tests/<Name>/<Name>.csproj`:
```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <IsPackable>false</IsPackable>
    <Nullable>enable</Nullable>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.10.0" />
    <PackageReference Include="xunit" Version="2.9.2" />
    <PackageReference Include="xunit.runner.visualstudio" Version="2.8.2" />
    <PackageReference Include="FluentAssertions" Version="6.12.2" />
  </ItemGroup>
</Project>
```
For `Core.Tests` also add `<ProjectReference Include="..\..\src\SplatDev.Umbraco.Workflow.Core\SplatDev.Umbraco.Workflow.Core.csproj" />`.
For `Persistence.Tests` add Persistence ref + `Microsoft.Data.SqlClient` 5.x.
For `E2E.Tests` add `<PackageReference Include="Microsoft.Playwright" Version="1.47.0" />` and a Playwright init step in the test base class.

- [ ] **Step 7: Add all 8 projects to the .sln**

```bash
dotnet sln add \
  src/SplatDev.Umbraco.Workflow.Core/SplatDev.Umbraco.Workflow.Core.csproj \
  src/SplatDev.Umbraco.Workflow.Persistence/SplatDev.Umbraco.Workflow.Persistence.csproj \
  src/SplatDev.Umbraco.Workflow.Api/SplatDev.Umbraco.Workflow.Api.csproj \
  src/SplatDev.Umbraco.Workflow.Backoffice/SplatDev.Umbraco.Workflow.Backoffice.csproj \
  src/SplatDev.Umbraco.Workflow.Themes/SplatDev.Umbraco.Workflow.Themes.csproj \
  tests/SplatDev.Umbraco.Workflow.Core.Tests/SplatDev.Umbraco.Workflow.Core.Tests.csproj \
  tests/SplatDev.Umbraco.Workflow.Persistence.Tests/SplatDev.Umbraco.Workflow.Persistence.Tests.csproj \
  tests/SplatDev.Umbraco.Workflow.E2E.Tests/SplatDev.Umbraco.Workflow.E2E.Tests.csproj
```

- [ ] **Step 8: Verify the solution builds**

```bash
dotnet build -c Debug --nologo
```
Expected: all 8 projects compile (they're empty; just verifying refs).

- [ ] **Step 9: Commit**

```bash
git add src/ tests/ SplatDev.Umbraco.Workflow.sln
git commit -m "chore(scaffold): create 5 src + 3 test projects, wire refs"
```

---

## Phase 2 — Core contracts and engine

### Task 3: Define the Core enums

**Files:**
- Create: `src/SplatDev.Umbraco.Workflow.Core/Enums/WorkflowStatus.cs`
- Create: `src/SplatDev.Umbraco.Workflow.Core/Enums/AssignmentStrategy.cs`
- Create: `src/SplatDev.Umbraco.Workflow.Core/Enums/ActionMessageAudience.cs`
- Create: `src/SplatDev.Umbraco.Workflow.Core/Enums/WorkflowEventType.cs`

- [ ] **Step 1: Write each enum**

`Enums/WorkflowStatus.cs`:
```csharp
namespace SplatDev.Umbraco.Workflow.Core.Enums;

/// <summary>Lifecycle status of a workflow instance.</summary>
public enum WorkflowStatus
{
    Open       = 0,
    Completed  = 1,
    Cancelled  = 2,
}
```

`Enums/AssignmentStrategy.cs`:
```csharp
namespace SplatDev.Umbraco.Workflow.Core.Enums;

/// <summary>How the next assignment for a transitioned instance is computed.</summary>
public enum AssignmentStrategy
{
    AssignToGroup     = 0,
    AssignToSubmitter = 1,
    Manual            = 2,
}
```

`Enums/ActionMessageAudience.cs`:
```csharp
namespace SplatDev.Umbraco.Workflow.Core.Enums;

/// <summary>Audience for an action-message notification intent.</summary>
public enum ActionMessageAudience
{
    Default       = 0,
    Submitter     = 1,
    AssignedGroup = 2,
    Custom        = 3,
}
```

`Enums/WorkflowEventType.cs`:
```csharp
namespace SplatDev.Umbraco.Workflow.Core.Enums;

/// <summary>Type of event recorded in the append-only workflow event log.</summary>
public enum WorkflowEventType
{
    Created       = 0,
    Transition    = 1,
    Comment       = 2,
    Assignment    = 3,
    ActionMessage = 4,
}
```

- [ ] **Step 2: Build and commit**

```bash
dotnet build src/SplatDev.Umbraco.Workflow.Core/SplatDev.Umbraco.Workflow.Core.csproj -c Debug --nologo
git add src/SplatDev.Umbraco.Workflow.Core/Enums/
git commit -m "feat(core): add workflow enums"
```

---

### Task 4: Define the Core domain models (records)

**Files:**
- Create: `src/SplatDev.Umbraco.Workflow.Core/Models/Assignment.cs`
- Create: `src/SplatDev.Umbraco.Workflow.Core/Models/WorkflowEvent.cs`
- Create: `src/SplatDev.Umbraco.Workflow.Core/Models/WorkflowQueryFilter.cs`
- Create: `src/SplatDev.Umbraco.Workflow.Core/Models/WorkflowDisplayRow.cs`
- Create: `src/SplatDev.Umbraco.Workflow.Core/Models/DisplayColumn.cs`
- Create: `src/SplatDev.Umbraco.Workflow.Core/Models/TransitionResult.cs`

- [ ] **Step 1: Write models**

`Models/Assignment.cs`:
```csharp
namespace SplatDev.Umbraco.Workflow.Core.Models;

/// <summary>An active assignment on a workflow instance.</summary>
public sealed record Assignment(
    long   InstanceId,
    string? AssignedTo,
    string? AssignedToGroup,
    string? Department,
    DateTime AssignedAt);
```

`Models/WorkflowEvent.cs`:
```csharp
using SplatDev.Umbraco.Workflow.Core.Enums;

namespace SplatDev.Umbraco.Workflow.Core.Models;

/// <summary>An event entry in the workflow's append-only history log.</summary>
public sealed record WorkflowEvent(
    long              InstanceId,
    WorkflowEventType EventType,
    string?           FromStepKey,
    string?           ToStepKey,
    string?           ActionKey,
    string?           PayloadJson,
    string            ActorUsername,
    DateTime          OccurredAt);
```

`Models/WorkflowQueryFilter.cs`:
```csharp
using SplatDev.Umbraco.Workflow.Core.Enums;

namespace SplatDev.Umbraco.Workflow.Core.Models;

/// <summary>Filter applied when querying workflow instances for a queue view.</summary>
public sealed record WorkflowQueryFilter(
    string?         WorkflowKey,
    WorkflowStatus? Status,
    bool            AssignedToMe,
    string?         Group,
    string?         Department,
    string?         FreeText,
    int             Page,
    int             PageSize);
```

`Models/DisplayColumn.cs`:
```csharp
namespace SplatDev.Umbraco.Workflow.Core.Models;

/// <summary>A column descriptor used by the queue UI.</summary>
public sealed record DisplayColumn(
    string Key,
    string Label,
    string Type,            // "string" | "date" | "number" | "badge"
    bool   IsSortable);
```

`Models/WorkflowDisplayRow.cs`:
```csharp
namespace SplatDev.Umbraco.Workflow.Core.Models;

/// <summary>A row returned to the queue UI; values keyed by DisplayColumn.Key.</summary>
public sealed record WorkflowDisplayRow(
    long                              InstanceId,
    IReadOnlyDictionary<string, object?> Values);
```

`Models/TransitionResult.cs`:
```csharp
namespace SplatDev.Umbraco.Workflow.Core.Models;

/// <summary>Result returned from IWorkflowEngine.TransitionAsync.</summary>
public sealed record TransitionResult(
    bool    Success,
    string  NewStepKey,
    string? Error);
```

- [ ] **Step 2: Build and commit**

```bash
dotnet build src/SplatDev.Umbraco.Workflow.Core/SplatDev.Umbraco.Workflow.Core.csproj -c Debug --nologo
git add src/SplatDev.Umbraco.Workflow.Core/Models/
git commit -m "feat(core): add domain models"
```

---

### Task 5: Define the Core contracts (interfaces)

**Files:** all under `src/SplatDev.Umbraco.Workflow.Core/Contracts/`

- [ ] **Step 1: Write `IWorkflowStep.cs`, `IWorkflowAction.cs`, `IActionMessage.cs`, `IWorkflow.cs`**

`Contracts/IActionMessage.cs`:
```csharp
using SplatDev.Umbraco.Workflow.Core.Enums;

namespace SplatDev.Umbraco.Workflow.Core.Contracts;

/// <summary>A named notification intent attached to a step transition.</summary>
public interface IActionMessage
{
    string                Alias    { get; }
    string                Label    { get; }
    ActionMessageAudience Audience { get; }
}
```

`Contracts/IWorkflowAction.cs`:
```csharp
using SplatDev.Umbraco.Workflow.Core.Enums;

namespace SplatDev.Umbraco.Workflow.Core.Contracts;

/// <summary>A user-facing action available from a step (button on the UI).</summary>
public interface IWorkflowAction
{
    string             Key         { get; }
    string             Label       { get; }
    string             NextStepKey { get; }
    AssignmentStrategy Assignment  { get; }
}
```

`Contracts/IWorkflowStep.cs`:
```csharp
namespace SplatDev.Umbraco.Workflow.Core.Contracts;

/// <summary>A single step inside a workflow definition.</summary>
public interface IWorkflowStep
{
    string                            Key                { get; }
    string                            Label              { get; }
    IReadOnlyList<IWorkflowAction>    Actions            { get; }
    string?                           Department         { get; }
    string?                           Group              { get; }
    IReadOnlyList<IActionMessage>     PreActionMessages  { get; }
    IReadOnlyList<IActionMessage>     PostActionMessages { get; }
}
```

`Contracts/IWorkflow.cs`:
```csharp
namespace SplatDev.Umbraco.Workflow.Core.Contracts;

/// <summary>An ordered sequence of steps that an instance progresses through.</summary>
public interface IWorkflow
{
    string                       Key     { get; }
    string                       Label   { get; }
    int                          Version { get; }
    IReadOnlyList<IWorkflowStep> Steps   { get; }
}
```

- [ ] **Step 2: Write `IWorkflowInstance.cs`**

```csharp
using SplatDev.Umbraco.Workflow.Core.Enums;

namespace SplatDev.Umbraco.Workflow.Core.Contracts;

/// <summary>A runtime instance of a workflow.</summary>
public interface IWorkflowInstance
{
    long           Id              { get; }
    string         WorkflowKey     { get; }
    int            WorkflowVersion { get; }
    string         CurrentStepKey  { get; }
    WorkflowStatus Status          { get; }
    string?        MetadataJson    { get; }
    DateTime       CreatedAt       { get; }
    string         CreatedBy       { get; }
    DateTime       UpdatedAt       { get; }
    string         UpdatedBy       { get; }
}
```

- [ ] **Step 3: Write the host extension-point contracts**

`Contracts/IWorkflowDataProvider.cs`:
```csharp
using SplatDev.Umbraco.Workflow.Core.Models;

namespace SplatDev.Umbraco.Workflow.Core.Contracts;

/// <summary>Host extension point that maps workflow instances to display data.</summary>
public interface IWorkflowDataProvider
{
    IReadOnlyList<WorkflowDisplayRow> GetDisplayRows(WorkflowQueryFilter filter, out int totalCount);
    string?                           GetSearchableValue(long instanceId, string fieldKey);
    IReadOnlyList<DisplayColumn>      GetColumns(string workflowKey);
}
```

`Contracts/IAssignmentRouter.cs`:
```csharp
using SplatDev.Umbraco.Workflow.Core.Models;

namespace SplatDev.Umbraco.Workflow.Core.Contracts;

/// <summary>Decides who/which group the next assignment goes to on a transition.</summary>
public interface IAssignmentRouter
{
    Assignment Route(IWorkflowInstance instance, IWorkflowAction action, string actorUsername);
}
```

`Contracts/IActionMessageDispatcher.cs`:
```csharp
using SplatDev.Umbraco.Workflow.Core.Models;

namespace SplatDev.Umbraco.Workflow.Core.Contracts;

/// <summary>Host-supplied transport for action-message notifications (email, Slack, etc.).</summary>
public interface IActionMessageDispatcher
{
    Task DispatchAsync(WorkflowEvent evt, CancellationToken ct);
}
```

`Contracts/IWorkflowEventStore.cs`:
```csharp
using SplatDev.Umbraco.Workflow.Core.Models;

namespace SplatDev.Umbraco.Workflow.Core.Contracts;

/// <summary>Append-only event log for a workflow instance.</summary>
public interface IWorkflowEventStore
{
    Task                        AppendAsync(WorkflowEvent evt, CancellationToken ct);
    IReadOnlyList<WorkflowEvent> GetHistory(long instanceId);
}
```

`Contracts/IWorkflowEngine.cs`:
```csharp
using SplatDev.Umbraco.Workflow.Core.Models;

namespace SplatDev.Umbraco.Workflow.Core.Contracts;

/// <summary>The state machine that drives workflow instances forward.</summary>
public interface IWorkflowEngine
{
    Task<IWorkflowInstance> CreateAsync(string workflowKey, string? metadataJson, string actorUsername, CancellationToken ct);
    Task<TransitionResult>  TransitionAsync(long instanceId, string actionKey, string actorUsername, CancellationToken ct);
}
```

- [ ] **Step 4: Build and commit**

```bash
dotnet build src/SplatDev.Umbraco.Workflow.Core/SplatDev.Umbraco.Workflow.Core.csproj -c Debug --nologo
git add src/SplatDev.Umbraco.Workflow.Core/Contracts/
git commit -m "feat(core): add public contracts (interfaces)"
```

---

### Task 6: Implement `WorkflowEngine` with TDD

**Files:**
- Test: `tests/SplatDev.Umbraco.Workflow.Core.Tests/Engine/WorkflowEngineTests.cs`
- Create: `src/SplatDev.Umbraco.Workflow.Core/Engine/WorkflowEngine.cs`

- [ ] **Step 1: Write a failing test for the simplest transition**

`tests/SplatDev.Umbraco.Workflow.Core.Tests/Engine/WorkflowEngineTests.cs`:
```csharp
using FluentAssertions;
using SplatDev.Umbraco.Workflow.Core.Contracts;
using SplatDev.Umbraco.Workflow.Core.Engine;
using SplatDev.Umbraco.Workflow.Core.Enums;
using SplatDev.Umbraco.Workflow.Core.Models;
using Xunit;

namespace SplatDev.Umbraco.Workflow.Core.Tests.Engine;

public sealed class WorkflowEngineTests
{
    [Fact]
    public async Task Transition_FromStartToNext_AdvancesCurrentStepKey()
    {
        // Arrange: a 2-step workflow with one action moving start -> next
        var workflow = TestWorkflows.TwoStep();
        var store    = new InMemoryEventStore();
        var instance = new TestInstance(Id: 1, WorkflowKey: "demo", WorkflowVersion: 1, CurrentStepKey: "start");
        var instanceStore = new InMemoryInstanceStore(instance);
        var router    = new StubRouter();
        var dispatcher = new NoopDispatcher();

        var engine = new WorkflowEngine(
            new SingleWorkflowResolver(workflow),
            instanceStore,
            store,
            router,
            dispatcher);

        // Act
        var result = await engine.TransitionAsync(1, "approve", "user@x.com", CancellationToken.None);

        // Assert
        result.Success.Should().BeTrue();
        result.NewStepKey.Should().Be("next");
        instanceStore.Get(1).CurrentStepKey.Should().Be("next");
        store.GetHistory(1).Should().ContainSingle(e => e.EventType == WorkflowEventType.Transition);
    }

    [Fact]
    public async Task Transition_WithUnknownAction_ReturnsFailureAndDoesNotMutate()
    {
        var workflow = TestWorkflows.TwoStep();
        var instance = new TestInstance(1, "demo", 1, "start");
        var instanceStore = new InMemoryInstanceStore(instance);
        var store    = new InMemoryEventStore();
        var engine = new WorkflowEngine(
            new SingleWorkflowResolver(workflow),
            instanceStore,
            store,
            new StubRouter(),
            new NoopDispatcher());

        var result = await engine.TransitionAsync(1, "nonexistent", "user@x.com", CancellationToken.None);

        result.Success.Should().BeFalse();
        result.Error.Should().Contain("nonexistent");
        instanceStore.Get(1).CurrentStepKey.Should().Be("start");
        store.GetHistory(1).Should().BeEmpty();
    }
}
```

Plus a small `TestSupport` folder under the test project for `TestWorkflows`, `TestInstance`, `InMemoryEventStore`, `InMemoryInstanceStore`, `StubRouter`, `NoopDispatcher`, `SingleWorkflowResolver`. Each is a few-line implementation of the relevant interface that the test exercises.

- [ ] **Step 2: Run the test and verify failure**

```bash
dotnet test tests/SplatDev.Umbraco.Workflow.Core.Tests/SplatDev.Umbraco.Workflow.Core.Tests.csproj
```
Expected: FAIL — `WorkflowEngine` / test helpers do not exist yet.

- [ ] **Step 3: Add a `IWorkflowResolver` and `IWorkflowInstanceStore` contract to Core**

`src/SplatDev.Umbraco.Workflow.Core/Contracts/IWorkflowResolver.cs`:
```csharp
namespace SplatDev.Umbraco.Workflow.Core.Contracts;

/// <summary>Resolves a workflow definition by key + version.</summary>
public interface IWorkflowResolver
{
    IWorkflow Resolve(string workflowKey, int version);
}
```

`src/SplatDev.Umbraco.Workflow.Core/Contracts/IWorkflowInstanceStore.cs`:
```csharp
namespace SplatDev.Umbraco.Workflow.Core.Contracts;

/// <summary>Persistence boundary for IWorkflowInstance — implemented by Persistence layer.</summary>
public interface IWorkflowInstanceStore
{
    IWorkflowInstance Get(long id);
    long              Create(string workflowKey, int workflowVersion, string startingStepKey, string? metadataJson, string actorUsername);
    void              UpdateCurrentStep(long id, string newStepKey, string actorUsername);
}
```

- [ ] **Step 4: Implement `WorkflowEngine`**

`src/SplatDev.Umbraco.Workflow.Core/Engine/WorkflowEngine.cs`:
```csharp
using System.Linq;
using SplatDev.Umbraco.Workflow.Core.Contracts;
using SplatDev.Umbraco.Workflow.Core.Enums;
using SplatDev.Umbraco.Workflow.Core.Models;

namespace SplatDev.Umbraco.Workflow.Core.Engine;

/// <summary>Default IWorkflowEngine implementation.</summary>
public sealed class WorkflowEngine(
    IWorkflowResolver         resolver,
    IWorkflowInstanceStore    instances,
    IWorkflowEventStore       events,
    IAssignmentRouter         router,
    IActionMessageDispatcher  dispatcher) : IWorkflowEngine
{
    public async Task<IWorkflowInstance> CreateAsync(string workflowKey, string? metadataJson, string actorUsername, CancellationToken ct)
    {
        // Resolve the active workflow version (resolver returns highest active version when not pinned).
        var workflow = resolver.Resolve(workflowKey, version: 0);
        if (workflow.Steps.Count == 0)
        {
            throw new InvalidOperationException($"Workflow '{workflowKey}' has no steps; cannot create instance.");
        }
        var startingStep = workflow.Steps[0];

        var id = instances.Create(workflow.Key, workflow.Version, startingStep.Key, metadataJson, actorUsername);

        var evt = new WorkflowEvent(
            InstanceId:    id,
            EventType:     WorkflowEventType.Created,
            FromStepKey:   null,
            ToStepKey:     startingStep.Key,
            ActionKey:     null,
            PayloadJson:   metadataJson,
            ActorUsername: actorUsername,
            OccurredAt:    DateTime.UtcNow);
        await events.AppendAsync(evt, ct).ConfigureAwait(false);

        return instances.Get(id);
    }

    public async Task<TransitionResult> TransitionAsync(long instanceId, string actionKey, string actorUsername, CancellationToken ct)
    {
        var instance = instances.Get(instanceId);
        var workflow = resolver.Resolve(instance.WorkflowKey, instance.WorkflowVersion);
        var currentStep = workflow.Steps.FirstOrDefault(s => s.Key == instance.CurrentStepKey)
            ?? throw new InvalidOperationException($"Current step '{instance.CurrentStepKey}' not found in workflow '{workflow.Key}' v{workflow.Version}.");

        var action = currentStep.Actions.FirstOrDefault(a => a.Key == actionKey);
        if (action is null)
        {
            return new TransitionResult(Success: false, NewStepKey: instance.CurrentStepKey,
                Error: $"Action '{actionKey}' is not valid from step '{currentStep.Key}'.");
        }

        var nextStep = workflow.Steps.FirstOrDefault(s => s.Key == action.NextStepKey)
            ?? throw new InvalidOperationException($"Target step '{action.NextStepKey}' missing from workflow.");

        instances.UpdateCurrentStep(instance.Id, nextStep.Key, actorUsername);

        var evt = new WorkflowEvent(
            InstanceId:    instance.Id,
            EventType:     WorkflowEventType.Transition,
            FromStepKey:   currentStep.Key,
            ToStepKey:     nextStep.Key,
            ActionKey:     action.Key,
            PayloadJson:   null,
            ActorUsername: actorUsername,
            OccurredAt:    DateTime.UtcNow);
        await events.AppendAsync(evt, ct).ConfigureAwait(false);

        _ = router.Route(instance, action, actorUsername);  // assignment side-effect; persisted separately in Task 9
        await dispatcher.DispatchAsync(evt, ct).ConfigureAwait(false);

        return new TransitionResult(Success: true, NewStepKey: nextStep.Key, Error: null);
    }
}
```

- [ ] **Step 5: Run tests and verify pass**

```bash
dotnet test tests/SplatDev.Umbraco.Workflow.Core.Tests/SplatDev.Umbraco.Workflow.Core.Tests.csproj
```
Expected: 2 tests PASS.

- [ ] **Step 6: Commit**

```bash
git add src/SplatDev.Umbraco.Workflow.Core/Contracts/IWorkflowResolver.cs \
        src/SplatDev.Umbraco.Workflow.Core/Contracts/IWorkflowInstanceStore.cs \
        src/SplatDev.Umbraco.Workflow.Core/Engine/WorkflowEngine.cs \
        tests/SplatDev.Umbraco.Workflow.Core.Tests/
git commit -m "feat(core): implement WorkflowEngine with TDD (transition + invalid-action)"
```

---

## Phase 3 — Persistence

### Task 7: Define NPoco entities

**Files:** all under `src/SplatDev.Umbraco.Workflow.Persistence/Entities/`

- [ ] **Step 1: Write `WorkflowDefinitionEntity.cs`**

```csharp
using NPoco;
using Umbraco.Cms.Infrastructure.Persistence.DatabaseAnnotations;

namespace SplatDev.Umbraco.Workflow.Persistence.Entities;

[TableName(TableName)]
[PrimaryKey(nameof(Id))]
[ExplicitColumns]
internal sealed class WorkflowDefinitionEntity
{
    public const string TableName = "splatWorkflowDefinition";

    [Column("id")]
    [PrimaryKeyColumn(AutoIncrement = true)]
    public int Id { get; set; }

    [Column("key")] public string Key { get; set; } = string.Empty;
    [Column("label")] public string Label { get; set; } = string.Empty;
    [Column("version")] public int Version { get; set; }
    [Column("definitionJson")]
    [SpecialDbType(SpecialDbTypes.NVARCHARMAX)]
    public string DefinitionJson { get; set; } = string.Empty;
    [Column("isActive")] public bool IsActive { get; set; }
    [Column("createdAt")] public DateTime CreatedAt { get; set; }
    [Column("createdBy")] public string CreatedBy { get; set; } = string.Empty;
}
```

- [ ] **Step 2: Write `WorkflowInstanceEntity.cs`**

```csharp
using NPoco;
using SplatDev.Umbraco.Workflow.Core.Contracts;
using SplatDev.Umbraco.Workflow.Core.Enums;
using Umbraco.Cms.Infrastructure.Persistence.DatabaseAnnotations;

namespace SplatDev.Umbraco.Workflow.Persistence.Entities;

[TableName(TableName)]
[PrimaryKey(nameof(Id))]
[ExplicitColumns]
internal sealed class WorkflowInstanceEntity : IWorkflowInstance
{
    public const string TableName = "splatWorkflowInstance";

    [Column("id")][PrimaryKeyColumn(AutoIncrement = true)] public long Id { get; set; }
    [Column("workflowKey")]     public string WorkflowKey { get; set; } = string.Empty;
    [Column("workflowVersion")] public int    WorkflowVersion { get; set; }
    [Column("currentStepKey")]  public string CurrentStepKey { get; set; } = string.Empty;
    [Column("status")]          public WorkflowStatus Status { get; set; }
    [Column("metadataJson")]
    [NullSetting(NullSetting = NullSettings.Null)]
    [SpecialDbType(SpecialDbTypes.NVARCHARMAX)]
    public string? MetadataJson { get; set; }
    [Column("createdAt")] public DateTime CreatedAt { get; set; }
    [Column("createdBy")] public string CreatedBy { get; set; } = string.Empty;
    [Column("updatedAt")] public DateTime UpdatedAt { get; set; }
    [Column("updatedBy")] public string UpdatedBy { get; set; } = string.Empty;
}
```

- [ ] **Step 3: Write `WorkflowEventEntity.cs`, `WorkflowAssignmentEntity.cs`, `WorkflowTaskEntity.cs`** (same pattern — see spec section 3 for columns).

- [ ] **Step 4: Build and commit**

```bash
dotnet build src/SplatDev.Umbraco.Workflow.Persistence/SplatDev.Umbraco.Workflow.Persistence.csproj -c Debug --nologo
git add src/SplatDev.Umbraco.Workflow.Persistence/Entities/
git commit -m "feat(persistence): NPoco entity types"
```

---

### Task 8: Write FluentMigrator migration M001

**Files:**
- Create: `src/SplatDev.Umbraco.Workflow.Persistence/Migrations/_Initial/M001_CreateSchema.cs`
- Create: `src/SplatDev.Umbraco.Workflow.Persistence/Migrations/MigrationPlan.cs`

- [ ] **Step 1: Write `M001_CreateSchema`**

```csharp
using Umbraco.Cms.Infrastructure.Migrations;

namespace SplatDev.Umbraco.Workflow.Persistence.Migrations.Initial;

/// <summary>Creates the 5 splatWorkflow* tables.</summary>
public sealed class M001_CreateSchema(IMigrationContext context) : MigrationBase(context)
{
    protected override void Migrate()
    {
        // splatWorkflowDefinition
        Create.Table("splatWorkflowDefinition")
            .WithColumn("id").AsInt32().Identity().PrimaryKey()
            .WithColumn("key").AsString(64).NotNullable()
            .WithColumn("label").AsString(256).NotNullable()
            .WithColumn("version").AsInt32().NotNullable()
            .WithColumn("definitionJson").AsCustom("NVARCHAR(MAX)").NotNullable()
            .WithColumn("isActive").AsBoolean().NotNullable()
            .WithColumn("createdAt").AsDateTime().NotNullable()
            .WithColumn("createdBy").AsString(256).NotNullable();
        Create.UniqueConstraint("UQ_splatWorkflowDefinition_key_version")
            .OnTable("splatWorkflowDefinition").Columns("key", "version");

        // splatWorkflowInstance
        Create.Table("splatWorkflowInstance")
            .WithColumn("id").AsInt64().Identity().PrimaryKey()
            .WithColumn("workflowKey").AsString(64).NotNullable()
            .WithColumn("workflowVersion").AsInt32().NotNullable()
            .WithColumn("currentStepKey").AsString(64).NotNullable()
            .WithColumn("status").AsByte().NotNullable()
            .WithColumn("metadataJson").AsCustom("NVARCHAR(MAX)").Nullable()
            .WithColumn("createdAt").AsDateTime().NotNullable()
            .WithColumn("createdBy").AsString(256).NotNullable()
            .WithColumn("updatedAt").AsDateTime().NotNullable()
            .WithColumn("updatedBy").AsString(256).NotNullable();
        Create.Index("IX_splatWorkflowInstance_workflow")
            .OnTable("splatWorkflowInstance")
            .OnColumn("workflowKey").Ascending()
            .OnColumn("status").Ascending()
            .OnColumn("currentStepKey").Ascending();

        // splatWorkflowEvent
        Create.Table("splatWorkflowEvent")
            .WithColumn("id").AsInt64().Identity().PrimaryKey()
            .WithColumn("instanceId").AsInt64().NotNullable()
            .WithColumn("eventType").AsByte().NotNullable()
            .WithColumn("fromStepKey").AsString(64).Nullable()
            .WithColumn("toStepKey").AsString(64).Nullable()
            .WithColumn("actionKey").AsString(64).Nullable()
            .WithColumn("payloadJson").AsCustom("NVARCHAR(MAX)").Nullable()
            .WithColumn("actorUsername").AsString(256).NotNullable()
            .WithColumn("occurredAt").AsDateTime().NotNullable();
        Create.Index("IX_splatWorkflowEvent_instance")
            .OnTable("splatWorkflowEvent")
            .OnColumn("instanceId").Ascending()
            .OnColumn("occurredAt").Ascending();

        // splatWorkflowAssignment
        Create.Table("splatWorkflowAssignment")
            .WithColumn("id").AsInt64().Identity().PrimaryKey()
            .WithColumn("instanceId").AsInt64().NotNullable()
            .WithColumn("assignedTo").AsString(256).Nullable()
            .WithColumn("assignedToGroup").AsString(64).Nullable()
            .WithColumn("department").AsString(64).Nullable()
            .WithColumn("assignedAt").AsDateTime().NotNullable()
            .WithColumn("isActive").AsBoolean().NotNullable();
        Create.Index("IX_splatWorkflowAssignment_active")
            .OnTable("splatWorkflowAssignment")
            .OnColumn("isActive").Ascending()
            .OnColumn("instanceId").Ascending();

        // splatWorkflowTask
        Create.Table("splatWorkflowTask")
            .WithColumn("id").AsInt64().Identity().PrimaryKey()
            .WithColumn("instanceId").AsInt64().NotNullable()
            .WithColumn("alias").AsString(64).NotNullable()
            .WithColumn("name").AsString(256).NotNullable()
            .WithColumn("description").AsCustom("NVARCHAR(MAX)").Nullable()
            .WithColumn("isCompleted").AsBoolean().NotNullable()
            .WithColumn("completedAt").AsDateTime().Nullable()
            .WithColumn("completedBy").AsString(256).Nullable()
            .WithColumn("departmentId").AsInt32().Nullable();
        Create.Index("IX_splatWorkflowTask_instance")
            .OnTable("splatWorkflowTask")
            .OnColumn("instanceId").Ascending();
    }
}
```

- [ ] **Step 2: Write `MigrationPlan.cs`**

```csharp
using Umbraco.Cms.Infrastructure.Migrations;
using SplatDev.Umbraco.Workflow.Persistence.Migrations.Initial;

namespace SplatDev.Umbraco.Workflow.Persistence.Migrations;

/// <summary>Migration plan registered at startup so the schema is applied automatically.</summary>
public sealed class SplatWorkflowMigrationPlan : MigrationPlan
{
    public SplatWorkflowMigrationPlan() : base("SplatDev.Workflow")
    {
        From(string.Empty)
            .To<M001_CreateSchema>("m001-create-schema");
    }
}
```

- [ ] **Step 3: Build and commit**

```bash
dotnet build src/SplatDev.Umbraco.Workflow.Persistence/SplatDev.Umbraco.Workflow.Persistence.csproj -c Debug --nologo
git add src/SplatDev.Umbraco.Workflow.Persistence/Migrations/
git commit -m "feat(persistence): M001 schema migration + plan"
```

---

### Task 9: Implement repositories (instance, event store, assignment, task, definition)

**Files:** under `src/SplatDev.Umbraco.Workflow.Persistence/Repositories/`

- [ ] **Step 1: Write `WorkflowInstanceRepository.cs` — implements `IWorkflowInstanceStore`**

```csharp
using SplatDev.Umbraco.Workflow.Core.Contracts;
using SplatDev.Umbraco.Workflow.Persistence.Entities;
using Umbraco.Cms.Infrastructure.Scoping;

namespace SplatDev.Umbraco.Workflow.Persistence.Repositories;

internal sealed class WorkflowInstanceRepository(IScopeProvider scopeProvider) : IWorkflowInstanceStore
{
    public IWorkflowInstance Get(long id)
    {
        using var scope = scopeProvider.CreateScope(autoComplete: true);
        var entity = scope.Database.SingleOrDefault<WorkflowInstanceEntity>(
            "WHERE id = @0", id)
            ?? throw new InvalidOperationException($"Workflow instance {id} not found.");
        return entity;
    }

    public void UpdateCurrentStep(long id, string newStepKey, string actorUsername)
    {
        using var scope = scopeProvider.CreateScope();
        scope.Database.Execute(
            "UPDATE splatWorkflowInstance SET currentStepKey = @0, updatedAt = @1, updatedBy = @2 WHERE id = @3",
            newStepKey, DateTime.UtcNow, actorUsername, id);
        scope.Complete();
    }

    public long Create(string workflowKey, int workflowVersion, string startingStepKey, string? metadataJson, string actorUsername)
    {
        using var scope = scopeProvider.CreateScope();
        var now = DateTime.UtcNow;
        var entity = new WorkflowInstanceEntity
        {
            WorkflowKey     = workflowKey,
            WorkflowVersion = workflowVersion,
            CurrentStepKey  = startingStepKey,
            Status          = Core.Enums.WorkflowStatus.Open,
            MetadataJson    = metadataJson,
            CreatedAt       = now,
            CreatedBy       = actorUsername,
            UpdatedAt       = now,
            UpdatedBy       = actorUsername,
        };
        var inserted = scope.Database.Insert(entity);
        scope.Complete();
        return Convert.ToInt64(inserted);
    }
}
```

- [ ] **Step 2: Write `WorkflowEventRepository.cs` — implements `IWorkflowEventStore`**

```csharp
using SplatDev.Umbraco.Workflow.Core.Contracts;
using SplatDev.Umbraco.Workflow.Core.Models;
using SplatDev.Umbraco.Workflow.Persistence.Entities;
using Umbraco.Cms.Infrastructure.Scoping;

namespace SplatDev.Umbraco.Workflow.Persistence.Repositories;

internal sealed class WorkflowEventRepository(IScopeProvider scopeProvider) : IWorkflowEventStore
{
    public async Task AppendAsync(WorkflowEvent evt, CancellationToken ct)
    {
        using var scope = scopeProvider.CreateScope();
        scope.Database.Insert(new WorkflowEventEntity
        {
            InstanceId    = evt.InstanceId,
            EventType     = (byte)evt.EventType,
            FromStepKey   = evt.FromStepKey,
            ToStepKey     = evt.ToStepKey,
            ActionKey     = evt.ActionKey,
            PayloadJson   = evt.PayloadJson,
            ActorUsername = evt.ActorUsername,
            OccurredAt    = evt.OccurredAt,
        });
        scope.Complete();
        await Task.CompletedTask.ConfigureAwait(false);
    }

    public IReadOnlyList<WorkflowEvent> GetHistory(long instanceId)
    {
        using var scope = scopeProvider.CreateScope(autoComplete: true);
        var rows = scope.Database.Fetch<WorkflowEventEntity>(
            "WHERE instanceId = @0 ORDER BY occurredAt ASC", instanceId);
        return rows.Select(r => new WorkflowEvent(
            r.InstanceId, (Core.Enums.WorkflowEventType)r.EventType,
            r.FromStepKey, r.ToStepKey, r.ActionKey, r.PayloadJson,
            r.ActorUsername, r.OccurredAt)).ToList();
    }
}
```

- [ ] **Step 3: Write `WorkflowDefinitionRepository.cs`, `WorkflowAssignmentRepository.cs`, `WorkflowTaskRepository.cs`** following the same NPoco-fluent pattern. Each provides Get/List/Add/Update/Delete methods used by the API layer.

- [ ] **Step 4: Commit**

```bash
git add src/SplatDev.Umbraco.Workflow.Persistence/Repositories/
git commit -m "feat(persistence): repositories — instance, event, definition, assignment, task"
```

---

### Task 10: Migration tests

**Files:**
- Create: `tests/SplatDev.Umbraco.Workflow.Persistence.Tests/MigrationTests.cs`

- [ ] **Step 1: Write a test that applies M001 to a fresh LocalDB and asserts the tables exist**

```csharp
using FluentAssertions;
using Microsoft.Data.SqlClient;
using Xunit;

namespace SplatDev.Umbraco.Workflow.Persistence.Tests;

public sealed class MigrationTests
{
    [Fact]
    public async Task M001_CreatesAllTablesOnFreshDatabase()
    {
        var dbName = $"SplatWorkflowTest_{Guid.NewGuid():N}";
        var master = "Server=(localdb)\\MSSQLLocalDB;Database=master;Integrated Security=true;TrustServerCertificate=true";
        var target = $"Server=(localdb)\\MSSQLLocalDB;Database={dbName};Integrated Security=true;TrustServerCertificate=true";

        // create the test DB
        await using (var c = new SqlConnection(master))
        {
            await c.OpenAsync();
            await using var cmd = c.CreateCommand();
            cmd.CommandText = $"CREATE DATABASE [{dbName}]";
            await cmd.ExecuteNonQueryAsync();
        }

        try
        {
            // run the migration via the Umbraco MigrationContext harness (helper to be added)
            await MigrationTestHarness.RunAsync(target);

            // verify
            await using var c = new SqlConnection(target);
            await c.OpenAsync();
            var tables = new[] { "splatWorkflowDefinition", "splatWorkflowInstance", "splatWorkflowEvent", "splatWorkflowAssignment", "splatWorkflowTask" };
            foreach (var name in tables)
            {
                await using var cmd = c.CreateCommand();
                cmd.CommandText = "SELECT COUNT(*) FROM sys.tables WHERE name = @n";
                cmd.Parameters.AddWithValue("@n", name);
                var found = (int)(await cmd.ExecuteScalarAsync())!;
                found.Should().Be(1, $"table {name} must exist after M001");
            }
        }
        finally
        {
            await using var c = new SqlConnection(master);
            await c.OpenAsync();
            await using var cmd = c.CreateCommand();
            cmd.CommandText = $"DROP DATABASE [{dbName}]";
            await cmd.ExecuteNonQueryAsync();
        }
    }
}
```

(A small `MigrationTestHarness` helper wires up the minimum Umbraco services to run a `MigrationPlanExecutor` against a connection string — implemented alongside this test.)

- [ ] **Step 2: Run the test and verify pass**

```bash
dotnet test tests/SplatDev.Umbraco.Workflow.Persistence.Tests/SplatDev.Umbraco.Workflow.Persistence.Tests.csproj
```
Expected: 1 test PASS. If LocalDB is missing on the dev machine, the test should `Skip` with a clear message.

- [ ] **Step 3: Commit**

```bash
git add tests/SplatDev.Umbraco.Workflow.Persistence.Tests/
git commit -m "test(persistence): M001 applies cleanly to fresh LocalDB"
```

---

### Task 11: JsonMetadataDataProvider + DefaultAssignmentRouter

**Files:**
- Create: `src/SplatDev.Umbraco.Workflow.Persistence/Providers/JsonMetadataDataProvider.cs`
- Create: `src/SplatDev.Umbraco.Workflow.Persistence/Routing/DefaultAssignmentRouter.cs`

- [ ] **Step 1: Write a small `JsonMetadataProviderOptions` record**

`src/SplatDev.Umbraco.Workflow.Core/Models/JsonMetadataProviderOptions.cs`:
```csharp
namespace SplatDev.Umbraco.Workflow.Core.Models;

/// <summary>Host-supplied configuration for the JSON metadata data provider.</summary>
public sealed record JsonMetadataProviderOptions(
    IReadOnlyDictionary<string, IReadOnlyList<DisplayColumn>> ColumnsByWorkflow,
    IReadOnlyList<string>                                     SearchableFieldKeys);
```

- [ ] **Step 2: Implement `JsonMetadataDataProvider`**

```csharp
using System.Text.Json;
using SplatDev.Umbraco.Workflow.Core.Contracts;
using SplatDev.Umbraco.Workflow.Core.Models;
using SplatDev.Umbraco.Workflow.Persistence.Entities;
using Umbraco.Cms.Infrastructure.Scoping;

namespace SplatDev.Umbraco.Workflow.Persistence.Providers;

/// <summary>Default IWorkflowDataProvider: reads display fields out of the instance's metadataJson.</summary>
public sealed class JsonMetadataDataProvider(
    IScopeProvider                  scopeProvider,
    JsonMetadataProviderOptions     options) : IWorkflowDataProvider
{
    public IReadOnlyList<WorkflowDisplayRow> GetDisplayRows(WorkflowQueryFilter filter, out int totalCount)
    {
        using var scope = scopeProvider.CreateScope(autoComplete: true);
        var sql = "SELECT * FROM splatWorkflowInstance WHERE 1=1";
        var args = new List<object>();
        if (filter.WorkflowKey is not null) { sql += " AND workflowKey = @" + args.Count; args.Add(filter.WorkflowKey); }
        if (filter.Status      is not null) { sql += " AND status = @"      + args.Count; args.Add((byte)filter.Status.Value); }

        totalCount = scope.Database.ExecuteScalar<int>("SELECT COUNT(*) " + sql.Substring(sql.IndexOf("FROM")), args.ToArray());
        var pageRows = scope.Database.Fetch<WorkflowInstanceEntity>(
            sql + " ORDER BY updatedAt DESC OFFSET @p SKIPROWS FETCH NEXT @ps ROWS ONLY"
                .Replace("@p", "@" + args.Count)
                .Replace("@ps", "@" + (args.Count + 1))
                .Replace("SKIPROWS", "ROWS"),
            args.Concat(new object[] { (filter.Page - 1) * filter.PageSize, filter.PageSize }).ToArray());

        if (!options.ColumnsByWorkflow.TryGetValue(filter.WorkflowKey ?? "", out var columns))
        {
            columns = new[] { new DisplayColumn("id", "ID", "number", true) };
        }
        return pageRows.Select(r => BuildRow(r, columns)).ToList();
    }

    public string? GetSearchableValue(long instanceId, string fieldKey)
    {
        using var scope = scopeProvider.CreateScope(autoComplete: true);
        var json = scope.Database.ExecuteScalar<string?>("SELECT metadataJson FROM splatWorkflowInstance WHERE id = @0", instanceId);
        if (string.IsNullOrWhiteSpace(json)) return null;
        using var doc = JsonDocument.Parse(json);
        return doc.RootElement.TryGetProperty(fieldKey, out var v) ? v.ToString() : null;
    }

    public IReadOnlyList<DisplayColumn> GetColumns(string workflowKey)
        => options.ColumnsByWorkflow.TryGetValue(workflowKey, out var cols)
            ? cols
            : Array.Empty<DisplayColumn>();

    private static WorkflowDisplayRow BuildRow(WorkflowInstanceEntity e, IReadOnlyList<DisplayColumn> cols)
    {
        var values = new Dictionary<string, object?>(cols.Count);
        if (e.MetadataJson is not null)
        {
            using var doc = JsonDocument.Parse(e.MetadataJson);
            foreach (var col in cols)
            {
                values[col.Key] = doc.RootElement.TryGetProperty(col.Key, out var v) ? v.ToString() : null;
            }
        }
        return new WorkflowDisplayRow(e.Id, values);
    }
}
```

- [ ] **Step 3: Implement `DefaultAssignmentRouter`**

```csharp
using SplatDev.Umbraco.Workflow.Core.Contracts;
using SplatDev.Umbraco.Workflow.Core.Enums;
using SplatDev.Umbraco.Workflow.Core.Models;

namespace SplatDev.Umbraco.Workflow.Persistence.Routing;

/// <summary>Routes assignments based on the action's AssignmentStrategy.</summary>
public sealed class DefaultAssignmentRouter : IAssignmentRouter
{
    public Assignment Route(IWorkflowInstance instance, IWorkflowAction action, string actorUsername)
    {
        return action.Assignment switch
        {
            AssignmentStrategy.AssignToSubmitter => new Assignment(instance.Id, actorUsername, null, null, DateTime.UtcNow),
            AssignmentStrategy.AssignToGroup     => new Assignment(instance.Id, null, GroupForNextStep(instance, action), null, DateTime.UtcNow),
            AssignmentStrategy.Manual            => new Assignment(instance.Id, null, null, null, DateTime.UtcNow),
            _                                    => throw new InvalidOperationException($"Unknown AssignmentStrategy: {action.Assignment}"),
        };
    }

    private static string? GroupForNextStep(IWorkflowInstance instance, IWorkflowAction action)
    {
        // The group attached to the *target* step. The router does not load the workflow itself —
        // the engine passes the resolved next step's group through a future overload. For v1, the
        // router uses the action's NextStepKey + the workflow resolver via DI (added in Task 12).
        return null;
    }
}
```

(Task 12 wires the workflow resolver into the router; the v1-minimal router returns null group for now and will be updated.)

- [ ] **Step 4: Commit**

```bash
git add src/SplatDev.Umbraco.Workflow.Core/Models/JsonMetadataProviderOptions.cs \
        src/SplatDev.Umbraco.Workflow.Persistence/Providers/JsonMetadataDataProvider.cs \
        src/SplatDev.Umbraco.Workflow.Persistence/Routing/DefaultAssignmentRouter.cs
git commit -m "feat(persistence): JSON-bag data provider + default assignment router"
```

---

## Phase 4 — API layer

### Task 12: Composer + DI wiring (`AddSplatDevWorkflow()`)

**Files:**
- Create: `src/SplatDev.Umbraco.Workflow.Api/Composition/SplatDevWorkflowComposer.cs`
- Create: `src/SplatDev.Umbraco.Workflow.Api/Composition/ServiceCollectionExtensions.cs`

- [ ] **Step 1: Write the composer that registers all services and the migration plan**

`Composition/SplatDevWorkflowComposer.cs`:
```csharp
using Microsoft.Extensions.DependencyInjection;
using SplatDev.Umbraco.Workflow.Core.Contracts;
using SplatDev.Umbraco.Workflow.Core.Engine;
using SplatDev.Umbraco.Workflow.Persistence.Migrations;
using SplatDev.Umbraco.Workflow.Persistence.Providers;
using SplatDev.Umbraco.Workflow.Persistence.Repositories;
using SplatDev.Umbraco.Workflow.Persistence.Routing;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.Notifications;

namespace SplatDev.Umbraco.Workflow.Api.Composition;

public sealed class SplatDevWorkflowComposer : IComposer
{
    public void Compose(IUmbracoBuilder builder)
    {
        builder.Services.AddScoped<IWorkflowInstanceStore, WorkflowInstanceRepository>();
        builder.Services.AddScoped<IWorkflowEventStore,    WorkflowEventRepository>();
        builder.Services.AddScoped<IAssignmentRouter,      DefaultAssignmentRouter>();
        builder.Services.AddScoped<IWorkflowEngine,        WorkflowEngine>();

        // Default no-op dispatcher unless the host overrides it.
        builder.Services.AddScoped<IActionMessageDispatcher, NullActionMessageDispatcher>();

        // Apply the migration on first boot.
        builder.AddNotificationHandler<UmbracoApplicationStartingNotification, RunWorkflowMigrationHandler>();
    }
}

internal sealed class NullActionMessageDispatcher : IActionMessageDispatcher
{
    public Task DispatchAsync(Core.Models.WorkflowEvent evt, CancellationToken ct) => Task.CompletedTask;
}

/// <summary>Applies the SplatWorkflow migration plan on Umbraco application start.</summary>
internal sealed class RunWorkflowMigrationHandler(
    Umbraco.Cms.Infrastructure.Migrations.Upgrade.IMigrationPlanExecutor executor,
    Umbraco.Cms.Infrastructure.Scoping.IScopeProvider                    scopeProvider,
    Umbraco.Cms.Core.Services.IKeyValueService                           keyValueService) : INotificationHandler<UmbracoApplicationStartingNotification>
{
    public void Handle(UmbracoApplicationStartingNotification notification)
    {
        if (notification.RuntimeLevel < Umbraco.Cms.Core.RuntimeLevel.Run) return;
        var plan = new SplatWorkflowMigrationPlan();
        var upgrader = new Umbraco.Cms.Infrastructure.Migrations.Upgrade.Upgrader(plan);
        upgrader.Execute(executor, scopeProvider, keyValueService);
    }
}
```

`Composition/ServiceCollectionExtensions.cs`:
```csharp
using Microsoft.Extensions.DependencyInjection;
using SplatDev.Umbraco.Workflow.Core.Contracts;
using SplatDev.Umbraco.Workflow.Core.Models;
using SplatDev.Umbraco.Workflow.Persistence.Providers;

namespace SplatDev.Umbraco.Workflow.Api.Composition;

public static class ServiceCollectionExtensions
{
    /// <summary>Configures the JSON metadata data provider with host-supplied columns.</summary>
    public static IServiceCollection AddSplatDevWorkflowJsonProvider(
        this IServiceCollection services,
        Action<JsonMetadataProviderOptionsBuilder> configure)
    {
        var b = new JsonMetadataProviderOptionsBuilder();
        configure(b);
        services.AddSingleton(b.Build());
        services.AddScoped<IWorkflowDataProvider, JsonMetadataDataProvider>();
        return services;
    }
}

public sealed class JsonMetadataProviderOptionsBuilder
{
    private readonly Dictionary<string, IReadOnlyList<DisplayColumn>> _columns = new();
    private readonly List<string> _searchable = new();

    public JsonMetadataProviderOptionsBuilder Columns(string workflowKey, params DisplayColumn[] cols)
    {
        _columns[workflowKey] = cols;
        return this;
    }
    public JsonMetadataProviderOptionsBuilder Searchable(params string[] fieldKeys)
    {
        _searchable.AddRange(fieldKeys);
        return this;
    }
    public JsonMetadataProviderOptions Build() => new(_columns, _searchable);
}
```

(`RunWorkflowMigrationHandler` is a tiny `INotificationHandler<UmbracoApplicationStartingNotification>` that calls `MigrationPlanExecutor.Execute(new SplatWorkflowMigrationPlan())`.)

- [ ] **Step 2: Build and commit**

```bash
dotnet build src/SplatDev.Umbraco.Workflow.Api/SplatDev.Umbraco.Workflow.Api.csproj -c Debug --nologo
git add src/SplatDev.Umbraco.Workflow.Api/Composition/
git commit -m "feat(api): composer + AddSplatDevWorkflow extension"
```

---

### Task 13: API DTOs and validators

**Files:** under `src/SplatDev.Umbraco.Workflow.Api/Contracts/` and `src/SplatDev.Umbraco.Workflow.Api/Validators/`

- [ ] **Step 1: Write `WorkflowDefinitionDto.cs`, `WorkflowInstanceDto.cs`, `CreateInstanceRequest.cs`, `TransitionRequest.cs`, `SetTaskCompletionRequest.cs`, `PagedResult.cs`**

```csharp
namespace SplatDev.Umbraco.Workflow.Api.Contracts;

public sealed record WorkflowDefinitionDto(
    string Key, string Label, int Version, bool IsActive, string DefinitionJson, DateTime CreatedAt);

public sealed record WorkflowInstanceDto(
    long Id, string WorkflowKey, int WorkflowVersion, string CurrentStepKey,
    int Status, string? MetadataJson, DateTime CreatedAt, string CreatedBy,
    DateTime UpdatedAt, string UpdatedBy);

public sealed record CreateInstanceRequest(string WorkflowKey, string? MetadataJson);

public sealed record TransitionRequest(string ActionKey);

public sealed record SetTaskCompletionRequest(IReadOnlyList<TaskCompletionEntry> Entries);
public sealed record TaskCompletionEntry(long TaskId, bool IsCompleted);

public sealed record PagedResult<T>(IReadOnlyList<T> Items, int TotalCount, int Page, int PageSize);
```

- [ ] **Step 2: Write validators**

```csharp
using FluentValidation;
using SplatDev.Umbraco.Workflow.Api.Contracts;

namespace SplatDev.Umbraco.Workflow.Api.Validators;

public sealed class TransitionRequestValidator : AbstractValidator<TransitionRequest>
{
    public TransitionRequestValidator()
    {
        RuleFor(r => r.ActionKey).NotEmpty().MaximumLength(64);
    }
}

public sealed class CreateInstanceRequestValidator : AbstractValidator<CreateInstanceRequest>
{
    public CreateInstanceRequestValidator()
    {
        RuleFor(r => r.WorkflowKey).NotEmpty().MaximumLength(64);
    }
}
```

- [ ] **Step 3: Commit**

```bash
git add src/SplatDev.Umbraco.Workflow.Api/Contracts/ src/SplatDev.Umbraco.Workflow.Api/Validators/
git commit -m "feat(api): DTOs + FluentValidation validators"
```

---

### Task 14: API Controllers

**Files:** under `src/SplatDev.Umbraco.Workflow.Api/Controllers/`

- [ ] **Step 1: Write `WorkflowInstancesController`**

```csharp
using Microsoft.AspNetCore.Mvc;
using SplatDev.Umbraco.Workflow.Api.Contracts;
using SplatDev.Umbraco.Workflow.Core.Contracts;
using SplatDev.Umbraco.Workflow.Core.Enums;
using SplatDev.Umbraco.Workflow.Core.Models;
using Umbraco.Cms.Web.BackOffice.Controllers;
using Umbraco.Cms.Web.Common.Attributes;

namespace SplatDev.Umbraco.Workflow.Api.Controllers;

[PluginController("SplatDevWorkflow")]
public sealed class WorkflowInstancesController(
    IWorkflowEngine        engine,
    IWorkflowInstanceStore instances,
    IWorkflowDataProvider  provider) : UmbracoAuthorizedApiController
{
    [HttpGet]
    public IActionResult List(
        [FromQuery] string?         workflowKey = null,
        [FromQuery] WorkflowStatus? status      = null,
        [FromQuery] bool            assignedToMe = false,
        [FromQuery] string?         group       = null,
        [FromQuery] string?         department  = null,
        [FromQuery] string?         freeText    = null,
        [FromQuery] int             page        = 1,
        [FromQuery] int             pageSize    = 50)
    {
        var filter = new WorkflowQueryFilter(workflowKey, status, assignedToMe, group, department, freeText, page, pageSize);
        var rows = provider.GetDisplayRows(filter, out var total);
        return Ok(new PagedResult<WorkflowDisplayRow>(rows, total, page, pageSize));
    }

    [HttpGet("{id:long}")]
    public IActionResult Get(long id)
    {
        var instance = instances.Get(id);
        return Ok(new WorkflowInstanceDto(
            instance.Id, instance.WorkflowKey, instance.WorkflowVersion, instance.CurrentStepKey,
            (int)instance.Status, instance.MetadataJson, instance.CreatedAt, instance.CreatedBy,
            instance.UpdatedAt, instance.UpdatedBy));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateInstanceRequest req, CancellationToken ct)
    {
        var actor = User.Identity?.Name ?? "anonymous";
        var instance = await engine.CreateAsync(req.WorkflowKey, req.MetadataJson, actor, ct).ConfigureAwait(false);
        return CreatedAtAction(nameof(Get), new { id = instance.Id }, instance);
    }

    [HttpPost("{id:long}/transition")]
    public async Task<IActionResult> Transition(long id, [FromBody] TransitionRequest req, CancellationToken ct)
    {
        var actor = User.Identity?.Name ?? "anonymous";
        var result = await engine.TransitionAsync(id, req.ActionKey, actor, ct).ConfigureAwait(false);
        if (!result.Success) return BadRequest(new ProblemDetails { Title = "Transition failed", Detail = result.Error });
        return Ok(result);
    }
}
```

- [ ] **Step 2: Write `WorkflowDefinitionsController`, `WorkflowTasksController`, `WorkflowThemesController`** following the same `[PluginController("SplatDevWorkflow")] : UmbracoAuthorizedApiController` pattern, one HTTP verb per endpoint listed in spec §4.

- [ ] **Step 3: Build and commit**

```bash
dotnet build src/SplatDev.Umbraco.Workflow.Api/SplatDev.Umbraco.Workflow.Api.csproj -c Debug --nologo
git add src/SplatDev.Umbraco.Workflow.Api/Controllers/
git commit -m "feat(api): 4 controllers — definitions, instances, tasks, themes"
```

---

## Phase 5 — Backoffice (AngularJS App_Plugin)

### Task 15: package.manifest, section, and trees

**Files:**
- Create: `src/SplatDev.Umbraco.Workflow.Backoffice/App_Plugins/SplatDev.Workflow/package.manifest`
- Create: `src/SplatDev.Umbraco.Workflow.Backoffice/Sections/WorkflowSection.cs`
- Create: `src/SplatDev.Umbraco.Workflow.Backoffice/Trees/QueuesTreeController.cs`
- Create: `src/SplatDev.Umbraco.Workflow.Backoffice/Trees/DefinitionsTreeController.cs`
- Create: `src/SplatDev.Umbraco.Workflow.Backoffice/Trees/ThemesTreeController.cs`

- [ ] **Step 1: Write `package.manifest`**

```json
{
  "name": "SplatDev Workflow",
  "language": ["en-US", "es-ES"],
  "javascript": [
    "~/App_Plugins/SplatDev.Workflow/services/workflowResource.js",
    "~/App_Plugins/SplatDev.Workflow/services/themeService.js",
    "~/App_Plugins/SplatDev.Workflow/directives/pizzaChart.directive.js",
    "~/App_Plugins/SplatDev.Workflow/directives/queueTable.directive.js",
    "~/App_Plugins/SplatDev.Workflow/dashboards/queue.controller.js",
    "~/App_Plugins/SplatDev.Workflow/dashboards/definitionEditor.controller.js",
    "~/App_Plugins/SplatDev.Workflow/dashboards/themes.controller.js"
  ],
  "css": [
    "~/App_Plugins/SplatDev.Workflow/styles/base.css"
  ]
}
```

- [ ] **Step 2: Write the section registration**

```csharp
using Umbraco.Cms.Core.Sections;

namespace SplatDev.Umbraco.Workflow.Backoffice.Sections;

/// <summary>Registers the "Workflow" backoffice section.</summary>
public sealed class WorkflowSection : ISection
{
    public string Alias => "workflow";
    public string Name  => "Workflow";
}
```

And a composer:
```csharp
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.Sections;

namespace SplatDev.Umbraco.Workflow.Backoffice.Sections;

public sealed class WorkflowSectionComposer : IComposer
{
    public void Compose(IUmbracoBuilder builder)
        => builder.Sections().Append<WorkflowSection>();
}
```

- [ ] **Step 3: Write the three tree controllers**

Each derives from `Umbraco.Cms.Web.BackOffice.Trees.TreeController` with `[Tree("workflow", "queues", TreeTitle = "Queues")]` (etc.) and `GetTreeNodes` returns one node per workflow / definition / theme.

- [ ] **Step 4: Build and commit**

```bash
dotnet build src/SplatDev.Umbraco.Workflow.Backoffice/SplatDev.Umbraco.Workflow.Backoffice.csproj -c Debug --nologo
git add src/SplatDev.Umbraco.Workflow.Backoffice/
git commit -m "feat(backoffice): section, manifest, and three trees"
```

---

### Task 16: AngularJS service `workflowResource` (API client)

**Files:**
- Create: `src/SplatDev.Umbraco.Workflow.Backoffice/App_Plugins/SplatDev.Workflow/services/workflowResource.js`

- [ ] **Step 1: Write the resource**

```javascript
(function () {
    'use strict';

    angular.module('umbraco.resources').factory('splatWorkflow.resource', ['$http', '$q', function ($http, $q) {
        var base = '/umbraco/backoffice/SplatDevWorkflow';

        function get(url, params) {
            return $http.get(base + url, { params: params }).then(function (r) { return r.data; });
        }
        function post(url, body) {
            return $http.post(base + url, body).then(function (r) { return r.data; });
        }

        return {
            listInstances: function (filter) { return get('/WorkflowInstances/List', filter); },
            getInstance:   function (id)     { return get('/WorkflowInstances/Get/' + id); },
            transition:    function (id, actionKey) { return post('/WorkflowInstances/Transition/' + id, { actionKey: actionKey }); },
            createInstance: function (workflowKey, metadataJson) { return post('/WorkflowInstances/Create', { workflowKey: workflowKey, metadataJson: metadataJson }); },

            listDefinitions:  function ()                { return get('/WorkflowDefinitions/List'); },
            getDefinition:    function (key)             { return get('/WorkflowDefinitions/Get/' + key); },
            saveDefinition:   function (def)             { return post('/WorkflowDefinitions/Save', def); },

            listThemes:       function ()                { return get('/WorkflowThemes/List'); },
            getTheme:         function (name)            { return get('/WorkflowThemes/Get/' + name); }
        };
    }]);
})();
```

- [ ] **Step 2: Commit**

```bash
git add src/SplatDev.Umbraco.Workflow.Backoffice/App_Plugins/SplatDev.Workflow/services/workflowResource.js
git commit -m "feat(backoffice): workflowResource AngularJS API client"
```

---

### Task 17: Pizza-chart directive + 3 template variants

**Files:**
- Create: `src/SplatDev.Umbraco.Workflow.Backoffice/App_Plugins/SplatDev.Workflow/directives/pizzaChart.directive.js`
- Create: `src/SplatDev.Umbraco.Workflow.Backoffice/App_Plugins/SplatDev.Workflow/directives/pizzaChart.template-horizontal.html`
- Create: `src/SplatDev.Umbraco.Workflow.Backoffice/App_Plugins/SplatDev.Workflow/directives/pizzaChart.template-vertical.html`
- Create: `src/SplatDev.Umbraco.Workflow.Backoffice/App_Plugins/SplatDev.Workflow/directives/pizzaChart.template-compact.html`

- [ ] **Step 1: Write the directive**

```javascript
(function () {
    'use strict';

    angular.module('umbraco').directive('splatPizzaChart', ['splatWorkflow.themeService', function (themeService) {
        return {
            restrict: 'E',
            scope: {
                steps:        '=',     // [{ key, label, status }]
                currentIndex: '=',
                themeName:    '@'
            },
            link: function (scope, element) {
                element.attr('data-swf-theme', scope.themeName || 'classic');
            },
            templateUrl: function (_, attrs) {
                var t = themeService.layoutForComponent(attrs.themeName || 'classic', 'chart');
                return '/App_Plugins/SplatDev.Workflow/directives/pizzaChart.template-' + t + '.html';
            }
        };
    }]);
})();
```

- [ ] **Step 2: Write the three template variants**

`pizzaChart.template-horizontal.html`:
```html
<div class="swf-chart swf-chart--horizontal" data-swf-theme="{{themeName}}">
    <div class="swf-chart-step"
         ng-repeat="step in steps track by step.key"
         ng-class="{
            'is-active': $index === currentIndex,
            'is-done':   $index < currentIndex,
            'is-pending':$index > currentIndex
         }">
        <span class="swf-chart-step-label">{{ step.label }}</span>
    </div>
</div>
```

`pizzaChart.template-vertical.html` and `pizzaChart.template-compact.html` follow the same data binding but with different markup (vertical stack with donut indicators / compact strip with just dots).

- [ ] **Step 3: Commit**

```bash
git add src/SplatDev.Umbraco.Workflow.Backoffice/App_Plugins/SplatDev.Workflow/directives/
git commit -m "feat(backoffice): pizza-chart directive + 3 template variants"
```

---

### Task 18: Queue dashboard

**Files:**
- Create: `src/SplatDev.Umbraco.Workflow.Backoffice/App_Plugins/SplatDev.Workflow/dashboards/queue.html`
- Create: `src/SplatDev.Umbraco.Workflow.Backoffice/App_Plugins/SplatDev.Workflow/dashboards/queue.controller.js`
- Create: `src/SplatDev.Umbraco.Workflow.Backoffice/App_Plugins/SplatDev.Workflow/directives/queueTable.directive.js`
- Create: `src/SplatDev.Umbraco.Workflow.Backoffice/App_Plugins/SplatDev.Workflow/directives/queueTable.template-table.html`
- Create: `src/SplatDev.Umbraco.Workflow.Backoffice/App_Plugins/SplatDev.Workflow/directives/queueTable.template-cards.html`

- [ ] **Step 1: Write `queue.controller.js`**

```javascript
(function () {
    'use strict';

    angular.module('umbraco').controller('SplatWorkflow.QueueController', [
        '$scope', 'splatWorkflow.resource', 'splatWorkflow.themeService',
        function ($scope, resource, themeService) {
            var vm = this;
            vm.filter = { workflowKey: null, status: 0, page: 1, pageSize: 50, assignedToMe: false, freeText: '' };
            vm.theme  = themeService.current();
            vm.rows   = [];
            vm.total  = 0;
            vm.columns = [];

            vm.load = function () {
                resource.listInstances(vm.filter).then(function (result) {
                    vm.rows = result.items;
                    vm.total = result.totalCount;
                });
            };

            vm.transition = function (instanceId, actionKey) {
                resource.transition(instanceId, actionKey).then(vm.load);
            };

            vm.load();
        }]);
})();
```

- [ ] **Step 2: Write `queue.html`**

```html
<div class="swf-queue" ng-controller="SplatWorkflow.QueueController as vm" data-swf-theme="{{vm.theme}}">
    <header class="swf-queue-filterbar">
        <input type="search" ng-model="vm.filter.freeText" ng-change="vm.load()" placeholder="Search…" />
        <label><input type="checkbox" ng-model="vm.filter.assignedToMe" ng-change="vm.load()" /> Assigned to me</label>
    </header>

    <splat-queue-table rows="vm.rows" columns="vm.columns" theme-name="{{vm.theme}}"></splat-queue-table>

    <footer class="swf-queue-paging">
        <span>{{ vm.total }} results</span>
    </footer>
</div>
```

- [ ] **Step 3: Write `queueTable.directive.js` + the 2 template variants** (table for desktop, cards for compact layouts).

- [ ] **Step 4: Commit**

```bash
git add src/SplatDev.Umbraco.Workflow.Backoffice/App_Plugins/SplatDev.Workflow/dashboards/queue.* \
        src/SplatDev.Umbraco.Workflow.Backoffice/App_Plugins/SplatDev.Workflow/directives/queueTable.*
git commit -m "feat(backoffice): queue dashboard with filter + table/cards directive"
```

---

### Task 19: Config Editor and Themes dashboards

**Files:** `App_Plugins/SplatDev.Workflow/dashboards/definitionEditor.{html,controller.js}` and `themes.{html,controller.js}`.

- [ ] **Step 1: Definition editor** — list workflows, click → edit a JSON form (steps, actions, next-step branches, assignment strategy, action messages). Save POSTs to `WorkflowDefinitions/Save`, which creates a new version row.

- [ ] **Step 2: Themes dashboard** — list installed themes, click → see CSS-variable tokens editable inline with live preview of the chart and queue rendered in the selected theme. Token edits saved as a "theme override" row.

- [ ] **Step 3: i18n resources**

`App_Plugins/SplatDev.Workflow/lang/splatWorkflow.en.xml`:
```xml
<?xml version="1.0" encoding="utf-8"?>
<language>
  <area alias="splatWorkflow">
    <key alias="queue_title">Queue</key>
    <key alias="queue_filter_search">Search</key>
    <key alias="queue_filter_assigned_to_me">Assigned to me</key>
    <key alias="definitions_title">Workflow definitions</key>
    <key alias="themes_title">Themes</key>
  </area>
</language>
```

Plus `splatWorkflow.es.xml` with Spanish equivalents.

- [ ] **Step 4: Commit**

```bash
git add src/SplatDev.Umbraco.Workflow.Backoffice/App_Plugins/SplatDev.Workflow/dashboards/ \
        src/SplatDev.Umbraco.Workflow.Backoffice/App_Plugins/SplatDev.Workflow/lang/
git commit -m "feat(backoffice): config editor + themes dashboards + i18n resources"
```

---

## Phase 6 — Themes

### Task 20: Ship Classic, Modern, Compact themes

**Files:** under `src/SplatDev.Umbraco.Workflow.Themes/App_Plugins/SplatDev.Workflow/Themes/`

- [ ] **Step 1: Write `classic/manifest.json`**

```json
{ "name": "classic", "label": "Classic", "templates": { "chart": "horizontal", "queue": "table" } }
```

- [ ] **Step 2: Write `classic/theme.css`**

```css
[data-swf-theme="classic"] {
    --swf-chart-step-pending-bg:   #e5e7eb;
    --swf-chart-step-active-bg:    #2563eb;
    --swf-chart-step-active-fg:    #ffffff;
    --swf-chart-step-done-bg:      #16a34a;
    --swf-chart-step-done-fg:      #ffffff;
    --swf-chart-step-skipped-bg:   #94a3b8;
    --swf-chart-step-radius:       4px;
    --swf-chart-step-padding:      0.5rem 0.75rem;
    --swf-chart-font:              "Inter", system-ui, sans-serif;
    --swf-queue-row-hover-bg:      #f1f5f9;
    --swf-queue-row-selected-bg:   #dbeafe;
    --swf-queue-row-padding:       0.625rem 0.75rem;
}
```

- [ ] **Step 3: Write `modern/manifest.json` and `modern/theme.css`**

```json
{ "name": "modern", "label": "Modern", "templates": { "chart": "horizontal", "queue": "cards" } }
```

```css
[data-swf-theme="modern"] {
    --swf-chart-step-pending-bg:   #1f2937;
    --swf-chart-step-active-bg:    #6366f1;
    --swf-chart-step-done-bg:      #10b981;
    --swf-chart-step-radius:       9999px;
    --swf-chart-step-padding:      0.625rem 1rem;
    --swf-chart-font:              "Geist", "Inter", system-ui, sans-serif;
    --swf-queue-card-bg:           #0f172a;
    --swf-queue-card-fg:           #e2e8f0;
    --swf-queue-card-radius:       0.75rem;
}
@media (prefers-color-scheme: light) {
    [data-swf-theme="modern"] {
        --swf-queue-card-bg: #ffffff;
        --swf-queue-card-fg: #0f172a;
    }
}
```

- [ ] **Step 4: Write `compact/manifest.json` and `compact/theme.css`**

```json
{ "name": "compact", "label": "Compact", "templates": { "chart": "compact", "queue": "table" } }
```

```css
[data-swf-theme="compact"] {
    --swf-chart-step-pending-bg:   #cbd5e1;
    --swf-chart-step-active-bg:    #0f766e;
    --swf-chart-step-done-bg:      #4d7c0f;
    --swf-chart-step-radius:       2px;
    --swf-chart-step-padding:      0.25rem 0.5rem;
    --swf-chart-font:              "JetBrains Mono", ui-monospace, monospace;
    --swf-queue-row-padding:       0.25rem 0.5rem;
    --swf-queue-row-font-size:     0.8125rem;
}
```

- [ ] **Step 5: Commit**

```bash
git add src/SplatDev.Umbraco.Workflow.Themes/
git commit -m "feat(themes): ship classic, modern, compact"
```

---

## Phase 7 — Sample host + E2E

### Task 21: Sample Umbraco 13 host that integrates the plugin in JSON-bag mode

**Files:** under `samples/SplatDev.Umbraco.Workflow.Sample/`

- [ ] **Step 1: Create the sample Umbraco project**

```bash
cd samples
dotnet new install Umbraco.Templates::13.13.1
dotnet new umbraco -n SplatDev.Umbraco.Workflow.Sample
cd SplatDev.Umbraco.Workflow.Sample
dotnet add reference ../../src/SplatDev.Umbraco.Workflow.Api/SplatDev.Umbraco.Workflow.Api.csproj
dotnet add reference ../../src/SplatDev.Umbraco.Workflow.Backoffice/SplatDev.Umbraco.Workflow.Backoffice.csproj
dotnet add reference ../../src/SplatDev.Umbraco.Workflow.Themes/SplatDev.Umbraco.Workflow.Themes.csproj
cd ../..
dotnet sln add samples/SplatDev.Umbraco.Workflow.Sample/SplatDev.Umbraco.Workflow.Sample.csproj
```

- [ ] **Step 2: Configure connection string for LocalDB**

`samples/SplatDev.Umbraco.Workflow.Sample/appsettings.Development.json`:
```json
{
  "ConnectionStrings": {
    "umbracoDbDSN": "Server=(localdb)\\MSSQLLocalDB;Database=SplatWorkflowSample;Integrated Security=true;TrustServerCertificate=true",
    "umbracoDbDSN_ProviderName": "Microsoft.Data.SqlClient"
  }
}
```

- [ ] **Step 3: Register the JSON provider with sample columns**

In `Program.cs`, after `services.AddUmbraco(...)`:
```csharp
builder.Services.AddSplatDevWorkflowJsonProvider(opts => opts
    .Columns("onboarding",
        new DisplayColumn("candidateName", "Candidate",   "string", true),
        new DisplayColumn("position",      "Position",    "string", true),
        new DisplayColumn("location",      "Location",    "string", true),
        new DisplayColumn("currentDepartment", "Dept",    "badge",  false))
    .Searchable("candidateName", "position", "location"));
```

- [ ] **Step 4: Seed an onboarding workflow definition**

`samples/SplatDev.Umbraco.Workflow.Sample/App_Data/SplatDevWorkflow/Seeds/onboarding.json`:
```json
{
  "key": "onboarding",
  "label": "Employee Onboarding",
  "version": 1,
  "steps": [
    { "key": "payrollNew",     "label": "Background Checks", "actions": [{ "key": "send-to-manager", "label": "Send to Manager", "nextStepKey": "managersNew", "assignment": 0 }], "group": "payroll" },
    { "key": "managersNew",    "label": "Manager Onboarding", "actions": [{ "key": "send-to-payroll", "label": "Send to Payroll", "nextStepKey": "payrollContinue", "assignment": 0 }], "group": "manager" },
    { "key": "payrollContinue","label": "Payroll Acceptance", "actions": [{ "key": "approve", "label": "Approve", "nextStepKey": "itNew", "assignment": 0 }], "group": "payroll" },
    { "key": "itNew",          "label": "Systems",            "actions": [{ "key": "complete", "label": "Complete", "nextStepKey": "payrollFinal", "assignment": 0 }], "group": "it" },
    { "key": "payrollFinal",   "label": "Hire",               "actions": [{ "key": "hire", "label": "Hire", "nextStepKey": "done", "assignment": 0 }], "group": "payroll" },
    { "key": "done",           "label": "Done",               "actions": [] }
  ]
}
```

(Seed-loading code runs on first boot via the existing migration handler.)

- [ ] **Step 5: Run the sample and verify**

```bash
dotnet run --project samples/SplatDev.Umbraco.Workflow.Sample/SplatDev.Umbraco.Workflow.Sample.csproj
```
Manually verify in the browser at `https://localhost:44339/umbraco`:
- Sign in
- Workflow section appears in the sidebar
- Queues > Onboarding loads (empty queue)
- Definitions > Onboarding shows the seeded v1 with 6 steps
- Themes shows 3 themes, switching reflects in the chart preview

- [ ] **Step 6: Commit**

```bash
git add samples/
git commit -m "feat(sample): Umbraco 13 host integrating the plugin in JSON-bag mode"
```

---

### Task 22: Playwright E2E test — queue load + transition + theme switch

**Files:**
- Create: `tests/SplatDev.Umbraco.Workflow.E2E.Tests/QueueFlowTests.cs`

- [ ] **Step 1: Write the E2E test**

```csharp
using FluentAssertions;
using Microsoft.Playwright;
using Xunit;

namespace SplatDev.Umbraco.Workflow.E2E.Tests;

[Collection("E2E")]
public sealed class QueueFlowTests : IAsyncLifetime
{
    private IPlaywright _pw = null!;
    private IBrowser   _browser = null!;
    private const string BaseUrl = "https://localhost:44339";

    public async Task InitializeAsync()
    {
        _pw = await Playwright.CreateAsync();
        _browser = await _pw.Chromium.LaunchAsync(new() { Headless = true });
    }
    public async Task DisposeAsync()
    {
        await _browser.DisposeAsync();
        _pw.Dispose();
    }

    [Fact]
    public async Task QueueLoads_TransitionPersists_ThemeSwitchUpdatesAttribute()
    {
        var ctx  = await _browser.NewContextAsync(new() { IgnoreHTTPSErrors = true });
        var page = await ctx.NewPageAsync();

        // 1. Sign in
        await page.GotoAsync($"{BaseUrl}/umbraco");
        await page.GetByLabel("Email").FillAsync("admin@local");
        await page.GetByLabel("Password").FillAsync("LocalDevPassword!1");
        await page.GetByRole(AriaRole.Button, new() { Name = "Login" }).ClickAsync();

        // 2. Navigate to Workflow > Queues > Onboarding
        await page.GetByRole(AriaRole.Link, new() { Name = "Workflow" }).ClickAsync();
        await page.GetByText("Queues").ClickAsync();
        await page.GetByText("onboarding").ClickAsync();

        // 3. Queue loads
        await page.WaitForSelectorAsync(".swf-queue");
        var rowsBefore = await page.Locator(".swf-queue tbody tr").CountAsync();

        // 4. Create an instance via API for the test (seed)
        var api = await page.APIRequest.NewContextAsync(new() { BaseURL = BaseUrl, IgnoreHTTPSErrors = true });
        await api.PostAsync("/umbraco/backoffice/SplatDevWorkflow/WorkflowInstances/Create",
            new() { DataObject = new { workflowKey = "onboarding", metadataJson = "{\"candidateName\":\"Test Person\",\"position\":\"QA\"}" } });
        await page.ReloadAsync();
        var rowsAfter = await page.Locator(".swf-queue tbody tr").CountAsync();
        rowsAfter.Should().Be(rowsBefore + 1);

        // 5. Switch theme
        await page.GetByText("Themes").ClickAsync();
        await page.GetByRole(AriaRole.Button, new() { Name = "Modern" }).ClickAsync();
        var attr = await page.Locator(".swf-queue").GetAttributeAsync("data-swf-theme");
        attr.Should().Be("modern");
    }
}
```

- [ ] **Step 2: Run the test**

```bash
dotnet test tests/SplatDev.Umbraco.Workflow.E2E.Tests/SplatDev.Umbraco.Workflow.E2E.Tests.csproj
```
Expected: 1 test PASS once the sample host is running on `https://localhost:44339` (the test setup launches it via a fixture; see `Playwright` browser-launch docs for the supplementary `WebApplicationFactory` startup hook, added alongside this test).

- [ ] **Step 3: Commit**

```bash
git add tests/SplatDev.Umbraco.Workflow.E2E.Tests/
git commit -m "test(e2e): queue load + transition + theme switch via Playwright"
```

---

## Phase 8 — Documentation & ship

### Task 23: Integration guide

**Files:**
- Create: `docs/integration-guide.md`

- [ ] **Step 1: Write the integration guide** — covers the three integration modes (JSON-bag, EF, custom provider), service registration, seeding a workflow, defining columns, implementing `IActionMessageDispatcher`, and authoring a theme.

- [ ] **Step 2: Commit**

```bash
git add docs/integration-guide.md
git commit -m "docs: integration guide for host projects"
```

---

### Task 24: README and version stamp

**Files:**
- Create: `README.md`

- [ ] **Step 1: Write a project README** with quickstart, links to the spec / plan / integration guide, status badges, and a screenshot of the queue + chart in each theme.

- [ ] **Step 2: Bump version in `Directory.Build.props` to `1.0.0` and tag the release**

```bash
git add README.md Directory.Build.props
git commit -m "docs: README + v1.0.0 release stamp"
git tag v1.0.0
```

---

## Done criteria for v1

- [ ] `dotnet build` is clean (0 errors, 0 warnings, StyleCop enabled).
- [ ] `dotnet test` passes all suites (Core unit, Persistence migration, E2E).
- [ ] Sample host runs and the manual checklist in Task 21 step 5 passes.
- [ ] All endpoints in spec §4 are wired and respond.
- [ ] Three themes ship and switching between them updates the chart and queue visually.
- [ ] Integration guide is complete; a developer can integrate the plugin into a fresh Umbraco 13 host in ≤ 30 minutes.

---

## Execution

Plan complete and saved to `docs/plans/2026-05-26-workflow-plugin-v1-plan.md`. Two execution options:

**1. Subagent-Driven (recommended)** — I dispatch a fresh subagent per task, review between tasks, fast iteration.

**2. Inline Execution** — Execute tasks in this session using `superpowers:executing-plans`, batch execution with checkpoints.

Which approach?
