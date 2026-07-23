using FormBuilder.Core;
using FormBuilder.Core.Configuration;
using FormBuilder.Core.Factory;
using FormBuilder.Core.Filters;
using FormBuilder.Core.Handlers;
using FormBuilder.Core.Models;
using FormBuilder.Core.Providers.Collections;
using FormBuilder.Core.Services;
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
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;

using Swashbuckle.AspNetCore.SwaggerGen;

using Umbraco.Cms.Api.Common.OpenApi;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.Mapping;
using Umbraco.Cms.Web.Common.ApplicationBuilder;
using Umbraco.Extensions;

namespace FormBuilder.Web.Extensions
{
    /// <summary>
    /// Extension methods for     /// </summary>
    public static class UmbracoBuilderExtensions
    {
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
                if (requiredService1.PartialViewsFileSystem is null)
                    throw new InvalidOperationException("Could not find partial views file system");
                return new FormThemeResolver(requiredService1.PartialViewsFileSystem, requiredService2, requiredService3);
            });
            builder.Services.AddSingleton<ProtectFormUploadRequestsMiddleware>();
            builder.Services.Configure((Action<UmbracoPipelineOptions>)(options => options.AddFilter(new UmbracoPipelineFilter("ProtectFormUploadRequestsMiddleware")
            {
                PrePipeline = app => app.UseMiddleware<ProtectFormUploadRequestsMiddleware>()
            })));
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
                    Description = "Describes the Form Builder Delivery API available for rendering and submitting forms. "
                });
                options.DocumentFilter<MimeTypeDocumentFilter>(
                [
                    "forms-delivery"
                ]);
                options.OperationFilter<SwaggerParameterAttributeFilter>([]);
            });
            builder.Services.Configure((Action<UmbracoPipelineOptions>)(options => options.AddFilter(new UmbracoPipelineFilter("FormBuilderOpenApi")
            {
                PrePipeline = app => app.UseWhen(context => context.Request.Path.StartsWithSegments((PathString)"/formBuilder/delivery/api", StringComparison.OrdinalIgnoreCase), appBuilder =>
                {
                    IWebHostEnvironment webHostEnvironment = app.ApplicationServices.GetRequiredService<IWebHostEnvironment>();
                    appBuilder.UseExceptionHandler(exceptionBuilder => exceptionBuilder.Run(async context =>
                    {
                        Exception? error = context.Features.Get<IExceptionHandlerPathFeature>()?.Error;
                        if (error is null)
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
            })));
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
                options.DocumentFilter<MimeTypeDocumentFilter>(
                [
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