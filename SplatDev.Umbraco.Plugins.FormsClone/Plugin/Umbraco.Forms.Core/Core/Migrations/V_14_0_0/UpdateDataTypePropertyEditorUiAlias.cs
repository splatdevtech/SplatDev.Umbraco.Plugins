
// Type: Umbraco.Forms.Core.Migrations.V_14_0_0.UpdateDataTypePropertyEditorUiAlias
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Umbraco.Cms.Core.Configuration.Models;
using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Strings;
using Umbraco.Cms.Infrastructure.Migrations;


#nullable enable
namespace Umbraco.Forms.Core.Migrations.V_14_0_0
{
  public class UpdateDataTypePropertyEditorUiAlias : FormsMigrationBase
  {
    private readonly IDataTypeService _dataTypeService;

    public UpdateDataTypePropertyEditorUiAlias(
      IDataTypeService dataTypeService,
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
      this._dataTypeService = dataTypeService;
    }

    protected override void Migrate()
    {
      Logger.LogDebug("Updating the propertyEditorUiAlias stored in the database for form's data types.");
      this.UpdateDataTypes("UmbracoForms.FormPicker", "Forms.PropertyEditorUi.FormPicker.Single");
      this.UpdateDataTypes("UmbracoForms.ThemePicker", "Forms.PropertyEditorUi.ThemePicker");
    }

    private void UpdateDataTypes(string editorAlias, string editorUiAlias)
    {
      foreach (IDataType dataType in this._dataTypeService.GetByEditorAliasAsync(editorAlias).GetAwaiter().GetResult())
      {
        dataType.EditorUiAlias = editorUiAlias;
        this._dataTypeService.UpdateAsync(dataType, Umbraco.Cms.Core.Constants.Security.SuperUserKey).GetAwaiter().GetResult();
      }
    }
  }
}
