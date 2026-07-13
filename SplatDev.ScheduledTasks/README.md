# SplatDev.ScheduledTasks

Task scheduling engine with EF Core persistence and reflection-based invocation.
Targets `net8.0` and `net10.0`.

## Installation

```bash
dotnet add package SplatDev.ScheduledTasks
```

## Dependencies

- `Microsoft.EntityFrameworkCore`
- `Microsoft.Extensions.Hosting.Abstractions`

## Features

- `ScheduledTaskDefinition` — EF Core entity for persisted task configuration
- `ScheduledTaskRunner` — background service that polls and executes tasks
- Reflection-based task discovery — scan assemblies for `IScheduledTask`
- `ScheduledTaskType` enum: `OneTime`, `Recurring`, `Interval`
- `ScheduledTaskCriteriaType`: `Date`, `Cron`, `IntervalMinutes`
- Event-driven: `ScheduledEventArgs` with `ScheduledTaskType` and state
- Configuration via `appsettings.json` section

## Usage

### Define a task

```csharp
public class ProcessQueuedEmails : IScheduledTask
{
    public async Task ExecuteAsync(CancellationToken ct)
    {
        // Process email queue
        await Task.CompletedTask;
    }
}
```

### Register in configuration

```json
{
  "ScheduledTasks": {
    "ScanAssemblies": ["MyApp.Tasks"],
    "IntervalSeconds": 30,
    "Enabled": true
  }
}
```

### Seed a task in the database

```csharp
db.ScheduledTasks.Add(new ScheduledTaskDefinition
{
    Name = "Process Queued Emails",
    Type = ScheduledTaskType.Recurring,
    CriteriaType = ScheduledTaskCriteriaType.IntervalMinutes,
    IntervalMinutes = 5,
    AssemblyQualifiedName = typeof(ProcessQueuedEmails).AssemblyQualifiedName,
    IsEnabled = true
});
```

> **Security**: The runner uses `AssemblyQualifiedName` with reflection to load
> and invoke task types. Only register tasks from trusted assemblies. Use
> `ScanAssemblies` to restrict which assemblies are discovered.

## Architecture

```
appsettings.json --> ScheduledTaskRunner (IHostedService)
                          |
                          v
              ScheduledTaskDefinition (EF Core)
                          |
                          v (reflection)
                   IScheduledTask.ExecuteAsync()
```

## License

MIT
