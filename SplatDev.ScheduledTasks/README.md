# SplatDev.ScheduledTasks

EF Core–backed task scheduling library with reflection-based dispatch. Define recurring or one-shot tasks as `ScheduledTask` entities in any `DbContext`, wire up `TaskScheduler`, and it will execute them via `System.Threading.Timer`, invoking any class with a parameterless constructor and a `Perform(ScheduledEventArgs)` method.

## Install

```bash
dotnet add package SplatDev.ScheduledTasks
```

Multi-targets `net8.0` and `net10.0`. Published to nuget.org.

## What's implemented

### `ScheduledTask` (EF Core entity)

| Property | Type | Description |
|----------|------|-------------|
| `Id` | `int` | Primary key |
| `Name` | `string` | Task display name |
| `Active` | `bool` | Enabled/disabled |
| `Repeat` | `bool` | Recurring vs. one-shot |
| `RepeatEveryXHours` / `RepeatEveryXMinutes` | `int` | Recurrence interval |
| `StartOn` | `DateTime` | Scheduled start |
| `EndOn` | `DateTime?` | Optional end date |
| `ClassToInvoke` | `string` | Fully-qualified type name |
| `TaskType` | `ScheduledTaskType` | `Action`, `Check`, `Email`, `Notify`, `Process`, `Report`, `Send`, `Validate`, `Verify` |
| `StopOnError` | `bool` | If `true`, timer stops on exception |
| `LastRunOnUtc` | `DateTime?` | Last execution time |
| `NextRun` | `DateTime?` | Next scheduled execution |

### `TaskScheduler`

The core engine. Instantiate with a `DbContext` and optional in-memory runtime tasks:

```csharp
var scheduler = new TaskScheduler(myDbContext);

scheduler.OnScheduleElapsed += (args) =>
    Console.WriteLine($"Task running: {args.Payload.Task.Name}");

scheduler.OnTriggerEvent += (args) =>
    Console.WriteLine($"Task triggered: {args.Message}");
```

On construction, `TaskScheduler` loads all `ScheduledTask` entities from the database, wires up `System.Threading.Timer` instances for each, and dispatches work via reflection. The target class must have a parameterless constructor and a method:

```csharp
public void Perform(ScheduledEventArgs args) { }
```

### Enums

| Enum | Values |
|------|--------|
| `ScheduledTaskType` | `Action`, `Check`, `Email`, `Notify`, `Process`, `Report`, `Send`, `Validate`, `Verify` |
| `ScheduledTaskCriteriaType` | `Always`, `Passed`, `Failed`, `LessThan`, `LessOrEqual`, `MoreThan`, `MoreOrEqual`, `Equals`, `NotEquals` |
| `ScheduledMessageType` | `Info`, `Success`, `Alert`, `Warning`, `Failure` |

### `ITaskAction` interface

Convention contract for invokable tasks (not enforced by the scheduler):

```csharp
public interface ITaskAction
{
    void Perform(ScheduledEventArgs args);
}
```

### `ScheduledTaskPayload`

Data bag carried in `ScheduledEventArgs`:

| Property | Type | Description |
|----------|------|-------------|
| `Message` | `string` | Context message |
| `Task` | `ScheduledTask` | The task being executed |
| `DependentTaskId` | `int` | Dependent task to run after completion |
| `Dependencies` | `object[]` | Arbitrary dependency data |
| `Actions` | `string[]` | Action names to invoke |

### Logging

Logs to `SplatDev.Logger.Logger.Log()` (the sibling `SplatDev.Logger` package).

## Usage

### 1. Add a scheduled task to your database

```csharp
db.Set<ScheduledTask>().Add(new ScheduledTask
{
    Name = "Daily Report",
    Active = true,
    Repeat = true,
    RepeatEveryXHours = 24,
    StartOn = DateTime.UtcNow,
    TaskType = ScheduledTaskType.Report,
    ClassToInvoke = "MyApp.Tasks.DailyReportTask, MyApp",
});
db.SaveChanges();
```

### 2. Define the task class

```csharp
namespace MyApp.Tasks
{
    public class DailyReportTask
    {
        public void Perform(ScheduledEventArgs args)
        {
            // Generate and email the report
            args.Message = "Report sent";
        }
    }
}
```

### 3. Start the scheduler

```csharp
var scheduler = new TaskScheduler(dbContext);
// Tasks begin executing automatically based on their schedule
```

## DI Registration

No built-in DI extensions. Manually instantiate `TaskScheduler` with your `DbContext`. Register as a singleton or hosted service as needed.

## Dependencies

| Package | Purpose |
|---------|---------|
| `Microsoft.EntityFrameworkCore` (8.0.13) | Entity persistence |
| `Microsoft.EntityFrameworkCore.SqlServer` (8.0.13) | SQL Server provider |
| `SplatDev.Logger` | Logging output |

---

**SplatDev.ScheduledTasks** — part of the [SplatDev.Umbraco.Plugins](https://github.com/SplatDev-Ltda/SplatDev.Umbraco.Plugins) suite. Licensed under MIT. © SplatDev Ltda.
