
// Type: Umbraco.Forms.Core.Migrations.V_10_5_2.EnsureIndexOnFormFolderKey
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Umbraco.Cms.Core.Configuration.Models;
using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Strings;
using Umbraco.Cms.Infrastructure.Migrations;


#nullable enable
namespace Umbraco.Forms.Core.Migrations.V_10_5_2
{
  public class EnsureIndexOnFormFolderKey : FormsMigrationBase
  {
    public EnsureIndexOnFormFolderKey(
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
      Logger.LogDebug("Ensuring index on the UFForms.FolderKey field.");
      this.AddIndex("UFForms", "FolderKey");
    }
  }
}
