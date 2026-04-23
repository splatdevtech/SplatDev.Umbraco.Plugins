using FormBuilder.Core.Persistence;
using FormBuilder.Core.Persistence.Dtos;
using FormBuilder.Core.Persistence.Fields;
using FormBuilder.Core.Persistence.Security;

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

namespace FormBuilder.Core.Migrations
{
#pragma warning disable CS0618 // Type or member is obsolete

    public class InstallFormBuilder(
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
      IScopeProvider scopeProvider) : FormBuilderMigrationBase(packagingService, mediaService, mediaFileManager, mediaUrlGeneratorCollection, shortStringHelper, contentTypeBaseServiceProvider, context, packageMigrationsSettings)
    {
        private readonly IUserService _userService = userService;
        private readonly IHostEnvironment _hostEnvironment = hostEnvironment;
        private readonly IScopeProvider _scopeProvider = scopeProvider;

        protected override void Migrate()
        {
            CreateTables();
            AddFormsApplicationPermission();
        }

        private void CreateTables()
        {
            Logger.LogDebug("Adding tables required to run Form Builder");
            CreateRecordTables();
            CreateDefinitionTables();
            CreateSecurityTables();
            EnsureTableConstraints();
        }

        private void CreateDefinitionTables()
        {
            if (TableExists("FormBuilderFolders"))
                Logger.LogDebug("The Form Builder DB table {TableName} already exists", "FormBuilderForms");
            else
                Create.Table<FolderDto>(false).Do();
            if (TableExists("FormBuilderForms"))
                Logger.LogDebug("The Form Builder DB table {TableName} already exists", "FormBuilderForms");
            else
                Create.Table<FormDto>(false).Do();
            if (TableExists("FormBuilderWorkflows"))
                Logger.LogDebug("The Form Builder DB table {TableName} already exists", "FormBuilderWorkflows");
            else
                Create.Table<WorkflowDto>(false).Do();
            if (TableExists("FormBuilderPrevalueSource"))
                Logger.LogDebug("The Form Builder DB table {TableName} already exists", "FormBuilderPrevalueSource");
            else
                Create.Table<PrevalueSourceDto>(false).Do();
            if (TableExists("FormBuilderDataSource"))
                Logger.LogDebug("The Form Builder DB table {TableName} already exists", "FormBuilderDataSource");
            else
                Create.Table<DataSourceDto>(false).Do();
        }

        private void CreateRecordTables()
        {
            if (TableExists("FormBuilderRecords"))
                Logger.LogDebug("The Form Builder DB table {TableName} already exists", "FormBuilderRecords");
            else
                Create.Table<Record>(false).Do();
            if (TableExists("FormBuilderRecordFields"))
                Logger.LogDebug("The Form Builder DB table {TableName} already exists", "FormBuilderRecordFields");
            else
                Create.Table<RecordField>(false).Do();
            if (TableExists("FormBuilderRecordDataString"))
                Logger.LogDebug("The Form Builder DB table {TableName} already exists", "FormBuilderRecordDataString");
            else
                Create.Table<RecordFieldDataString>(false).Do();
            if (TableExists("FormBuilderRecordDataLongString"))
                Logger.LogDebug("The Form Builder DB table {TableName} already exists", "FormBuilderRecordDataLongString");
            else
                Create.Table<RecordFieldDataLongString>(false).Do();
            if (TableExists("FormBuilderRecordDataInteger"))
                Logger.LogDebug("The Form Builder DB table {TableName} already exists", "FormBuilderRecordDataInteger");
            else
                Create.Table<RecordFieldDataInteger>(false).Do();
            if (TableExists("FormBuilderRecordDataBit"))
                Logger.LogDebug("The Form Builder DB table {TableName} already exists", "FormBuilderRecordDataBit");
            else
                Create.Table<RecordFieldDataBit>(false).Do();
            if (TableExists("FormBuilderRecordDataDateTime"))
                Logger.LogDebug("The Form Builder DB table {TableName} already exists", "FormBuilderRecordDataDateTime");
            else
                Create.Table<RecordFieldDataDateTime>(false).Do();
            if (TableExists("FormBuilderRecordAudit"))
                Logger.LogDebug("The Form Builder DB table {TableName} already exists", "FormBuilderRecordAudit");
            else
                Create.Table<RecordAudit>(false).Do();
        }

        private void CreateSecurityTables()
        {
            if (TableExists("FormBuilderUserSecurity"))
                Logger.LogDebug("The Form Builder DB table {TableName} already exists", "FormBuilderUserSecurity");
            else
                Create.Table<UserSecurity>(false).Do();
            if (TableExists("FormBuilderUserFormSecurity"))
                Logger.LogDebug("The Form Builder DB table {TableName} already exists", "FormBuilderUserFormSecurity");
            else
                Create.Table<UserFormSecurity>(false).Do();
            if (TableExists("FormBuilderUserGroupSecurity"))
                Logger.LogDebug("The Form Builder DB table {TableName} already exists", "FormBuilderUserGroupSecurity");
            else
                Create.Table<UserGroupSecurity>(false).Do();
            if (TableExists("FormBuilderUserGroupFormSecurity"))
                Logger.LogDebug("The Form Builder DB table {TableName} already exists", "FormBuilderUserGroupFormSecurity");
            else
                Create.Table<UserGroupFormSecurity>(false).Do();
            if (TableExists("FormBuilderUserStartFolders"))
                Logger.LogDebug("The Form Builder DB table {TableName} already exists", "FormBuilderUserStartFolders");
            else
                Create.Table<UserStartFolder>(false).Do();
            if (TableExists("FormBuilderUserGroupStartFolders"))
                Logger.LogDebug("The Form Builder DB table {TableName} already exists", "FormBuilderUserGroupStartFolders");
            else
                Create.Table<UserGroupStartFolder>(false).Do();
        }

        private void EnsureTableConstraints()
        {
            AddUniqueConstraint<FolderDto>("FormBuilderFolders", "Key");
            AddUniqueConstraint<FormDto>("FormBuilderForms", "Key");
            AddUniqueConstraint<DataSourceDto>("FormBuilderDataSource", "Key");
            AddUniqueConstraint<PrevalueSourceDto>("FormBuilderPrevalueSource", "Key");
            AddUniqueConstraint<WorkflowDto>("FormBuilderWorkflows", "Key");
            AddPrimaryKey<RecordFieldDataBit>("FormBuilderRecordDataBit", "Id");
            AddPrimaryKey<RecordFieldDataDateTime>("FormBuilderRecordDataDateTime", "Id");
            AddPrimaryKey<RecordFieldDataInteger>("FormBuilderRecordDataInteger", "Id");
            AddPrimaryKey<RecordFieldDataLongString>("FormBuilderRecordDataLongString", "Id");
            AddPrimaryKey<RecordFieldDataString>("FormBuilderRecordDataLongString", "Id");
            AddForeignKey<RecordField, Record>("FormBuilderRecordFields", "FormBuilderRecords", "Record", "Id", x => x.Record, x => x.Id, false);
            AddForeignKey<RecordFieldDataBit, RecordField>("FormBuilderRecordDataBit", "FormBuilderRecordFields", "Key", "Key", x => x.Key, x => x.Key, false);
            AddForeignKey<RecordFieldDataDateTime, RecordField>("FormBuilderRecordDataDateTime", "FormBuilderRecordFields", "Key", "Key", x => x.Key, x => x.Key, false);
            AddForeignKey<RecordFieldDataInteger, RecordField>("FormBuilderRecordDataInteger", "FormBuilderRecordFields", "Key", "Key", x => x.Key, x => x.Key, false);
            AddForeignKey<RecordFieldDataLongString, RecordField>("FormBuilderRecordDataLongString", "FormBuilderRecordFields", "Key", "Key", x => x.Key, x => x.Key, false);
            AddForeignKey<RecordFieldDataString, RecordField>("FormBuilderRecordDataString", "FormBuilderRecordFields", "Key", "Key", x => x.Key, x => x.Key, false);
            AddPrimaryKey<UserSecurity>("FormBuilderUserSecurity", "User");
            AddPrimaryKey<UserFormSecurity>("FormBuilderUserFormSecurity", "Id");
            AddPrimaryKey<UserGroupSecurity>("FormBuilderUserGroupSecurity", "UserGroupId");
            AddPrimaryKey<UserGroupFormSecurity>("FormBuilderUserGroupFormSecurity", "Id");
            AddUniqueConstraint<UserFormSecurity>("FormBuilderUserFormSecurity",
            [
                "User",
                "Form"
            ]);
            AddUniqueConstraint<UserGroupFormSecurity>("FormBuilderUserGroupFormSecurity",
            [
                "UserGroupId",
                "Form"
            ]);
            AddForeignKey<FolderDto, FolderDto>("FormBuilderFolders", "FormBuilderFolders", "ParentKey", "Key", x => x.ParentKey, x => x.Key, true);
            AddForeignKey<FormDto, FolderDto>("FormBuilderForms", "FormBuilderFolders", "FolderKey", "Key", x => x.FolderKey, x => x.Key, true);
            AddPrimaryKey<UserStartFolder>("FormBuilderUserStartFolders",
            [
                "UserId",
                "FolderKey"
            ]);
            AddPrimaryKey<UserGroupStartFolder>("FormBuilderUserGroupStartFolders",
            [
                "UserGroupId",
                "FolderKey"
            ]);
            AddForeignKey<UserStartFolder, FolderDto>("FormBuilderUserStartFolders", "FormBuilderFolders", "FolderKey", "Key", x => x.FolderKey, x => x.Key, false);
            AddForeignKey<UserGroupStartFolder, FolderDto>("FormBuilderUserGroupStartFolders", "FormBuilderFolders", "FolderKey", "Key", x => x.FolderKey, x => x.Key, false);
        }

        private void AddFormsApplicationPermission()
        {
            using IScope scope = _scopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.Unspecified, null, null, new bool?(), false, false);
            IUserGroup? userGroupByAlias = _userService.GetUserGroupByAlias("admin");
            if (userGroupByAlias != null)
            {
                if (userGroupByAlias.AllowedSections.Contains("forms"))
                {
                    Logger.LogDebug("The Form Builder Application/Section has been assigned to the Admin group already");
                }
                else
                {
                    userGroupByAlias.AddAllowedSection("forms");
                    _userService.Save(userGroupByAlias);
                }
            }
              ((ICoreScope)scope).Complete();
        }
    }

#pragma warning restore CS0618 // Type or member is obsolete
}