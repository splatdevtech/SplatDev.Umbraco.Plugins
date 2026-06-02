# SplatDev.ScheduledTasks

Task scheduler with EF Core persistence and reflection-based invocation. Schedule and execute recurring tasks with database-backed state management.

[![NuGet](https://img.shields.io/nuget/v/SplatDev.ScheduledTasks)](https://www.nuget.org/packages/SplatDev.ScheduledTasks)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

## Compatibility

| .NET Target | Package Version |
|---|---|
| net8.0 | 1.0.x |
| net10.0 | 1.0.x |

## Features

- `TaskScheduler` — Core scheduler service with EF Core persistence
- `ITaskAction` — Contract for task implementations
- `ScheduledTask` / `ScheduledTaskPayload` — Task definition and payload models
- Enum-based task types: `ScheduledTaskType`, `ScheduledTaskCriteriaType`, `ScheduledMessageType`
- `ScheduledEventArgs` — Event-driven task completion notifications
- Includes `ProcessQueuedEmails` example implementation

## Installation

```bash
dotnet add package SplatDev.ScheduledTasks
```

## Dependencies

- Microsoft.EntityFrameworkCore 8.0.13
- Microsoft.EntityFrameworkCore.SqlServer 8.0.13
- SplatDev.Logger (project reference)

## License

MIT © [SplatDev](https://github.com/SplatDev-Ltda)
