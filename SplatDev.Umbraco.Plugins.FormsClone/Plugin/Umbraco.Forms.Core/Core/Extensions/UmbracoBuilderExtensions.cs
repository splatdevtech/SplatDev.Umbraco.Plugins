
// Type: Umbraco.Forms.Core.Extensions.UmbracoBuilderExtensions
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.Extensions;
using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.Mapping;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Infrastructure.Persistence.Mappers;
using Umbraco.Extensions;
using Umbraco.Forms.Core.Cache;
using Umbraco.Forms.Core.Cache.NotificationHandlers;
using Umbraco.Forms.Core.Configuration;
using Umbraco.Forms.Core.Configuration.Validation;
using Umbraco.Forms.Core.Data;
using Umbraco.Forms.Core.Data.Helpers;
using Umbraco.Forms.Core.Data.Storage;
using Umbraco.Forms.Core.HostedServices;
using Umbraco.Forms.Core.Models.Mapping;
using Umbraco.Forms.Core.NotificationHandlers;
using Umbraco.Forms.Core.Persistence.Factories;
using Umbraco.Forms.Core.Persistence.Mappers;
using Umbraco.Forms.Core.Persistence.Repositories;
using Umbraco.Forms.Core.Persistence.Repositories.Implement;
using Umbraco.Forms.Core.Providers;
using Umbraco.Forms.Core.Searchers;
using Umbraco.Forms.Core.Security;
using Umbraco.Forms.Core.Services;

using Umbraco.Forms.Core.Services.Notifications;
using Umbraco.Forms.Data.FileSystem;
using Umbraco.Forms.Data.Storage;


#nullable enable
namespace Umbraco.Forms.Core.Extensions
{
    public static class UmbracoBuilderExtensions
    {
        public static IUmbracoBuilder AddUmbracoFormsCore(this IUmbracoBuilder builder)
        {
            builder.Services.AddUnique<IFormRecordSearcher, FormRecordSearcher>();
            builder.Services.AddUnique<IFolderFactory, FolderFactory>();
            builder.Services.AddUnique<IFormFactory, FormFactory>();
            builder.Services.AddUnique<IPrevalueSourceFactory, PrevalueSourceFactory>();
            builder.Services.AddUnique<IWorkflowFactory, WorkflowFactory>();
            builder.Services.AddUnique<IDataSourceFactory, DataSourceFactory>();
            builder.Services.AddUnique<IFieldTypeStorage, FieldTypeStorage>();
            builder.Services.AddUnique<IFormTemplateStorage, FormTemplateStorage>();
            builder.Services.AddUnique<IPreValueTextFileStorage, PreValueTextFileStorage>();
            builder.Services.AddUnique<IRecordFieldValueStorage, RecordFieldValueStorage>();
            builder.Services.AddUnique<IRecordFieldStorage, RecordFieldStorage>();
            builder.Services.AddUnique<IRecordAuditStorage, RecordAuditStorage>();
            builder.Services.AddUnique<IRecordWorkflowAuditStorage, RecordWorkflowAuditStorage>();
            builder.Services.AddUnique<IRecordStorage, RecordStorage>();
            builder.Services.AddUnique<IUserSecurityStorage, UserSecurityStorage>();
            builder.Services.AddUnique<IUserGroupSecurityStorage, UserGroupSecurityStorage>();
            builder.Services.AddUnique<IUserFormSecurityStorage, UserFormSecurityStorage>();
            builder.Services.AddUnique<IUserGroupFormSecurityStorage, UserGroupFormSecurityStorage>();
            builder.Services.AddUnique<IUserStartFolderStorage, UserStartFolderStorage>();
            builder.Services.AddUnique<IUserGroupStartFolderStorage, UserGroupStartFolderStorage>();
            builder.Services.AddUnique<IFolderRepository, FolderRepository>();
            builder.Services.AddUnique<IFormRepository, FormRepository>();
            builder.Services.AddUnique<IWorkflowRepository, WorkflowRepository>();
            builder.Services.AddUnique<IPrevalueSourceRepository, PrevalueSourceRepository>();
            builder.Services.AddUnique<IDataSourceRepository, DataSourceRepository>();
            builder.Services.AddUnique<IPlaceholderParsingService, PlaceholderParsingService>();
            builder.Services.AddUnique<IScheduledRecordDeletionService, ScheduledRecordDeletionService>();
            builder.Services.AddHostedService<ScheduledRecordDeletion>();
            builder.Services.AddUnique<IWorkflowExecutionService, WorkflowExecutionService>();
            builder.Services.AddUnique<IWorkflowService, WorkflowService>();
            builder.Services.AddUnique<IFieldPreValueSourceService, FieldPreValueSourceService>();
            builder.Services.AddUnique<IFieldPreValueSourceTypeService, FieldPreValueSourceTypeService>();
            builder.Services.AddUnique<IRecordService, RecordService>();
            builder.Services.AddUnique<IXmlService, XmlService>();
            builder.Services.AddUnique<IPageService, PublishedContentPageService>();
            builder.Services.AddUnique<IWorkflowEmailService, WorkflowEmailService>();
            builder.Services.AddUnique<IFieldService, FieldService>();
            builder.Services.AddUnique<IFolderService, FolderService>();
            builder.Services.AddUnique<IFormService, FormService>();
            builder.Services.AddUnique<IPrevalueSourceService, PrevalueSourceService>();
            builder.Services.AddUnique<IDataSourceService, DataSourceService>();
            builder.WithCollectionBuilder<MapperCollectionBuilder>().Add<FolderMapPersistenceDefinition>();
            builder.WithCollectionBuilder<MapDefinitionCollectionBuilder>().Add<FolderMapDefinition>();
            builder.WithCollectionBuilder<MapperCollectionBuilder>().Add<FormMapPersistenceDefinition>();
            builder.WithCollectionBuilder<MapDefinitionCollectionBuilder>().Add<FormMapDefinition>();
            builder.WithCollectionBuilder<MapperCollectionBuilder>().Add<WorkflowMapPersistenceDefinition>();
            builder.WithCollectionBuilder<MapDefinitionCollectionBuilder>().Add<WorkflowMapDefinition>();
            builder.WithCollectionBuilder<MapperCollectionBuilder>().Add<PrevalueSourceMapPersistenceDefinition>();
            builder.WithCollectionBuilder<MapDefinitionCollectionBuilder>().Add<PrevalueSourceMapDefinition>();
            builder.WithCollectionBuilder<MapperCollectionBuilder>().Add<DataSourceMapPersistenceDefinition>();
            builder.WithCollectionBuilder<MapDefinitionCollectionBuilder>().Add<DataSourceMapDefinition>();
            builder.Services.AddUnique<IFormsSecurity, FormsSecurity>();
            builder.Services.AddUnique<IFormsDistributedCache, FormsDistributedCache>();
            builder.Services.AddSingleton<FieldPreValueSourceCollectionFactory>();
            builder.Services.AddSingleton<WorkflowCollectionFactory>();
            builder.Services.AddUnique<IDictionaryHelper, DictionaryHelper>();
            builder.Services.AddUnique<IDynamicRootContentLocator, DynamicRootContentLocator>();
            builder.SetFormsSavedDataFileSystem(factory =>
            {
                IIOHelper requiredService1 = ServiceProviderServiceExtensions.GetRequiredService<IIOHelper>(factory);
                IHostEnvironment requiredService2 = ServiceProviderServiceExtensions.GetRequiredService<IHostEnvironment>(factory);
                Umbraco.Cms.Core.Hosting.IHostingEnvironment requiredService3 = ServiceProviderServiceExtensions.GetRequiredService<Umbraco.Cms.Core.Hosting.IHostingEnvironment>(factory);
                ILogger<PhysicalFileSystem> requiredService4 = ServiceProviderServiceExtensions.GetRequiredService<ILogger<PhysicalFileSystem>>(factory);
                string str = requiredService2.MapPathContentRoot("~/umbraco/Data/UmbracoForms/");
                string absolute = requiredService3.ToAbsolute("~/umbraco/Data/UmbracoForms/");
                FormsFileSystemForSavedData.IsDefault = true;
                Umbraco.Cms.Core.Hosting.IHostingEnvironment hostingEnvironment = requiredService3;
                ILogger<PhysicalFileSystem> logger = requiredService4;
                string rootPath = str;
                string rootUrl = absolute;
                return new FormsFileSystemForSavedData(new PhysicalFileSystem(requiredService1, hostingEnvironment, logger, rootPath, rootUrl));
            });
            builder.Services.AddSingleton<IValidateOptions<PackageOptionSettings>, PackageOptionSettingsValidator>();
            UmbracoBuilderExtensions.AddAndValidateConfiguration<FormDesignSettings>(builder.Services, Constants.Configuration.SectionKeys.FormDesign);
            UmbracoBuilderExtensions.AddAndValidateConfiguration<PackageOptionSettings>(builder.Services, Constants.Configuration.SectionKeys.PackageOptions);
            UmbracoBuilderExtensions.AddAndValidateConfiguration<SecuritySettings>(builder.Services, Constants.Configuration.SectionKeys.Security);
            UmbracoBuilderExtensions.AddAndValidateConfiguration<DatePickerSettings>(builder.Services, Constants.Configuration.SectionKeys.FieldTypes.DatePicker);
            UmbracoBuilderExtensions.AddAndValidateConfiguration<Recaptcha2Settings>(builder.Services, Constants.Configuration.SectionKeys.FieldTypes.Recaptcha2);
            UmbracoBuilderExtensions.AddAndValidateConfiguration<Recaptcha3Settings>(builder.Services, Constants.Configuration.SectionKeys.FieldTypes.Recaptcha3);
            UmbracoBuilderExtensions.AddAndValidateConfiguration<RichTextSettings>(builder.Services, Constants.Configuration.SectionKeys.FieldTypes.RichText);
            UmbracoBuilderExtensions.AddAndValidateConfiguration<TitleAndDescriptionSettings>(builder.Services, Constants.Configuration.SectionKeys.FieldTypes.TitleAndDescription);
            builder.AddDistributedCacheNotificationHandlers();
            builder.AddNotificationHandler<FormCreatedNotification, FormCreatedNotificationHandler>();
            builder.AddNotificationHandler<FormDeletingNotification, FormDeletingNotificationHandler>();
            builder.WebhookEvents().AddForms(true);
            return builder;
        }

        private static void AddAndValidateConfiguration<T>(
          IServiceCollection services,
          string configurationSectionPath)
          where T : class
        {
            services.AddOptions<T>().BindConfiguration<T>(configurationSectionPath).ValidateDataAnnotations<T>().ValidateOnStart<T>();
        }

        private static IUmbracoBuilder AddDistributedCacheNotificationHandlers(
          this IUmbracoBuilder builder)
        {
            return builder.AddNotificationHandler<DataSourceDeletedNotification, DataSourceDeletedDistributedCacheNotificationHandler>().AddNotificationHandler<DataSourceSavedNotification, DataSourceSavedDistributedCacheNotificationHandler>().AddNotificationHandler<FolderDeletedNotification, FolderDeletedDistributedCacheNotificationHandler>().AddNotificationHandler<FolderSavedNotification, FolderSavedDistributedCacheNotificationHandler>().AddNotificationHandler<FormDeletedNotification, FormDeletedDistributedCacheNotificationHandler>().AddNotificationHandler<FormSavedNotification, FormSavedDistributedCacheNotificationHandler>().AddNotificationHandler<MemberSavedNotification, MemberSavedDistributedCacheNotificationHandler>().AddNotificationHandler<PrevalueSourceDeletedNotification, PrevalueSourceDeletedDistributedCacheNotificationHandler>().AddNotificationHandler<PrevalueSourceSavedNotification, PrevalueSourceSavedDistributedCacheNotificationHandler>().AddNotificationHandler<WorkflowDeletedNotification, WorkflowDeletedDistributedCacheNotificationHandler>().AddNotificationHandler<WorkflowSavedNotification, WorkflowSavedDistributedCacheNotificationHandler>();
        }
    }
}
