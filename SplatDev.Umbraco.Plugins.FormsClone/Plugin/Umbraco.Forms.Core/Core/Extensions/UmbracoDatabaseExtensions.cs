
// Type: Umbraco.Forms.Core.Extensions.UmbracoDatabaseExtensions
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using NPoco;

using System.Runtime.CompilerServices;

using Umbraco.Cms.Infrastructure.Persistence;
using Umbraco.Extensions;


#nullable enable
namespace Umbraco.Forms.Core.Extensions
{
    public static class UmbracoDatabaseExtensions
    {
        private static readonly string[] _tableNames = new string[19]
        {
      "UFRecords",
      "UFRecordFields",
      "UFRecordDataBit",
      "UFRecordDataDateTime",
      "UFRecordDataInteger",
      "UFRecordDataLongString",
      "UFRecordDataString",
      "UFRecordAudit",
      "UFUserSecurity",
      "UFUserGroupSecurity",
      "UFUserFormSecurity",
      "UFUserGroupFormSecurity",
      "UFForms",
      "UFFolders",
      "UFWorkflows",
      "UFPrevalueSource",
      "UFDataSource",
      "UFUserStartFolders",
      "UFUserGroupStartFolders"
        };

        internal static bool DoesPrimaryKeyExist(
          this IUmbracoDatabase database,
          ISqlContext sqlContext,
          string table)
        {
            Sql<ISqlContext> sql;
            if (sqlContext.IsSqlite())
            {
                UmbracoDatabaseExtensions.ValidateInput(table);
                sql = NPocoSqlExtensions.SelectCount(sqlContext.Sql(), null).From("pragma_table_info('" + table + "')").Where("[pk] = 1");
            }
            else
                sql = NPocoSqlExtensions.SelectCount(sqlContext.Sql(), null).From("INFORMATION_SCHEMA.TABLE_CONSTRAINTS").Where("CONSTRAINT_TYPE = 'PRIMARY KEY' AND TABLE_NAME = @0", table);
            return database.ExecuteScalar<int>(sql) == 1;
        }

        internal static bool DoesForeignKeyExist(
          this IUmbracoDatabase database,
          ISqlContext sqlContext,
          string fromTable,
          string toTable,
          string field)
        {
            string foreignKeyName = database.GetForeignKeyName(fromTable, toTable, field);
            Sql<ISqlContext> sql;
            if (sqlContext.IsSqlite())
            {
                UmbracoDatabaseExtensions.ValidateInput(fromTable);
                UmbracoDatabaseExtensions.ValidateInput(toTable);
                sql = NPocoSqlExtensions.SelectCount(sqlContext.Sql(), null).From("pragma_foreign_key_list('" + fromTable + "')").Where("[table] = @0 AND [from] = @1", toTable, field);
            }
            else
                sql = NPocoSqlExtensions.SelectCount(sqlContext.Sql(), null).From("INFORMATION_SCHEMA.TABLE_CONSTRAINTS").Where("CONSTRAINT_TYPE = 'FOREIGN KEY' AND TABLE_NAME = @0 AND CONSTRAINT_NAME = @1", fromTable, foreignKeyName);
            return database.ExecuteScalar<int>(sql) == 1;
        }

        internal static bool DoesUniqueConstraintExist(
          this IUmbracoDatabase database,
          ISqlContext sqlContext,
          string table,
          string[] columns)
        {
            string uniqueConstraintName = database.GetUniqueConstraintName(table, columns);
            Sql<ISqlContext> sql;
            if (sqlContext.IsSqlite())
            {
                UmbracoDatabaseExtensions.ValidateInput(table);
                string indexName = database.GetIndexName(table, string.Join('_', columns));
                sql = NPocoSqlExtensions.SelectCount(sqlContext.Sql(), null).From("pragma_index_list('" + table + "')").Where("[unique] = 1 AND (name = @0 OR name = @1)", uniqueConstraintName, indexName);
            }
            else
                sql = NPocoSqlExtensions.SelectCount(sqlContext.Sql(), null).From("INFORMATION_SCHEMA.TABLE_CONSTRAINTS").Where("CONSTRAINT_TYPE = 'UNIQUE' AND TABLE_NAME = @0 AND CONSTRAINT_NAME = @1", table, uniqueConstraintName);
            return database.ExecuteScalar<int>(sql) == 1;
        }

        internal static bool DoesIndexExistOnColumn(
          this IUmbracoDatabase database,
          ISqlContext sqlContext,
          string table,
          string column)
        {
            return database.DoesNamedIndexExistOnTable(sqlContext, table, database.GetIndexName(table, column));
        }

        internal static bool DoesNamedIndexExistOnTable(
          this IUmbracoDatabase database,
          ISqlContext sqlContext,
          string table,
          string indexName)
        {
            Sql<ISqlContext> sql;
            if (sqlContext.IsSqlite())
                sql = NPocoSqlExtensions.SelectCount(sqlContext.Sql(), null).From("pragma_index_list('" + table + "')").Where("name = @0", indexName);
            else
                sql = NPocoSqlExtensions.SelectCount(sqlContext.Sql(), null).From("sys.indexes").Where("object_id = OBJECT_ID(@0) AND name = @1", table, indexName);
            return database.ExecuteScalar<int>(sql) == 1;
        }

        public static bool IsSqlite(this ISqlContext sqlContext) => Type.GetType("Umbraco.Cms.Persistence.Sqlite.Services.SqliteSyntaxProvider, Umbraco.Cms.Persistence.Sqlite") == sqlContext.SqlSyntax.GetType();

        private static void ValidateInput(string table)
        {
            if (!_tableNames.InvariantContains(table))
                throw new InvalidOperationException("Cannot determine if schema exists for the table " + table + " as it is not in the list of expected Forms package tables.");
        }

        internal static string GetPrimaryKeyName(this IUmbracoDatabase database, string table) => "PK_" + table;

        internal static string GetForeignKeyName(
          this IUmbracoDatabase database,
          string fromTable,
          string toTable,
          string field)
        {
            DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(5, 3);
            interpolatedStringHandler.AppendLiteral("FK_");
            interpolatedStringHandler.AppendFormatted(fromTable);
            interpolatedStringHandler.AppendLiteral("_");
            interpolatedStringHandler.AppendFormatted(toTable);
            interpolatedStringHandler.AppendLiteral("_");
            interpolatedStringHandler.AppendFormatted(field);
            return interpolatedStringHandler.ToStringAndClear();
        }

        internal static string GetIndexName(
          this IUmbracoDatabase database,
          string table,
          string column)
        {
            return "IX_" + table + "_" + column;
        }

        internal static string GetUniqueConstraintName(
          this IUmbracoDatabase database,
          string table,
          string[] columns)
        {
            return "UK_" + table + "_" + string.Join("_", columns);
        }
    }
}
