using FormBuilder.Core.Cache;
using FormBuilder.Core.Configuration;
using FormBuilder.Core.DataSources;
using FormBuilder.Core.Export;
using FormBuilder.Core.Factory;
using FormBuilder.Core.FieldTypes;
using FormBuilder.Core.FileSystem;
using FormBuilder.Core.Filters;
using FormBuilder.Core.Handlers;
using FormBuilder.Core.Helpers;
using FormBuilder.Core.Interfaces;
using FormBuilder.Core.Jobs;
using FormBuilder.Core.Mapping;
using FormBuilder.Core.Models;
using FormBuilder.Core.Notifications.Handlers;
using FormBuilder.Core.Persistence.Factories;
using FormBuilder.Core.Persistence.Interfaces;
using FormBuilder.Core.Persistence.Mappers;
using FormBuilder.Core.Persistence.Repositories;
using FormBuilder.Core.Prevalues;
using FormBuilder.Core.Providers.Builders;
using FormBuilder.Core.Providers.Collections;
using FormBuilder.Core.Providers.DataSourceTypes;
using FormBuilder.Core.Providers.EmailTemplates;
using FormBuilder.Core.Providers.Export;
using FormBuilder.Core.Providers.Factories;
using FormBuilder.Core.Providers.FieldTypes;
using FormBuilder.Core.Providers.ParsedPlacholderFormatters;
using FormBuilder.Core.Providers.Prevalues;
using FormBuilder.Core.Providers.RecordSets;
using FormBuilder.Core.Providers.Themes;
using FormBuilder.Core.Providers.ValidationPatterns;
using FormBuilder.Core.Providers.WorkflowTypes;
using FormBuilder.Core.Searches;
using FormBuilder.Core.Searches.Interfaces;
using FormBuilder.Core.Security;
using FormBuilder.Core.Security.Interfaces;
using FormBuilder.Core.Services;
using FormBuilder.Core.Services.Interfaces;
using FormBuilder.Core.Services.Notifications;
using FormBuilder.Core.Storage;
using FormBuilder.Core.Storage.Interfaces;
using FormBuilder.Core.Validations;
using FormBuilder.Core.Workflows;
using FormBuilder.Examine.Extensions;
using FormBuilder.Extension.Forms.Core.Helpers;
using FormBuilder.Web.Behaviors;
using FormBuilder.Web.Behaviors.Interfaces;
using FormBuilder.Web.EntityPermissions;
using FormBuilder.Web.HttpModules;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;

using Swashbuckle.AspNetCore.SwaggerGen;

using Umbraco.Cms.Api.Common.OpenApi;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.Extensions;
using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.Mapping;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Infrastructure.Persistence.Mappers;
using Umbraco.Cms.Web.Common.ApplicationBuilder;
using Umbraco.Extensions;

namespace FormBuilder.Core.Extensions
{
    public static class FormsUmbracoBuilderExtensions
    {
        public static IUmbracoBuilder AddFormBuilderCore(this IUmbracoBuilder builder)
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
            builder.Services.AddUnique<IFieldPrevalueSourceService, FieldPrevalueSourceService>();
            builder.Services.AddUnique<IFieldPrevalueSourceTypeService, FieldPrevalueSourceTypeService>();
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
            builder.Services.AddSingleton<DistributedCache>();
            builder.Services.AddSingleton<FieldPrevalueSourceCollectionFactory>();
            builder.Services.AddSingleton<WorkflowCollectionFactory>();
            builder.Services.AddUnique<IDictionaryHelper, DictionaryHelper>();
            builder.Services.AddUnique<IDynamicRootContentLocator, DynamicRootContentLocator>();
            builder.SetFormsSavedDataFileSystem(factory =>
            {
                IIOHelper requiredService1 = factory.GetRequiredService<IIOHelper>();
                IHostEnvironment requiredService2 = factory.GetRequiredService<IHostEnvironment>();
                Umbraco.Cms.Core.Hosting.IHostingEnvironment requiredService3 = factory.GetRequiredService<Umbraco.Cms.Core.Hosting.IHostingEnvironment>();
                ILogger<PhysicalFileSystem> requiredService4 = factory.GetRequiredService<ILogger<PhysicalFileSystem>>();
                string str = requiredService2.MapPathContentRoot("~/umbraco/Data/FormBuilder/");
                string absolute = requiredService3.ToAbsolute("~/umbraco/Data/FormBuilder/");
                FormsFileSystemForSavedData.IsDefault = true;
                Umbraco.Cms.Core.Hosting.IHostingEnvironment hostingEnvironment = requiredService3;
                ILogger<PhysicalFileSystem> logger = requiredService4;
                string rootPath = str;
                string rootUrl = absolute;
                return new FormsFileSystemForSavedData(new PhysicalFileSystem(requiredService1, hostingEnvironment, logger, rootPath, rootUrl));
            });
            builder.Services.AddSingleton<IValidateOptions<PackageOptionSettings>, PackageOptionSettingsValidator>();
            AddAndValidateConfiguration<FormDesignSettings>(builder.Services, Constants.Configuration.SectionKeys.FormDesign);
            AddAndValidateConfiguration<PackageOptionSettings>(builder.Services, Constants.Configuration.SectionKeys.PackageOptions);
            AddAndValidateConfiguration<SecuritySettings>(builder.Services, Constants.Configuration.SectionKeys.Security);
            AddAndValidateConfiguration<DatePickerSettings>(builder.Services, Constants.Configuration.SectionKeys.FieldTypes.DatePicker);
            AddAndValidateConfiguration<Recaptcha2Settings>(builder.Services, Constants.Configuration.SectionKeys.FieldTypes.Recaptcha2);
            AddAndValidateConfiguration<Recaptcha3Settings>(builder.Services, Constants.Configuration.SectionKeys.FieldTypes.Recaptcha3);
            AddAndValidateConfiguration<RichTextSettings>(builder.Services, Constants.Configuration.SectionKeys.FieldTypes.RichText);
            AddAndValidateConfiguration<TitleAndDescriptionSettings>(builder.Services, Constants.Configuration.SectionKeys.FieldTypes.TitleAndDescription);
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
            services.AddOptions<T>().BindConfiguration(configurationSectionPath).ValidateDataAnnotations().ValidateOnStart();
        }

        private static IUmbracoBuilder AddDistributedCacheNotificationHandlers(
          this IUmbracoBuilder builder)
        {
            return builder.AddNotificationHandler<DataSourceDeletedNotification, DataSourceDeletedDistributedCacheNotificationHandler>().AddNotificationHandler<DataSourceSavedNotification, DataSourceSavedDistributedCacheNotificationHandler>().AddNotificationHandler<FolderDeletedNotification, FolderDeletedDistributedCacheNotificationHandler>().AddNotificationHandler<FolderSavedNotification, FolderSavedDistributedCacheNotificationHandler>().AddNotificationHandler<FormDeletedNotification, FormDeletedDistributedCacheNotificationHandler>().AddNotificationHandler<FormSavedNotification, FormSavedDistributedCacheNotificationHandler>().AddNotificationHandler<MemberSavedNotification, MemberSavedDistributedCacheNotificationHandler>().AddNotificationHandler<PrevalueSourceDeletedNotification, PrevalueSourceDeletedDistributedCacheNotificationHandler>().AddNotificationHandler<PrevalueSourceSavedNotification, PrevalueSourceSavedDistributedCacheNotificationHandler>().AddNotificationHandler<WorkflowDeletedNotification, WorkflowDeletedDistributedCacheNotificationHandler>().AddNotificationHandler<WorkflowSavedNotification, WorkflowSavedDistributedCacheNotificationHandler>();
        }

        public static IUmbracoBuilder AddFormBuilder(this IUmbracoBuilder builder)
        {
            if (builder.Services.Any(x => x.ServiceType == typeof(IFormRecordSearcher)))
                return builder;
            builder.AddFormBuilderCore();
            builder.AddFormBuilderCoreProviders();
            builder.AddFormBuilderWeb();
            if (IsRecordIndexingEnabled(builder))
                builder.AddFormBuilderExamine();
            return builder;
        }

        private static bool IsRecordIndexingEnabled(IUmbracoBuilder builder) => !builder.Config.GetSection(Constants.Configuration.SectionKeys.PackageOptions).GetValue<bool>("DisableRecordIndexing");

        public static void SetFormsSavedDataFileSystem(
          this IUmbracoBuilder builder,
          Func<IServiceProvider, IFileSystem> filesystemFactory)
        {
            builder.Services.AddUnique(provider =>
            {
                IFileSystem filesystem = filesystemFactory(provider);
                IFileSystem shadowWrapper = provider.GetRequiredService<FileSystems>().CreateShadowWrapper(filesystem, "FormsFileSystemForSavedData");
                return provider.CreateInstance<FormsFileSystemForSavedData>(shadowWrapper);
            });
        }

        /// <summary>
        /// Populates the Forms collection builders with the out-of-the-box providers.
        /// </summary>
        public static IUmbracoBuilder AddFormBuilderCoreProviders(
          this IUmbracoBuilder builder)
        {
            builder.FormsDataSources().Add<MsSql>();
            builder.FormsExporters().Add<ExportToExcel>().Add<ExportToExcelWithDisplayValues>().Add<SaveAllUploadedFiles>().Add<SaveAllUploadedFilesByEntry>();
            builder.FormsFieldPreValueSources().Add<DataSource>().Add<GetValuesFromTextFile>().Add<NodePreValues>().Add<ReadOnlySql>().Add<UmbracoPreValuesReadOnly>();
            builder.FormsFields().Add<CheckBox>().Add<CheckBoxList>().Add<DataConsent>().Add<DatePicker>().Add<DropDownList>().Add<FileUpload>().Add<HiddenField>().Add<Password>().Add<RadioButtonList>().Add<Recaptcha2>().Add<Recaptcha3>().Add<RichText>().Add<Text>().Add<Textarea>().Add<Textfield>();
            builder.FormsRecordSetActions().Add<ApproveRecordSet>().Add<DeleteRecordSet>().Add<RejectRecordSet>();
            builder.FormsWorkflows().Add<ChangeRecordState>().Add<PostAsXml>().Add<PostToUrl>().Add<SaveAsFile>().Add<SaveAsUmbracoNode>().Add<SendEmail>().Add<SendRazorEmail>().Add<SendXsltEmail>().Add<Slack>();
            builder.FormsParsedPlaceholderFormatters().Add<BoundNumber>().Add<Currency>().Add<FormatDate>().Add<FormatNumber>().Add<HtmlEncode>().Add<ToLowerCaseString>().Add<ToUpperCaseString>().Add<TruncateString>();
            builder.FormsValidationPatterns().Append<Email>().Append<Number>().Append<Url>();
            builder.Themes().Add<DefaultTheme>().Add<BootstrapHorizontalTheme>();
            builder.EmailTemplates().Add<DefaultEmailTemplate>();
            return builder;
        }

        /// <summary>
        /// Adds a         /// </summary>
        public static IUmbracoBuilder AddFormsDataSource<T>(this IUmbracoBuilder builder) where T : FormDataSourceType
        {
            builder.FormsDataSources().Add<T>();
            return builder;
        }

        /// <summary>
        /// Provides access to the collection builder for instances of         /// </summary>
        public static DataSourceTypeCollectionBuilder FormsDataSources(
          this IUmbracoBuilder builder)
        {
            return builder.WithCollectionBuilder<DataSourceTypeCollectionBuilder>();
        }

        /// <summary>
        /// Adds an         /// </summary>
        public static IUmbracoBuilder AddFormsExporter<T>(this IUmbracoBuilder builder) where T : ExportType
        {
            builder.FormsExporters().Add<T>();
            return builder;
        }

        /// <summary>
        /// Provides access to the collection builder for instances of         /// </summary>
        public static ExportCollectionBuilder FormsExporters(
          this IUmbracoBuilder builder)
        {
            return builder.WithCollectionBuilder<ExportCollectionBuilder>();
        }

        /// <summary>
        /// Adds a         /// </summary>
        public static IUmbracoBuilder AddFormsFieldPreValueSource<T>(
          this IUmbracoBuilder builder)
          where T : FieldPrevalueSourceType
        {
            builder.FormsFieldPreValueSources().Add<T>();
            return builder;
        }

        /// <summary>
        /// Provides access to the collection builder for instances of         /// </summary>
        public static FieldPrevalueSourceCollectionBuilder FormsFieldPreValueSources(
          this IUmbracoBuilder builder)
        {
            return builder.WithCollectionBuilder<FieldPrevalueSourceCollectionBuilder>();
        }

        /// <summary>
        /// Adds a         /// </summary>
        public static IUmbracoBuilder AddFormsField<T>(this IUmbracoBuilder builder) where T : FieldType
        {
            builder.FormsFields().Add<T>();
            return builder;
        }

        /// <summary>
        /// Provides access to the collection builder for instances of         /// </summary>
        public static FieldCollectionBuilder FormsFields(
          this IUmbracoBuilder builder)
        {
            return builder.WithCollectionBuilder<FieldCollectionBuilder>();
        }

        /// <summary>
        /// Adds a         /// </summary>
        public static IUmbracoBuilder AddFormsRecordSetAction<T>(
          this IUmbracoBuilder builder)
          where T : RecordSetActionType
        {
            builder.FormsRecordSetActions().Add<T>();
            return builder;
        }

        /// <summary>
        /// Provides access to the collection builder for instances of         /// </summary>
        public static RecordSetActionCollectionBuilder FormsRecordSetActions(
          this IUmbracoBuilder builder)
        {
            return builder.WithCollectionBuilder<RecordSetActionCollectionBuilder>();
        }

        /// <summary>
        /// Adds a         /// </summary>
        public static IUmbracoBuilder AddFormsWorkflow<T>(this IUmbracoBuilder builder) where T : WorkflowType
        {
            builder.FormsWorkflows().Add<T>();
            return builder;
        }

        /// <summary>
        /// Provides access to the collection builder for instances of         /// </summary>
        public static WorkflowCollectionBuilder FormsWorkflows(
          this IUmbracoBuilder builder)
        {
            return builder.WithCollectionBuilder<WorkflowCollectionBuilder>();
        }

        /// <summary>
        /// Adds a         /// </summary>
        public static IUmbracoBuilder AddFormsParsedPlaceholderFormatter<T>(
          this IUmbracoBuilder builder)
          where T : IParsedPlaceholderFormatter
        {
            builder.FormsParsedPlaceholderFormatters().Add<T>();
            return builder;
        }

        /// <summary>
        /// Provides access to the collection builder for instances of         /// </summary>
        public static ParsedPlaceholderFormatterCollectionBuilder FormsParsedPlaceholderFormatters(
          this IUmbracoBuilder builder)
        {
            return builder.WithCollectionBuilder<ParsedPlaceholderFormatterCollectionBuilder>();
        }

        /// <summary>
        /// Adds a         /// </summary>
        public static IUmbracoBuilder ApppendFormsValidationPattern<T>(
          this IUmbracoBuilder builder)
          where T : IValidationPattern
        {
            builder.FormsValidationPatterns().Append<T>();
            return builder;
        }

        /// <summary>
        /// Provides access to the collection builder for instances of         /// </summary>
        public static ValidationPatternCollectionBuilder FormsValidationPatterns(
          this IUmbracoBuilder builder)
        {
            return builder.WithCollectionBuilder<ValidationPatternCollectionBuilder>();
        }

        /// <summary>
        /// Provides access to the collection builder for instances of         /// </summary>
        public static ThemeCollectionBuilder Themes(this IUmbracoBuilder builder) => builder.WithCollectionBuilder<ThemeCollectionBuilder>();

        /// <summary>
        /// Provides access to the collection builder for instances of         /// </summary>
        public static EmailTemplateCollectionBuilder EmailTemplates(
          this IUmbracoBuilder builder)
        {
            return builder.WithCollectionBuilder<EmailTemplateCollectionBuilder>();
        }

        public static IUmbracoBuilder AddFormBuilderWeb(this IUmbracoBuilder builder)
        {
            builder.Services.AddSingleton<IApplyDefaultFieldsBehavior, ApplyDefaultFieldsBehavior>();
            builder.Services.AddSingleton<IApplyDefaultWorkflowsBehavior, ApplyDefaultWorkflowsBehavior>();
            builder.WithCollectionBuilder<MapDefinitionCollectionBuilder>().Add<FormDesignMapping>();
            builder.Services.AddScoped<IFormRenderingService, FormRenderingService>();
            builder.Services.AddUnique<IFormThemeResolver>(provider =>
            {
                FileSystems requiredService1 = provider.GetRequiredService<FileSystems>();
                IOptions<FormDesignSettings> requiredService2 = provider.GetRequiredService<IOptions<FormDesignSettings>>();
                ThemeCollection requiredService3 = provider.GetRequiredService<ThemeCollection>();
                if (requiredService1.PartialViewsFileSystem == null)
                    throw new InvalidOperationException("Could not find partial views file system");
                return new FormThemeResolver(requiredService1.PartialViewsFileSystem, requiredService2, requiredService3);
            });
            builder.Services.AddSingleton<ProtectFormUploadRequestsMiddleware>();
            builder.Services.Configure<UmbracoPipelineOptions>(options => options.AddFilter(new UmbracoPipelineFilter("ProtectFormUploadRequestsMiddleware")
            {
                PrePipeline = app => app.UseMiddleware<ProtectFormUploadRequestsMiddleware>()
            }));
            builder.AddFormsDeliveryApi();
            builder.AddFormsManagementApi();
            builder.AddFormsAuthoriziation();
            return builder;
        }

        private static IUmbracoBuilder AddFormsDeliveryApi(this IUmbracoBuilder builder)
        {
            int num = builder.Config.GetSection(Constants.Configuration.SectionKeys.PackageOptions).GetValue<bool>("EnableFormsApi") ? 1 : 0;
            builder.Services.AddSingleton<FormDtoFactory>();
            builder.Services.AddSingleton<EntryAcceptedDtoFactory>();
            builder.Services.AddProblemDetails();
            if (num == 0)
                return builder;
            builder.Services.Configure<SwaggerGenOptions>(options =>
            {
                options.SwaggerDoc("forms-delivery", new OpenApiInfo()
                {
                    Title = "Form Builder Delivery API",
                    Version = "Latest",
                    Description = "Describes the Form Builder Delivery API available for rendering and submitting forms. You can find out more about the API in [the documentation](https://docs.formbuilder.com/)"
                });
                options.DocumentFilter<MimeTypeDocumentFilter>([
           "forms-delivery"
                ]);
                options.OperationFilter<SwaggerParameterAttributeFilter>([]);
            });
            builder.Services.Configure<UmbracoPipelineOptions>(options => options.AddFilter(new UmbracoPipelineFilter("FormBuilderOpenApi")
            {
                PrePipeline = app => app.UseWhen(context => context.Request.Path.StartsWithSegments((PathString)"/umbraco/forms/delivery/api", StringComparison.OrdinalIgnoreCase), appBuilder =>
                {
                    IWebHostEnvironment webHostEnvironment = app.ApplicationServices.GetRequiredService<IWebHostEnvironment>();
                    appBuilder.UseExceptionHandler(exceptionBuilder => exceptionBuilder.Run(async context =>
                    {
                        Exception? error = context.Features.Get<IExceptionHandlerPathFeature>()?.Error;
                        if (error == null)
                            return;
                        await context.Response.WriteAsJsonAsync(new ProblemDetails()
                        {
                            Title = error.Message,
                            Detail = webHostEnvironment.IsProduction() ? string.Empty : error.StackTrace,
                            Status = new int?(500),
                            Instance = error.GetType().Name,
                            Type = "Error"
                        }, context.RequestAborted);
                    }));
                })
            }));
            return builder;
        }

        private static IUmbracoBuilder AddFormsManagementApi(this IUmbracoBuilder builder)
        {
            builder.Services.Configure<SwaggerGenOptions>(options =>
            {
                options.SwaggerDoc("formBuilder-management", new OpenApiInfo()
                {
                    Title = "Form Builder Management API",
                    Version = "Latest",
                    Description = "Describes the Form Builder Management API available for managing forms data when authenticated as a backoffice user."
                });
                options.DocumentFilter<MimeTypeDocumentFilter>([
           "Form Builder Management API"
                ]);
                options.OperationFilter<SwaggerParameterAttributeFilter>([]);
                options.OperationFilter<BackOfficeSecurityRequirementsOperationFilter>([]);
            });
            builder.Services.AddSingleton<ISchemaIdHandler, FormsSchemaIdHandler>();
            builder.Services.AddSingleton<IOperationIdHandler, FormsOperationIdHandler>();
            return builder;
        }

        private static IUmbracoBuilder AddFormsAuthoriziation(
          this IUmbracoBuilder builder)
        {
            builder.Services.AddSingleton<IAuthorizationHandler, ManageDataSourceEntityHandler>();
            builder.Services.AddSingleton<IAuthorizationHandler, ManageFormEntityHandler>();
            builder.Services.AddSingleton<IAuthorizationHandler, ManagePrevalueSourceEntityHandler>();
            builder.Services.AddSingleton<IAuthorizationHandler, EditRecordEntityHandler>();
            builder.Services.AddSingleton<IAuthorizationHandler, ViewRecordEntityHandler>();
            builder.Services.AddAuthorization(CreatePolicies);
            return builder;
        }

        private static void CreatePolicies(AuthorizationOptions options)
        {
            options.AddPolicy("SectionAccessForms", policy =>
            {
                policy.AuthenticationSchemes.Add("OpenIddict.Validation.AspNetCore");
                policy.RequireClaim("http://formbuilder.com/2015/02/identity/claims/backoffice/allowedapp", "forms");
            });
            options.AddPolicy("ManageDataSources", policy =>
            {
                policy.AuthenticationSchemes.Add("OpenIddict.Validation.AspNetCore");
                policy.Requirements.Add(new ManageDataSourceEntityRequirement());
            });
            options.AddPolicy("ManageForms", policy =>
            {
                policy.AuthenticationSchemes.Add("OpenIddict.Validation.AspNetCore");
                policy.Requirements.Add(new ManageFormEntityRequirement());
            });
            options.AddPolicy("ManagePrevalueSources", policy =>
            {
                policy.AuthenticationSchemes.Add("OpenIddict.Validation.AspNetCore");
                policy.Requirements.Add(new ManagePrevalueSourceEntityRequirement());
            });
            options.AddPolicy("EditEntries", policy =>
            {
                policy.AuthenticationSchemes.Add("OpenIddict.Validation.AspNetCore");
                policy.Requirements.Add(new EditRecordEntityRequirement());
            });
            options.AddPolicy("ViewEntries", policy =>
            {
                policy.AuthenticationSchemes.Add("OpenIddict.Validation.AspNetCore");
                policy.Requirements.Add(new ViewRecordEntityRequirement());
            });
        }
    }
}