
// Type: Umbraco.Forms.Core.Migrations.V_13_3_0.AddDeleteEntriesPermissions
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
namespace Umbraco.Forms.Core.Migrations.V_13_3_0
{
    public class AddDeleteEntriesPermissions : FormsMigrationBase
    {
        public AddDeleteEntriesPermissions(
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
            Logger.LogDebug("Adding delete entries permissions field to the security tables.");
            List<ColumnInfo> list = Context.SqlContext.SqlSyntax.GetColumnsInSchema(Context.Database).ToList<ColumnInfo>();
            this.AddDeleteEntriesColumn<UserSecurity>(list, "UFUserSecurity");
            this.AddDeleteEntriesColumn<UserGroupSecurity>(list, "UFUserGroupSecurity");
        }

        private void AddDeleteEntriesColumn<TTableDefinition>(
          List<ColumnInfo> columns,
          string tableName)
          where TTableDefinition : SecurityBaseDto
        {
            if (TableExists(tableName))
            {
                if (columns.SingleOrDefault<ColumnInfo>(x => x.TableName == tableName && x.ColumnName == "DeleteEntries") != null)
                    return;
                AddColumn<TTableDefinition>(tableName, "DeleteEntries");
                (Context.Database).Execute("UPDATE " + tableName + " SET DeleteEntries = ViewEntries");
            }
            else
            {
                ILogger logger = Logger;
                DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(114, 2);
                interpolatedStringHandler.AppendLiteral("Table ");
                interpolatedStringHandler.AppendFormatted(tableName);
                interpolatedStringHandler.AppendLiteral(" does not exist so the addition of the DeleteEntries column in migration ");
                interpolatedStringHandler.AppendFormatted(nameof(AddDeleteEntriesPermissions));
                interpolatedStringHandler.AppendLiteral("migration step cannot be completed.");
                string stringAndClear = interpolatedStringHandler.ToStringAndClear();
                object[] objArray = Array.Empty<object>();
                logger.LogWarning(stringAndClear, objArray);
            }
        }
    }
}
