using FluentAssertions;
using FluentMigrator.Runner;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NPoco;
using SplatDev.Umbraco.Workflow.Persistence.Entities;
using SplatDev.Umbraco.Workflow.Persistence.Migrations.Initial;
using Xunit;

namespace SplatDev.Umbraco.Workflow.Persistence.Tests;

public abstract class DbTestBase : IAsyncLifetime
{
    private const string MasterCs = "Server=(localdb)\\MSSQLLocalDB;Database=master;Integrated Security=true;TrustServerCertificate=true";

    private readonly string _dbName = $"SplatWfTest_{Guid.NewGuid():N}";

    public bool LocalDbReady { get; private set; }

    protected string TargetConnectionString =>
        $"Server=(localdb)\\MSSQLLocalDB;Database={_dbName};Integrated Security=true;TrustServerCertificate=true";

    public async Task InitializeAsync()
    {
        if (!LocalDbGuard.IsAvailable())
        {
            return;
        }

        await using var c = new SqlConnection(MasterCs);
        await c.OpenAsync();
        await using var cmd = c.CreateCommand();
        cmd.CommandText = $"CREATE DATABASE [{_dbName}]";
        await cmd.ExecuteNonQueryAsync();
        LocalDbReady = true;
    }

    public async Task DisposeAsync()
    {
        if (!LocalDbReady)
        {
            return;
        }

        try
        {
            await using var c = new SqlConnection(MasterCs);
            await c.OpenAsync();
            await using var cmd = c.CreateCommand();
            cmd.CommandText = $@"
                IF EXISTS (SELECT 1 FROM sys.databases WHERE name = '{_dbName}')
                BEGIN
                    ALTER DATABASE [{_dbName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
                    DROP DATABASE [{_dbName}];
                END";
            await cmd.ExecuteNonQueryAsync();
        }
        catch
        {
        }
    }
}

public sealed class MigrationTests : DbTestBase
{
    [Fact]
    public async Task M001_CreatesAllTablesOnFreshDatabase()
    {
        if (!LocalDbReady) return;
        MigrationTestHarness.Run(TargetConnectionString);

        var tables = new[]
        {
            "splatWorkflowDefinition",
            "splatWorkflowInstance",
            "splatWorkflowEvent",
            "splatWorkflowAssignment",
            "splatWorkflowTask",
        };

        await using var c = new SqlConnection(TargetConnectionString);
        await c.OpenAsync();

        foreach (var name in tables)
        {
            await using var cmd = c.CreateCommand();
            cmd.CommandText = "SELECT COUNT(*) FROM sys.tables WHERE name = @n";
            cmd.Parameters.AddWithValue("@n", name);
            var found = (int)(await cmd.ExecuteScalarAsync())!;
            found.Should().Be(1, $"table {name} must exist after M001");
        }
    }

    [Fact]
    public async Task M001_IsIdempotent_SecondRunDoesNotFail()
    {
        if (!LocalDbReady) return;
        MigrationTestHarness.Run(TargetConnectionString);
        var act = () => MigrationTestHarness.Run(TargetConnectionString);

        act.Should().NotThrow("running M001 a second time must be idempotent");

        await using var c = new SqlConnection(TargetConnectionString);
        await c.OpenAsync();
        await using var cmd = c.CreateCommand();
        cmd.CommandText = "SELECT COUNT(*) FROM sys.tables WHERE name = 'splatWorkflowDefinition'";
        var found = (int)(await cmd.ExecuteScalarAsync())!;
        found.Should().Be(1);
    }
}

public sealed class EntityCrudTests : DbTestBase
{
    [Fact]
    public async Task WorkflowDefinition_CreateReadUpdate_WorksCorrectly()
    {
        if (!LocalDbReady) return;
        MigrationTestHarness.Run(TargetConnectionString);

        await using var c = new SqlConnection(TargetConnectionString);
        await c.OpenAsync();
        using var db = new Database(c);

        var now = DateTime.UtcNow;
        var entity = new
        {
            Key = "test-workflow",
            Label = "Test Workflow",
            Version = 1,
            DefinitionJson = "{\"steps\":[]}",
            IsActive = true,
            CreatedAt = now,
            CreatedBy = "test-user",
        };

        var insertedId = Convert.ToInt32(db.Insert("splatWorkflowDefinition", "id", entity));

        var fetched = db.SingleOrDefault<WorkflowDefinitionEntity>(
            "WHERE [key] = @0 AND version = @1", "test-workflow", 1);
        fetched.Should().NotBeNull();
        fetched!.Label.Should().Be("Test Workflow");
        fetched.DefinitionJson.Should().Be("{\"steps\":[]}");

        db.Execute(
            "UPDATE splatWorkflowDefinition SET isActive = @0 WHERE id = @1",
            false,
            insertedId);

        var updated = db.SingleOrDefault<WorkflowDefinitionEntity>(
            "WHERE id = @0", insertedId);
        updated!.IsActive.Should().BeFalse();
    }

    [Fact]
    public async Task WorkflowInstance_CreateAndRead_WorksCorrectly()
    {
        if (!LocalDbReady) return;
        MigrationTestHarness.Run(TargetConnectionString);

        await using var c = new SqlConnection(TargetConnectionString);
        await c.OpenAsync();
        using var db = new Database(c);

        var now = DateTime.UtcNow;
        var entity = new
        {
            WorkflowKey = "test-wf",
            WorkflowVersion = 1,
            CurrentStepKey = "start",
            Status = (byte)0,
            MetadataJson = "{\"entityId\":42}",
            CreatedAt = now,
            CreatedBy = "test-user",
            UpdatedAt = now,
            UpdatedBy = "test-user",
        };

        var insertedId = Convert.ToInt64(db.Insert("splatWorkflowInstance", "id", entity));
        insertedId.Should().BeGreaterThan(0);

        var fetched = db.SingleOrDefault<dynamic>(
            "WHERE id = @0", insertedId);
        ((object)fetched).Should().NotBeNull();
        ((string)fetched.workflowKey).Should().Be("test-wf");
        ((string)fetched.metadataJson).Should().Be("{\"entityId\":42}");
    }

    [Fact]
    public async Task WorkflowEvent_AppendAndReadHistory_WorksCorrectly()
    {
        if (!LocalDbReady) return;
        MigrationTestHarness.Run(TargetConnectionString);

        await using var c = new SqlConnection(TargetConnectionString);
        await c.OpenAsync();
        using var db = new Database(c);

        var now = DateTime.UtcNow;
        var e1 = new
        {
            InstanceId = 1L,
            EventType = (byte)0,
            FromStepKey = (string?)null,
            ToStepKey = "start",
            ActionKey = (string?)null,
            PayloadJson = (string?)null,
            ActorUsername = "admin",
            OccurredAt = now,
        };
        var e2 = new
        {
            InstanceId = 1L,
            EventType = (byte)1,
            FromStepKey = "start",
            ToStepKey = "review",
            ActionKey = "approve",
            PayloadJson = (string?)null,
            ActorUsername = "admin",
            OccurredAt = now.AddMinutes(1),
        };

        db.Insert("splatWorkflowEvent", "id", e1);
        db.Insert("splatWorkflowEvent", "id", e2);

        var history = db.Fetch<dynamic>("WHERE instanceId = @0 ORDER BY occurredAt", 1L);
        history.Should().HaveCount(2);
    }

    [Fact]
    public async Task WorkflowAssignment_CreateAndQueryActive_WorksCorrectly()
    {
        if (!LocalDbReady) return;
        MigrationTestHarness.Run(TargetConnectionString);

        await using var c = new SqlConnection(TargetConnectionString);
        await c.OpenAsync();
        using var db = new Database(c);

        var assignment = new
        {
            InstanceId = 1L,
            AssignedTo = "user@example.com",
            AssignedToGroup = "payroll",
            Department = "finance",
            AssignedAt = DateTime.UtcNow,
            IsActive = true,
        };

        var id = Convert.ToInt64(db.Insert("splatWorkflowAssignment", "id", assignment));
        id.Should().BeGreaterThan(0);

        var active = db.Fetch<dynamic>("WHERE isActive = 1 AND instanceId = @0", 1L);
        active.Should().HaveCount(1);
        ((string)active[0].assignedTo).Should().Be("user@example.com");
    }

    [Fact]
    public async Task WorkflowTask_CreateAndComplete_WorksCorrectly()
    {
        if (!LocalDbReady) return;
        MigrationTestHarness.Run(TargetConnectionString);

        await using var c = new SqlConnection(TargetConnectionString);
        await c.OpenAsync();
        using var db = new Database(c);

        var task = new
        {
            InstanceId = 1L,
            Alias = "review-contract",
            Name = "Review Contract",
            Description = "Review the attached contract document",
            IsCompleted = false,
            CompletedAt = (DateTime?)null,
            CompletedBy = (string?)null,
            DepartmentId = (int?)null,
        };

        var id = Convert.ToInt64(db.Insert("splatWorkflowTask", "id", task));
        id.Should().BeGreaterThan(0);

        db.Execute(
            "UPDATE splatWorkflowTask SET isCompleted = 1, completedAt = @0, completedBy = @1 WHERE id = @2",
            DateTime.UtcNow,
            "reviewer@x.com",
            id);

        var completed = db.SingleOrDefault<dynamic>("WHERE id = @0", id);
        ((bool)completed.isCompleted).Should().BeTrue();
        ((string)completed.completedBy).Should().Be("reviewer@x.com");
    }
}

public sealed class TransactionalRollbackTests : DbTestBase
{
    [Fact]
    public async Task InsertWithRollback_DoesNotPersistData()
    {
        if (!LocalDbReady) return;
        MigrationTestHarness.Run(TargetConnectionString);

        await using var c = new SqlConnection(TargetConnectionString);
        await c.OpenAsync();
        using var db = new Database(c);

        db.BeginTransaction();
        try
        {
            db.Insert("splatWorkflowDefinition", "id", new
            {
                Key = "rolled-back",
                Label = "Should Not Exist",
                Version = 1,
                DefinitionJson = "{}",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "test",
            });

            db.AbortTransaction();
        }
        catch
        {
            db.AbortTransaction();
        }

        var found = db.SingleOrDefault<dynamic>(
            "WHERE [key] = @0", "rolled-back");
        found.Should().BeNull("rolled back insert must not persist");
    }

    [Fact]
    public async Task MultipleInsertsInTransaction_AllOrNothing()
    {
        if (!LocalDbReady) return;
        MigrationTestHarness.Run(TargetConnectionString);

        await using var c = new SqlConnection(TargetConnectionString);
        await c.OpenAsync();
        using var db = new Database(c);

        db.BeginTransaction();
        try
        {
            db.Insert("splatWorkflowDefinition", "id", new
            {
                Key = "txn-1",
                Label = "Txn 1",
                Version = 1,
                DefinitionJson = "{}",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "test",
            });

            db.Insert("splatWorkflowDefinition", "id", new
            {
                Key = "txn-2",
                Label = "Txn 2",
                Version = 1,
                DefinitionJson = "{}",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "test",
            });

            db.AbortTransaction();
        }
        catch
        {
            db.AbortTransaction();
        }

        var all = db.Fetch<dynamic>("WHERE [key] LIKE 'txn-%'");
        all.Should().BeEmpty("both inserts in the aborted transaction must be rolled back");
    }
}

internal static class LocalDbGuard
{
    public static bool IsAvailable()
    {
        try
        {
            using var c = new SqlConnection(
                "Server=(localdb)\\MSSQLLocalDB;Database=master;Integrated Security=true;TrustServerCertificate=true;Connect Timeout=3");
            c.Open();
            return true;
        }
        catch
        {
            return false;
        }
    }
}

internal static class MigrationTestHarness
{
    public static void Run(string connectionString)
    {
        var services = new ServiceCollection()
            .AddFluentMigratorCore()
            .ConfigureRunner(rb => rb
                .AddSqlServer()
                .WithGlobalConnectionString(connectionString)
                .ScanIn(typeof(M001_CreateSchema).Assembly).For.Migrations())
            .AddLogging(lb => lb.AddFluentMigratorConsole())
            .BuildServiceProvider(false);

        using var scope = services.CreateScope();
        var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
        runner.MigrateUp();
    }
}
