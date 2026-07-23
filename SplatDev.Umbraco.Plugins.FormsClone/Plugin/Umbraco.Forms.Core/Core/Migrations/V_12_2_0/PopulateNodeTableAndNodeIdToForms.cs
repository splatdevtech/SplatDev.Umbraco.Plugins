
// Type: Umbraco.Forms.Core.Migrations.V_12_2_0.PopulateNodeTableAndNodeIdToForms
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
using Umbraco.Cms.Infrastructure.Persistence.Dtos;
using Umbraco.Extensions;
using Umbraco.Forms.Core.Models;
using Umbraco.Forms.Core.Persistence.Dtos;
using Umbraco.Forms.Core.Persistence.Factories;
using Umbraco.Forms.Core.Persistence.Repositories;


#nullable enable
namespace Umbraco.Forms.Core.Migrations.V_12_2_0
{
    public class PopulateNodeTableAndNodeIdToForms : FormsMigrationBase
    {
        private readonly IFormRepository _formRepository;
        private readonly IFormFactory _formFactory;

        public PopulateNodeTableAndNodeIdToForms(
          IPackagingService packagingService,
          IMediaService mediaService,
          MediaFileManager mediaFileManager,
          MediaUrlGeneratorCollection mediaUrlGeneratorCollection,
          IShortStringHelper shortStringHelper,
          IContentTypeBaseServiceProvider contentTypeBaseServiceProvider,
          IMigrationContext context,
          IOptions<PackageMigrationSettings> packageMigrationSettings,
          IFormRepository formRepository,
          IFormFactory formFactory)
          : base(packagingService, mediaService, mediaFileManager, mediaUrlGeneratorCollection, shortStringHelper, contentTypeBaseServiceProvider, context, packageMigrationSettings)
        {
            this._formRepository = formRepository;
            this._formFactory = formFactory;
        }

        protected override void Migrate()
        {
            foreach (FormEntitySlim formEntitySlim in this._formRepository.GetManySlim())
            {
                FormEntitySlim form = formEntitySlim;
                if (form.NodeId == 0)
                {
                    Logger.LogDebug("Adding new record to the {tableName} table.", "umbracoNode");
                    object nodeId = Database.Insert<NodeDto>(this._formFactory.BuildNodeDto(form));
                    Logger.LogDebug("Update NodeId to the current form.");
                    Database.Execute(NPocoSqlExtensions.Update<FormDto>(Sql(), u => u.Set(f => f.NodeId, nodeId)).Where<FormDto>(f => f.Id == form.Id, null));
                }
                else
                    Logger.LogDebug("This form with id: {id} already has NodeId: {nodeid}.", form.Id, form.NodeId);
            }
        }
    }
}
