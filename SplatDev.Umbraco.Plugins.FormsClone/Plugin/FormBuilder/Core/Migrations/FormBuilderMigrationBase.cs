using System.Threading.Tasks;
﻿using FormBuilder.Core.Extensions;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using NPoco;

using System.Linq.Expressions;
using System.Runtime.CompilerServices;

using Umbraco.Cms.Core.Configuration.Models;
using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Strings;
using Umbraco.Cms.Infrastructure.Migrations;

using Umbraco.Cms.Infrastructure.Packaging;

using Umbraco.Cms.Infrastructure.Persistence;

using Umbraco.Extensions;

namespace FormBuilder.Core.Migrations
{
    public abstract class FormBuilderMigrationBase(
      IPackagingService packagingService,
      IMediaService mediaService,
      MediaFileManager mediaFileManager,
      MediaUrlGeneratorCollection mediaUrlGeneratorCollection,
      IShortStringHelper shortStringHelper,
      IContentTypeBaseServiceProvider contentTypeBaseServiceProvider,
      IMigrationContext context,
      IOptions<PackageMigrationSettings> packageMigrationSettings) : PackageMigrationBase(packagingService, mediaService, mediaFileManager, mediaUrlGeneratorCollection, shortStringHelper, contentTypeBaseServiceProvider, context, packageMigrationSettings)
    {
        protected void AddPrimaryKey<TDto>(string table, string column) => AddPrimaryKey<TDto>(table,
        [
            column
        ]);

        protected void AddPrimaryKey<TDto>(string table, string[] columns)
        {
            if (ContainsDuplicates<TDto>(columns))
                LogIncompleteMigrationStep("could not create primary key constraint on " + table + " due to existing duplicate records.", LogLevel.Warning);
            else if (Context.Database.DoesPrimaryKeyExist(Context.SqlContext, table))
                LogIncompleteMigrationStep("could not create primary key constraint on " + table + " as a primary key already exists.", LogLevel.Debug);
            else if (IsSqlite())
                LogIncompleteMigrationStep("could not create primary key constraint on " + table + " as adding constraints to existing tables is not supported with SQLite.");
            else
                Create.PrimaryKey(Database.GetPrimaryKeyName(table), true).OnTable(table).Columns(columns).Do();
        }

        protected void AddForeignKey<TFromDto, TToDto>(
          string fromTable,
          string toTable,
          string foreignColumn,
          string primaryColumn,
          Expression<Func<TFromDto, object?>> fromKeySelector,
          Expression<Func<TToDto, object?>> toKeySelector,
          bool allowNulls)
        {
            Sql<ISqlContext> sql = Database.SqlContext.Sql().SelectCount(null).From<TFromDto>(null).WhereNotIn(fromKeySelector, NPocoSqlExtensions.Select(Database.SqlContext.Sql(),
            [
             toKeySelector
            ]).From<TToDto>(null));
            if (allowNulls)
                sql = sql.Where(foreignColumn + " Is Not Null");
            int num = Database.ExecuteScalar<int>(sql);
            if (num > 0)
            {
                DefaultInterpolatedStringHandler interpolatedStringHandler = new(94, 6);
                interpolatedStringHandler.AppendLiteral("could not create foreign key constraint between ");
                interpolatedStringHandler.AppendFormatted(fromTable);
                interpolatedStringHandler.AppendLiteral(".");
                interpolatedStringHandler.AppendFormatted(foreignColumn);
                interpolatedStringHandler.AppendLiteral(" and ");
                interpolatedStringHandler.AppendFormatted(toTable);
                interpolatedStringHandler.AppendLiteral(".");
                interpolatedStringHandler.AppendFormatted(primaryColumn);
                interpolatedStringHandler.AppendLiteral(" ");
                interpolatedStringHandler.AppendLiteral("due to ");
                interpolatedStringHandler.AppendFormatted(num);
                interpolatedStringHandler.AppendLiteral(" existing data integrity issue");
                interpolatedStringHandler.AppendFormatted(num > 1 ? "s" : string.Empty);
                interpolatedStringHandler.AppendLiteral(".");
                LogIncompleteMigrationStep(interpolatedStringHandler.ToStringAndClear(), LogLevel.Warning);
            }
            else
            {
                string foreignKeyName = Database.GetForeignKeyName(fromTable, toTable, foreignColumn);
                if (Context.Database.DoesForeignKeyExist(Context.SqlContext, fromTable, toTable, foreignColumn))
                    return;
                if (IsSqlite())
                {
                    DefaultInterpolatedStringHandler interpolatedStringHandler = new(126, 4);
                    interpolatedStringHandler.AppendLiteral("could not create foreign key constraint between ");
                    interpolatedStringHandler.AppendFormatted(fromTable);
                    interpolatedStringHandler.AppendLiteral(".");
                    interpolatedStringHandler.AppendFormatted(foreignColumn);
                    interpolatedStringHandler.AppendLiteral(" and ");
                    interpolatedStringHandler.AppendFormatted(toTable);
                    interpolatedStringHandler.AppendLiteral(".");
                    interpolatedStringHandler.AppendFormatted(primaryColumn);
                    interpolatedStringHandler.AppendLiteral(" ");
                    interpolatedStringHandler.AppendLiteral("as adding constraints to existing tables is not supported with SQLite.");
                    LogIncompleteMigrationStep(interpolatedStringHandler.ToStringAndClear());
                }
                else
                    Create.ForeignKey(foreignKeyName).FromTable(fromTable).ForeignColumn(foreignColumn).ToTable(toTable).PrimaryColumn(primaryColumn).Do();
            }
        }

        protected void AddIndex(string table, string column, string alternateIndexName = "")
        {
            string indexName = Database.GetIndexName(table, column);
            if (IndexExists(indexName) || !string.IsNullOrEmpty(alternateIndexName) && IndexExists(alternateIndexName))
                return;
            Create.Index(indexName).OnTable(table).OnColumn(column).Ascending().WithOptions().NonClustered().Do();
        }

        protected void AddUniqueConstraint<TDto>(string table, string column) => AddUniqueConstraint<TDto>(table,
        [
            column
        ]);

        protected void AddUniqueConstraint<TDto>(string table, string[] columns)
        {
            string str = string.Join(", ", columns);
            if (ContainsDuplicates<TDto>(columns))
            {
                DefaultInterpolatedStringHandler interpolatedStringHandler = new(93, 3);
                interpolatedStringHandler.AppendLiteral("could not create unique constraint on ");
                interpolatedStringHandler.AppendFormatted(table);
                interpolatedStringHandler.AppendLiteral(" due to existing ");
                interpolatedStringHandler.AppendLiteral("duplicate records across the column");
                interpolatedStringHandler.AppendFormatted(columns.Length > 1 ? "s" : string.Empty);
                interpolatedStringHandler.AppendLiteral(": ");
                interpolatedStringHandler.AppendFormatted(str);
                interpolatedStringHandler.AppendLiteral(".");
                LogIncompleteMigrationStep(interpolatedStringHandler.ToStringAndClear(), LogLevel.Warning);
            }
            else
            {
                if (Context.Database.DoesUniqueConstraintExist(Context.SqlContext, table, columns))
                    return;
                string uniqueConstraintName = Database.GetUniqueConstraintName(table, columns);
                if (IndexExists(uniqueConstraintName))
                {
                    DefaultInterpolatedStringHandler interpolatedStringHandler = new(97, 3);
                    interpolatedStringHandler.AppendLiteral("could not create unique constraint on the ");
                    interpolatedStringHandler.AppendFormatted(str);
                    interpolatedStringHandler.AppendLiteral(" columns in ");
                    interpolatedStringHandler.AppendFormatted(table);
                    interpolatedStringHandler.AppendLiteral(" ");
                    interpolatedStringHandler.AppendLiteral("as an index with the name ");
                    interpolatedStringHandler.AppendFormatted(uniqueConstraintName);
                    interpolatedStringHandler.AppendLiteral(" already exists.");
                    LogIncompleteMigrationStep(interpolatedStringHandler.ToStringAndClear(), LogLevel.Debug);
                }
                else if (IsSqlite())
                {
                    DefaultInterpolatedStringHandler interpolatedStringHandler = new(125, 2);
                    interpolatedStringHandler.AppendLiteral("could not create unique constraint on the ");
                    interpolatedStringHandler.AppendFormatted(str);
                    interpolatedStringHandler.AppendLiteral(" columns in ");
                    interpolatedStringHandler.AppendFormatted(table);
                    interpolatedStringHandler.AppendLiteral(" ");
                    interpolatedStringHandler.AppendLiteral("as adding constraints to existing tables is not supported with SQLite.");
                    LogIncompleteMigrationStep(interpolatedStringHandler.ToStringAndClear());
                }
                else
                    Create.UniqueConstraint(uniqueConstraintName).OnTable(table).Columns(columns).Do();
            }
        }

        protected bool ContainsRecords<TDto>() => GetRecordCount<TDto>() > 0;

        private void LogIncompleteMigrationStep(string message, LogLevel level = LogLevel.Information) => Logger.Log(level, "Database migration step not completed: {message}", message);

        private bool ContainsDuplicates<TDto>(string[] columns) => GetRecordCount<TDto>() > GetDistinctRecordCount<TDto>(columns);

        private int GetRecordCount<TDto>() => Database.ExecuteScalar<int>(Database.SqlContext.Sql().SelectCount(null).From<TDto>(null));

        private int GetDistinctRecordCount<TDto>(string[] columns)
        {
            if (IsSqlite())
            {
                string str = columns.Length == 1 ? StringConvertedAndQuotedColumnName(columns[0]) : string.Join(" + ", columns.Select(StringConvertedAndQuotedColumnName)) ?? "";
                return Database.Fetch<string>(Database.SqlContext.Sql().Select(str).From<TDto>(null)).Distinct().Count();
            }
            string str1 = columns.Length == 1 ? QuoteColumnName(columns[0]) : "CONCAT(" + string.Join(",", columns.Select(new Func<string, string>(QuoteColumnName))) + ")";
            return Database.ExecuteScalar<int>(Database.SqlContext.Sql().Select("COUNT(DISTINCT(" + str1 + "))").From<TDto>(null));
        }

        private bool IsSqlite() => Database.SqlContext.IsSqlite();

        private string StringConvertedAndQuotedColumnName(string column) => IsSqlite() ? "CAST(" + QuoteColumnName(column) + " as text)" : "CONVERT(nvarchar(1000)," + QuoteColumnName(column) + ")";

        private string QuoteColumnName(string column) => "[" + column + "]";
    }
}