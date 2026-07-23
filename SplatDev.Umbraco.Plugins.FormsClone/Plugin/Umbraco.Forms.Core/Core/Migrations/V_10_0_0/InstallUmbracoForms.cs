
// Type: Umbraco.Forms.Core.Migrations.V_10_0_0.InstallUmbracoForms
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using System.Data;

using Umbraco.Cms.Core.Configuration.Models;
using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.Models.Membership;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Scoping;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Strings;
using Umbraco.Cms.Infrastructure.Migrations;
using Umbraco.Forms.Core.Persistence.Dtos;

using IScope = Umbraco.Cms.Infrastructure.Scoping.IScope;
using IScopeProvider = Umbraco.Cms.Infrastructure.Scoping.IScopeProvider;
#nullable enable
namespace Umbraco.Forms.Core.Migrations.V_10_0_0
{
    public class InstallUmbracoForms : FormsMigrationBase
    {
        private readonly IUserService _userService;
        private readonly IHostEnvironment _hostEnvironment;
        private readonly IScopeProvider _scopeProvider;

        public InstallUmbracoForms(
          IPackagingService packagingService,
          IMediaService mediaService,
          MediaFileManager mediaFileManager,
          MediaUrlGeneratorCollection mediaUrlGeneratorCollection,
          IShortStringHelper shortStringHelper,
          IContentTypeBaseServiceProvider contentTypeBaseServiceProvider,
          IMigrationContext context,
          IOptions<PackageMigrationSettings> packageMigrationsSettings,
          IUserService userService,
          IHostEnvironment hostEnvironment,
          IScopeProvider scopeProvider)
          : base(packagingService, mediaService, mediaFileManager, mediaUrlGeneratorCollection, shortStringHelper, contentTypeBaseServiceProvider, context, packageMigrationsSettings)
        {
            this._userService = userService;
            this._hostEnvironment = hostEnvironment;
            this._scopeProvider = scopeProvider;
        }

        protected override void Migrate()
        {
            this.CreateTables();
            this.AddFormsApplicationPermission();
        }

        private void CreateTables()
        {
            Logger.LogDebug("Adding tables required to run Umbraco Forms");
            this.CreateRecordTables();
            this.CreateDefinitionTables();
            this.CreateSecurityTables();
            this.EnsureTableConstraints();
        }

        private void CreateDefinitionTables()
        {
            if (TableExists("UFFolders"))
                Logger.LogDebug("The Umbraco Forms DB table {TableName} already exists", "UFForms");
            else
                Create.Table<FolderDto>(false).Do();
            if (TableExists("UFForms"))
                Logger.LogDebug("The Umbraco Forms DB table {TableName} already exists", "UFForms");
            else
                Create.Table<FormDto>(false).Do();
            if (TableExists("UFWorkflows"))
                Logger.LogDebug("The Umbraco Forms DB table {TableName} already exists", "UFWorkflows");
            else
                Create.Table<WorkflowDto>(false).Do();
            if (TableExists("UFPrevalueSource"))
                Logger.LogDebug("The Umbraco Forms DB table {TableName} already exists", "UFPrevalueSource");
            else
                Create.Table<PrevalueSourceDto>(false).Do();
            if (TableExists("UFDataSource"))
                Logger.LogDebug("The Umbraco Forms DB table {TableName} already exists", "UFDataSource");
            else
                Create.Table<DataSourceDto>(false).Do();
        }

        private void CreateRecordTables()
        {
            if (TableExists("UFRecords"))
                Logger.LogDebug("The Umbraco Forms DB table {TableName} already exists", "UFRecords");
            else
                Create.Table<Record>(false).Do();
            if (TableExists("UFRecordFields"))
                Logger.LogDebug("The Umbraco Forms DB table {TableName} already exists", "UFRecordFields");
            else
                Create.Table<RecordField>(false).Do();
            if (TableExists("UFRecordDataString"))
                Logger.LogDebug("The Umbraco Forms DB table {TableName} already exists", "UFRecordDataString");
            else
                Create.Table<RecordFieldDataString>(false).Do();
            if (TableExists("UFRecordDataLongString"))
                Logger.LogDebug("The Umbraco Forms DB table {TableName} already exists", "UFRecordDataLongString");
            else
                Create.Table<RecordFieldDataLongString>(false).Do();
            if (TableExists("UFRecordDataInteger"))
                Logger.LogDebug("The Umbraco Forms DB table {TableName} already exists", "UFRecordDataInteger");
            else
                Create.Table<RecordFieldDataInteger>(false).Do();
            if (TableExists("UFRecordDataBit"))
                Logger.LogDebug("The Umbraco Forms DB table {TableName} already exists", "UFRecordDataBit");
            else
                Create.Table<RecordFieldDataBit>(false).Do();
            if (TableExists("UFRecordDataDateTime"))
                Logger.LogDebug("The Umbraco Forms DB table {TableName} already exists", "UFRecordDataDateTime");
            else
                Create.Table<RecordFieldDataDateTime>(false).Do();
            if (TableExists("UFRecordAudit"))
                Logger.LogDebug("The Umbraco Forms DB table {TableName} already exists", "UFRecordAudit");
            else
                Create.Table<RecordAudit>(false).Do();
        }

        private void CreateSecurityTables()
        {
            if (TableExists("UFUserSecurity"))
                Logger.LogDebug("The Umbraco Forms DB table {TableName} already exists", "UFUserSecurity");
            else
                Create.Table<UserSecurity>(false).Do();
            if (TableExists("UFUserFormSecurity"))
                Logger.LogDebug("The Umbraco Forms DB table {TableName} already exists", "UFUserFormSecurity");
            else
                Create.Table<UserFormSecurity>(false).Do();
            if (TableExists("UFUserGroupSecurity"))
                Logger.LogDebug("The Umbraco Forms DB table {TableName} already exists", "UFUserGroupSecurity");
            else
                Create.Table<UserGroupSecurity>(false).Do();
            if (TableExists("UFUserGroupFormSecurity"))
                Logger.LogDebug("The Umbraco Forms DB table {TableName} already exists", "UFUserGroupFormSecurity");
            else
                Create.Table<UserGroupFormSecurity>(false).Do();
            if (TableExists("UFUserStartFolders"))
                Logger.LogDebug("The Umbraco Forms DB table {TableName} already exists", "UFUserStartFolders");
            else
                Create.Table<UserStartFolder>(false).Do();
            if (TableExists("UFUserGroupStartFolders"))
                Logger.LogDebug("The Umbraco Forms DB table {TableName} already exists", "UFUserGroupStartFolders");
            else
                Create.Table<UserGroupStartFolder>(false).Do();
        }

        private void EnsureTableConstraints()
        {
            this.AddUniqueConstraint<FolderDto>("UFFolders", "Key");
            this.AddUniqueConstraint<FormDto>("UFForms", "Key");
            this.AddUniqueConstraint<DataSourceDto>("UFDataSource", "Key");
            this.AddUniqueConstraint<PrevalueSourceDto>("UFPrevalueSource", "Key");
            this.AddUniqueConstraint<WorkflowDto>("UFWorkflows", "Key");
            this.AddPrimaryKey<RecordFieldDataBit>("UFRecordDataBit", "Id");
            this.AddPrimaryKey<RecordFieldDataDateTime>("UFRecordDataDateTime", "Id");
            this.AddPrimaryKey<RecordFieldDataInteger>("UFRecordDataInteger", "Id");
            this.AddPrimaryKey<RecordFieldDataLongString>("UFRecordDataLongString", "Id");
            this.AddPrimaryKey<RecordFieldDataString>("UFRecordDataLongString", "Id");
            this.AddForeignKey<RecordField, Record>("UFRecordFields", "UFRecords", "Record", "Id", x => x.Record, x => x.Id, false);
            this.AddForeignKey<RecordFieldDataBit, RecordField>("UFRecordDataBit", "UFRecordFields", "Key", "Key", x => x.Key, x => x.Key, false);
            this.AddForeignKey<RecordFieldDataDateTime, RecordField>("UFRecordDataDateTime", "UFRecordFields", "Key", "Key", x => x.Key, x => x.Key, false);
            this.AddForeignKey<RecordFieldDataInteger, RecordField>("UFRecordDataInteger", "UFRecordFields", "Key", "Key", x => x.Key, x => x.Key, false);
            this.AddForeignKey<RecordFieldDataLongString, RecordField>("UFRecordDataLongString", "UFRecordFields", "Key", "Key", x => x.Key, x => x.Key, false);
            this.AddForeignKey<RecordFieldDataString, RecordField>("UFRecordDataString", "UFRecordFields", "Key", "Key", x => x.Key, x => x.Key, false);
            this.AddPrimaryKey<UserSecurity>("UFUserSecurity", "User");
            this.AddPrimaryKey<UserFormSecurity>("UFUserFormSecurity", "Id");
            this.AddPrimaryKey<UserGroupSecurity>("UFUserGroupSecurity", "UserGroupId");
            this.AddPrimaryKey<UserGroupFormSecurity>("UFUserGroupFormSecurity", "Id");
            this.AddUniqueConstraint<UserFormSecurity>("UFUserFormSecurity", new string[2]
            {
        "User",
        "Form"
            });
            this.AddUniqueConstraint<UserGroupFormSecurity>("UFUserGroupFormSecurity", new string[2]
            {
        "UserGroupId",
        "Form"
            });
            this.AddForeignKey<FolderDto, FolderDto>("UFFolders", "UFFolders", "ParentKey", "Key", x => x.ParentKey, x => x.Key, true);
            this.AddForeignKey<FormDto, FolderDto>("UFForms", "UFFolders", "FolderKey", "Key", x => x.FolderKey, x => x.Key, true);
            this.AddPrimaryKey<UserStartFolder>("UFUserStartFolders", new string[2]
            {
        "UserId",
        "FolderKey"
            });
            this.AddPrimaryKey<UserGroupStartFolder>("UFUserGroupStartFolders", new string[2]
            {
        "UserGroupId",
        "FolderKey"
            });
            this.AddForeignKey<UserStartFolder, FolderDto>("UFUserStartFolders", "UFFolders", "FolderKey", "Key", x => x.FolderKey, x => x.Key, false);
            this.AddForeignKey<UserGroupStartFolder, FolderDto>("UFUserGroupStartFolders", "UFFolders", "FolderKey", "Key", x => x.FolderKey, x => x.Key, false);
        }

        private void AddFormsApplicationPermission()
        {
            using (IScope scope = this._scopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.Unspecified, null, null, null, false, false))
            {
                IUserGroup userGroupByAlias = this._userService.GetUserGroupByAlias("admin");
                if (userGroupByAlias != null)
                {
                    if (userGroupByAlias.AllowedSections.Contains<string>("forms"))
                    {
                        Logger.LogDebug("The Umbraco Forms Application/Section has been assigned to the Admin group already");
                    }
                    else
                    {
                        userGroupByAlias.AddAllowedSection("forms");
                        this._userService.Save(userGroupByAlias);
                    }
                }
                scope.Complete();
            }
        }
    }
}
