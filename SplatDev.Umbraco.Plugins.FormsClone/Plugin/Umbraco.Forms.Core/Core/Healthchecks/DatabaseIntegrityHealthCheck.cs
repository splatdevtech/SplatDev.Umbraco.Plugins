
// Type: Umbraco.Forms.Core.Healthchecks.DatabaseIntegrityHealthCheck
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using System.Data;
using System.Runtime.CompilerServices;

using Umbraco.Cms.Core.HealthChecks;
using Umbraco.Cms.Core.Scoping;
using Umbraco.Forms.Core.Extensions;

using IScope = Umbraco.Cms.Infrastructure.Scoping.IScope;
using IScopeProvider = Umbraco.Cms.Infrastructure.Scoping.IScopeProvider;

#nullable enable
namespace Umbraco.Forms.Core.Healthchecks
{
    [HealthCheck("17942c05-5c2c-4b43-80e2-1bbf1099543a", "Database Integrity", Description = "Checks to ensure that expected database integrity constraints and indexes are in place. Normally these will be created via package installation and upgrades, but it's possible if existing data was already violating constraints they couldn't be added automatically. Guidance on resolving any data issues and adding any missing key, constraints or indexes can be found at: https://docs.umbraco.com/umbraco-forms/developer/healthchecks ", Group = "Forms")]
    public class DatabaseIntegrityHealthCheck : HealthCheck
    {
        private readonly IScopeProvider _scopeProvider;

        public DatabaseIntegrityHealthCheck(IScopeProvider scopeProvider) => this._scopeProvider = scopeProvider;

        public override Task<IEnumerable<HealthCheckStatus>> GetStatus()
        {
            List<HealthCheckStatus> first = new List<HealthCheckStatus>()
      {
        this.CheckPrimaryKeyExists("UFRecordDataBit"),
        this.CheckPrimaryKeyExists("UFRecordDataDateTime"),
        this.CheckPrimaryKeyExists("UFRecordDataInteger"),
        this.CheckPrimaryKeyExists("UFRecordDataLongString"),
        this.CheckPrimaryKeyExists("UFRecordDataString"),
        this.CheckPrimaryKeyExists("UFUserSecurity"),
        this.CheckPrimaryKeyExists("UFUserFormSecurity")
      };
            List<HealthCheckStatus> second1 = new List<HealthCheckStatus>()
      {
        this.CheckForeignKeyExists("UFRecordFields", "UFRecords", "Record"),
        this.CheckForeignKeyExists("UFRecordDataBit", "UFRecordFields", "Key"),
        this.CheckForeignKeyExists("UFRecordDataDateTime", "UFRecordFields", "Key"),
        this.CheckForeignKeyExists("UFRecordDataInteger", "UFRecordFields", "Key"),
        this.CheckForeignKeyExists("UFRecordDataLongString", "UFRecordFields", "Key"),
        this.CheckForeignKeyExists("UFRecordDataString", "UFRecordFields", "Key")
      };
            List<HealthCheckStatus> second2 = new List<HealthCheckStatus>()
      {
        this.CheckUniqueConstraintExists("UFUserFormSecurity", new string[2]
        {
          "User",
          "Form"
        }),
        this.CheckUniqueConstraintExists("UFForms", "Key"),
        this.CheckUniqueConstraintExists("UFDataSource", "Key"),
        this.CheckUniqueConstraintExists("UFPrevalueSource", "Key"),
        this.CheckUniqueConstraintExists("UFWorkflows", "Key")
      };
            List<HealthCheckStatus> second3 = new List<HealthCheckStatus>()
      {
        this.CheckIndexExists("UFRecordDataBit", "Key", "IX_databit_recordfield"),
        this.CheckIndexExists("UFRecordDataDateTime", "Key", "IX_datadatetime_recordfield"),
        this.CheckIndexExists("UFRecordDataInteger", "Key", "IX_datainteger_recordfield"),
        this.CheckIndexExists("UFRecordDataLongString", "Key", "IX_datalongstring_recordfield"),
        this.CheckIndexExists("UFRecordDataString", "Key", "IX_datastring_recordfield"),
        this.CheckIndexExists("UFWorkflows", "FormId")
      };
            return Task.FromResult<IEnumerable<HealthCheckStatus>>(first.Union<HealthCheckStatus>(second1).Union<HealthCheckStatus>(second2).Union<HealthCheckStatus>(second3));
        }

        public override HealthCheckStatus ExecuteAction(HealthCheckAction action) => throw new InvalidOperationException("Forms database integrity check has no executable actions");

        private HealthCheckStatus CheckPrimaryKeyExists(string table)
        {
            bool flag = this.DoesPrimaryKeyExist(table);
            DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(25, 2);
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
            using (IScope scope = this._scopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.Unspecified, null, null, null, false, false))
                return scope.Database.DoesPrimaryKeyExist(scope.SqlContext, table);
        }

        private HealthCheckStatus CheckForeignKeyExists(
          string fromTable,
          string toTable,
          string field)
        {
            bool flag = this.DoesForeignKeyExist(fromTable, toTable, field);
            DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(57, 4);
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
            using (IScope scope = this._scopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.Unspecified, null, null, null, false, false))
                return scope.Database.DoesForeignKeyExist(scope.SqlContext, fromTable, toTable, field);
        }

        private HealthCheckStatus CheckUniqueConstraintExists(
          string table,
          string column)
        {
            return this.CheckUniqueConstraintExists(table, new string[1]
            {
        column
            });
        }

        private HealthCheckStatus CheckUniqueConstraintExists(
          string table,
          string[] columns)
        {
            bool flag = this.DoesUniqueConstraintExist(table, columns);
            DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(42, 4);
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
            using (IScope scope = this._scopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.Unspecified, null, null, null, false, false))
                return scope.Database.DoesUniqueConstraintExist(scope.SqlContext, table, columns);
        }

        private HealthCheckStatus CheckIndexExists(
          string table,
          string column,
          string alternativeIndexName = "")
        {
            bool flag = this.DoesIndexExistOnColumn(table, column) || !string.IsNullOrEmpty(alternativeIndexName) && this.DoesNamedIndexExistOnTable(table, alternativeIndexName);
            DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(30, 3);
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
            using (IScope scope = this._scopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.Unspecified, null, null, null, false, false))
                return scope.Database.DoesIndexExistOnColumn(scope.SqlContext, table, column);
        }

        private bool DoesNamedIndexExistOnTable(string table, string indexName)
        {
            using (IScope scope = this._scopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.Unspecified, null, null, null, false, false))
                return scope.Database.DoesNamedIndexExistOnTable(scope.SqlContext, table, indexName);
        }
    }
}
