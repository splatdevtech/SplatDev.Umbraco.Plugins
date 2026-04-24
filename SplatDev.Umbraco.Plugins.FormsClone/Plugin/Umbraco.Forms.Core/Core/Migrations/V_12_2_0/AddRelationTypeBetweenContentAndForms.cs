
// Type: Umbraco.Forms.Core.Migrations.V_12_2_0.AddRelationTypeBetweenContentAndForms
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using Umbraco.Cms.Core.Configuration.Models;
using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Strings;
using Umbraco.Cms.Infrastructure.Migrations;


#nullable enable
namespace Umbraco.Forms.Core.Migrations.V_12_2_0
{
  public class AddRelationTypeBetweenContentAndForms : FormsMigrationBase
  {
    private readonly IRelationService _relationService;

    public AddRelationTypeBetweenContentAndForms(
      IRelationService relationService,
      IPackagingService packagingService,
      IMediaService mediaService,
      MediaFileManager mediaFileManager,
      MediaUrlGeneratorCollection mediaUrlGeneratorCollection,
      IShortStringHelper shortStringHelper,
      IContentTypeBaseServiceProvider contentTypeBaseServiceProvider,
      IMigrationContext context,
      IOptions<PackageMigrationSettings> packageMigrationSettings)
      : base(packagingService, mediaService, mediaFileManager, mediaUrlGeneratorCollection, shortStringHelper, contentTypeBaseServiceProvider, context, packageMigrationSettings)
    {
      this._relationService = relationService;
    }

    protected override void Migrate()
    {
      Logger.LogDebug("Adding relation type between content and form.");
      if (this._relationService.GetRelationTypeByAlias("umbForm") != null)
      {
        Logger.LogDebug("Relation type between content and form already exists.");
      }
      else
      {
        RelationType relationType = new RelationType("Related Form", "umbForm", false, null, null, true);
        relationType.Key = Constants.Relations.FormToContent.Id;
        this._relationService.Save((IRelationType) relationType);
      }
    }
  }
}
