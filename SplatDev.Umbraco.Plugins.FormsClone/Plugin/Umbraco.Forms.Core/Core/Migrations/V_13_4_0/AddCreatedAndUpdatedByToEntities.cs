
// Type: Umbraco.Forms.Core.Migrations.V_13_4_0.AddCreatedAndUpdatedByToEntities
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using System.Runtime.CompilerServices;

using Umbraco.Cms.Core.Configuration.Models;
using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Strings;
using Umbraco.Cms.Infrastructure.Migrations;
using Umbraco.Forms.Core.Persistence.Dtos;

using ColumnInfo = Umbraco.Cms.Infrastructure.Persistence.SqlSyntax.ColumnInfo;


#nullable enable
namespace Umbraco.Forms.Core.Migrations.V_13_4_0
{
    public class AddCreatedAndUpdatedByToEntities : FormsMigrationBase
    {
        public AddCreatedAndUpdatedByToEntities(
          IPackagingService packagingService,
          IMediaService mediaService,
          MediaFileManager mediaFileManager,
          MediaUrlGeneratorCollection mediaUrlGeneratorCollection,
          IShortStringHelper shortStringHelper,
          IContentTypeBaseServiceProvider contentTypeBaseServiceProvider,
          IMigrationContext context,
          IOptions<PackageMigrationSettings> packageMigrationsSettings)
          : base(packagingService, mediaService, mediaFileManager, mediaUrlGeneratorCollection, shortStringHelper, contentTypeBaseServiceProvider, context, packageMigrationsSettings)
        {
        }

        protected override void Migrate()
        {
            Logger.LogDebug("Adding created and updated by columns to the entity tables.");
            List<ColumnInfo> list = Context.SqlContext.SqlSyntax.GetColumnsInSchema(Context.Database).ToList<ColumnInfo>();
            this.AddCreatedAndUpdatedByColumns<FormDto>(list, "UFForms");
            this.AddCreatedAndUpdatedByColumns<DataSourceDto>(list, "UFDataSource");
            this.AddCreatedAndUpdatedByColumns<PrevalueSourceDto>(list, "UFPrevalueSource");
        }

        private void AddCreatedAndUpdatedByColumns<TTableDefinition>(
          List<ColumnInfo> columns,
          string tableName)
          where TTableDefinition : BaseEntityDto
        {
            if (TableExists(tableName))
            {
                this.AddColumn<TTableDefinition>(columns, tableName, "CreatedBy");
                this.AddColumn<TTableDefinition>(columns, tableName, "UpdatedBy");
            }
            else
            {
                ILogger logger = Logger;
                DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(125, 2);
                interpolatedStringHandler.AppendLiteral("Table ");
                interpolatedStringHandler.AppendFormatted(tableName);
                interpolatedStringHandler.AppendLiteral(" does not exist so the addition of the created and updated by columnss in migration ");
                interpolatedStringHandler.AppendFormatted(nameof(AddCreatedAndUpdatedByToEntities));
                interpolatedStringHandler.AppendLiteral("migration step cannot be completed.");
                string stringAndClear = interpolatedStringHandler.ToStringAndClear();
                object[] objArray = Array.Empty<object>();
                logger.LogWarning(stringAndClear, objArray);
            }
        }

        private void AddColumn<TTableDefinition>(
          List<ColumnInfo> columns,
          string tableName,
          string columnName)
          where TTableDefinition : BaseEntityDto
        {
            if (columns.SingleOrDefault<ColumnInfo>(x => x.TableName == tableName && x.ColumnName == columnName) != null)
                return;
            AddColumn<TTableDefinition>(tableName, columnName);
        }
    }
}
