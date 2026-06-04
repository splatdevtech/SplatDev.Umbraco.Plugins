# Umbraco 18 API Migration Guide

## Rationale

Several APIs deprecated in Umbraco 17 will be REMOVED in Umbraco 18. Fix CS0618 warnings now to ensure smooth upgrade.

## Pattern 1: UmbracoApiController → ControllerBase

Old:
```csharp
public class MyController : UmbracoApiController
{
    [HttpGet]
    public async Task<IActionResult> Get() { ... }
}
```

New:
```csharp
public class MyController : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get() { ... }
}
```

Affected: 67+ controllers across all plugins.

## Pattern 2: IComponent → IAsyncComponent

Old:
```csharp
public class MyComponent : IComponent
{
    public void Initialize() { ... }
    public void Terminate() { }
}
```

New:
```csharp
public class MyComponent : IAsyncComponent
{
    public Task InitializeAsync(CancellationToken ct) { ... return Task.CompletedTask; }
    public Task TerminateAsync(CancellationToken ct) { return Task.CompletedTask; }
}
```

Affected: USStates, CacheManager, EmailTemplates, Members (3+ files).

## Pattern 3: MigrationBase → AsyncMigrationBase

Old:
```csharp
public class MyMigration : MigrationBase
```

New:
```csharp
public class MyMigration : AsyncMigrationBase
```

Affected: 26+ migration files.

## Pattern 4: Constants.Security.SuperUserId → Constants.Security.SuperUserKey

Old:
```csharp
Constants.Security.SuperUserId
```

New:
```csharp
Constants.Security.SuperUserKey  // Guid instead of int
```

Affected: NPoco repository (11 references, 3 files).

## Execution Plan

1. Fix Pattern 1 (67 files) — bulk find-replace, verify build
2. Fix Pattern 2 (3 files) — manual, verify build
3. Fix Pattern 3 (26 files) — bulk find-replace + manual async adjustment
4. Fix Pattern 4 (3 files) — manual type change (int to Guid)

Total estimate: ~240 min (4 hours).
