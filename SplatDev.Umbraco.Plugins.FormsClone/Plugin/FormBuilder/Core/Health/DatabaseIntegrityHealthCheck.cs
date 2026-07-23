using FormBuilder.Core.Extensions;

using System.Data;
using System.Runtime.CompilerServices;

using Umbraco.Cms.Core.HealthChecks;
using Umbraco.Cms.Core.Scoping;

namespace FormBuilder.Core.Health
{
#pragma warning disable CS0618 // Type or member is obsolete

    [HealthCheck("17942c05-5c2c-4b43-80e2-1bbf1099543a", "Database Integrity", Description = "Checks to ensure that expected database integrity constraints and indexes are in place. Normally these will be created via package installation and upgrades, but it's possible if existing data was already violating constraints they couldn't be added automatically. Guidance on resolving any data issues and adding any missing key, constraints or indexes can be found at: https://docs.umbraco.com/umbraco-forms/developer/healthchecks ", Group = "Forms")]
    public class DatabaseIntegrityHealthCheck(IScopeProvider scopeProvider) : HealthCheck
    {
        private readonly IScopeProvider _scopeProvider = scopeProvider;

#pragma warning disable CS0672 // Member overrides obsolete member

        public override Task<IEnumerable<HealthCheckStatus>> GetStatus()
#pragma warning restore CS0672 // Member overrides obsolete member
        {
            List<HealthCheckStatus> first =
          [
            CheckPrimaryKeyExists("UFRecordDataBit"),
            CheckPrimaryKeyExists("UFRecordDataDateTime"),
            CheckPrimaryKeyExists("UFRecordDataInteger"),
            CheckPrimaryKeyExists("UFRecordDataLongString"),
            CheckPrimaryKeyExists("UFRecordDataString"),
            CheckPrimaryKeyExists("UFUserSecurity"),
            CheckPrimaryKeyExists("UFUserFormSecurity")
          ];
            List<HealthCheckStatus> second1 =
          [
            CheckForeignKeyExists("UFRecordFields", "UFRecords", "Record"),
            CheckForeignKeyExists("UFRecordDataBit", "UFRecordFields", "Key"),
            CheckForeignKeyExists("UFRecordDataDateTime", "UFRecordFields", "Key"),
            CheckForeignKeyExists("UFRecordDataInteger", "UFRecordFields", "Key"),
            CheckForeignKeyExists("UFRecordDataLongString", "UFRecordFields", "Key"),
            CheckForeignKeyExists("UFRecordDataString", "UFRecordFields", "Key")
          ];
            List<HealthCheckStatus> second2 =
          [
            CheckUniqueConstraintExists("UFUserFormSecurity",
            [
              "User",
              "Form"
            ]),
            CheckUniqueConstraintExists("UFForms", "Key"),
            CheckUniqueConstraintExists("UFDataSource", "Key"),
            CheckUniqueConstraintExists("UFPrevalueSource", "Key"),
            CheckUniqueConstraintExists("UFWorkflows", "Key")
          ];
            List<HealthCheckStatus> second3 =
          [
            CheckIndexExists("UFRecordDataBit", "Key", "IX_databit_recordfield"),
            CheckIndexExists("UFRecordDataDateTime", "Key", "IX_datadatetime_recordfield"),
            CheckIndexExists("UFRecordDataInteger", "Key", "IX_datainteger_recordfield"),
            CheckIndexExists("UFRecordDataLongString", "Key", "IX_datalongstring_recordfield"),
            CheckIndexExists("UFRecordDataString", "Key", "IX_datastring_recordfield"),
            CheckIndexExists("UFWorkflows", "FormId")
          ];
            return Task.FromResult(first.Union(second1).Union(second2).Union(second3));
        }

        public override HealthCheckStatus ExecuteAction(HealthCheckAction action) => throw new InvalidOperationException("Forms database integrity check has no executable actions");

        private HealthCheckStatus CheckPrimaryKeyExists(string table)
        {
            bool flag = DoesPrimaryKeyExist(table);
            DefaultInterpolatedStringHandler interpolatedStringHandler = new(25, 2);
            interpolatedStringHandler.AppendLiteral("Primary key on table '");
            interpolatedStringHandler.AppendFormatted(table);
            interpolatedStringHandler.AppendLiteral("' ");
            interpolatedStringHandler.AppendFormatted(flag ? "found" : "is missing");
            interpolatedStringHandler.AppendLiteral(".");
            return new HealthCheckStatus(interpolatedStringHandler.ToStringAndClear())
            {
                ResultType = flag ? StatusResultType.Success : StatusResultType.Error
            };
        }

        private bool DoesPrimaryKeyExist(string table)
        {
            using IScope scope = _scopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.Unspecified, null, null, new bool?(), false, false);
            return scope.Database.DoesPrimaryKeyExist(scope.SqlContext, table);
        }

        private HealthCheckStatus CheckForeignKeyExists(
          string fromTable,
          string toTable,
          string field)
        {
            bool flag = DoesForeignKeyExist(fromTable, toTable, field);
            DefaultInterpolatedStringHandler interpolatedStringHandler = new(57, 4);
            interpolatedStringHandler.AppendLiteral("Foreign key on table '");
            interpolatedStringHandler.AppendFormatted(fromTable);
            interpolatedStringHandler.AppendLiteral("', field '");
            interpolatedStringHandler.AppendFormatted(field);
            interpolatedStringHandler.AppendLiteral("', referencing table '");
            interpolatedStringHandler.AppendFormatted(toTable);
            interpolatedStringHandler.AppendLiteral("' ");
            interpolatedStringHandler.AppendFormatted(flag ? "found" : "is missing");
            interpolatedStringHandler.AppendLiteral(".");
            return new HealthCheckStatus(interpolatedStringHandler.ToStringAndClear())
            {
                ResultType = flag ? StatusResultType.Success : StatusResultType.Error
            };
        }

        private bool DoesForeignKeyExist(string fromTable, string toTable, string field)
        {
            using IScope scope = _scopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.Unspecified, null, null, new bool?(), false, false);
            return scope.Database.DoesForeignKeyExist(scope.SqlContext, fromTable, toTable, field);
        }

        private HealthCheckStatus CheckUniqueConstraintExists(
          string table,
          string column)
        {
            return CheckUniqueConstraintExists(table,
            [
        column
            ]);
        }

        private HealthCheckStatus CheckUniqueConstraintExists(
          string table,
          string[] columns)
        {
            bool flag = DoesUniqueConstraintExist(table, columns);
            DefaultInterpolatedStringHandler interpolatedStringHandler = new(42, 4);
            interpolatedStringHandler.AppendLiteral("Unique constraint on table '");
            interpolatedStringHandler.AppendFormatted(table);
            interpolatedStringHandler.AppendLiteral("', column");
            interpolatedStringHandler.AppendFormatted(columns.Length > 1 ? "s" : string.Empty);
            interpolatedStringHandler.AppendLiteral(" '");
            interpolatedStringHandler.AppendFormatted(string.Join(", ", columns));
            interpolatedStringHandler.AppendLiteral("' ");
            interpolatedStringHandler.AppendFormatted(flag ? "found" : "is missing");
            interpolatedStringHandler.AppendLiteral(".");
            return new HealthCheckStatus(interpolatedStringHandler.ToStringAndClear())
            {
                ResultType = flag ? StatusResultType.Success : StatusResultType.Error
            };
        }

        private bool DoesUniqueConstraintExist(string table, string[] columns)
        {
            using IScope scope = _scopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.Unspecified, null, null, new bool?(), false, false);
            return scope.Database.DoesUniqueConstraintExist(scope.SqlContext, table, columns);
        }

        private HealthCheckStatus CheckIndexExists(
          string table,
          string column,
          string alternativeIndexName = "")
        {
            bool flag = DoesIndexExistOnColumn(table, column) || !string.IsNullOrEmpty(alternativeIndexName) && DoesNamedIndexExistOnTable(table, alternativeIndexName);
            DefaultInterpolatedStringHandler interpolatedStringHandler = new(30, 3);
            interpolatedStringHandler.AppendLiteral("Index on table '");
            interpolatedStringHandler.AppendFormatted(table);
            interpolatedStringHandler.AppendLiteral("', column '");
            interpolatedStringHandler.AppendFormatted(column);
            interpolatedStringHandler.AppendLiteral("' ");
            interpolatedStringHandler.AppendFormatted(flag ? "found" : "is missing");
            interpolatedStringHandler.AppendLiteral(".");
            return new HealthCheckStatus(interpolatedStringHandler.ToStringAndClear())
            {
                ResultType = flag ? StatusResultType.Success : StatusResultType.Error
            };
        }

        private bool DoesIndexExistOnColumn(string table, string column)
        {
            using IScope scope = _scopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.Unspecified, null, null, new bool?(), false, false);
            return scope.Database.DoesIndexExistOnColumn(scope.SqlContext, table, column);
        }

        private bool DoesNamedIndexExistOnTable(string table, string indexName)
        {
            using IScope scope = _scopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.Unspecified, null, null, new bool?(), false, false);
            return scope.Database.DoesNamedIndexExistOnTable(scope.SqlContext, table, indexName);
        }
    }

#pragma warning restore CS0618 // Type or member is obsolete
}