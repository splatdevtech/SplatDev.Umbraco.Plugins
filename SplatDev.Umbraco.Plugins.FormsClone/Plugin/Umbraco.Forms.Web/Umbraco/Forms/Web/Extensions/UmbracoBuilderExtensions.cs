
// Type: Umbraco.Forms.Web.Extensions.UmbracoBuilderExtensions
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

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
using Umbraco.Forms.Core;
using Umbraco.Forms.Core.Configuration;
using Umbraco.Forms.Core.Models.DeliveryApi;
using Umbraco.Forms.Core.Providers;
using Umbraco.Forms.Web.Api.Configuration;
using Umbraco.Forms.Web.Authorization.EntityPermission;
using Umbraco.Forms.Web.Behaviors;
using Umbraco.Forms.Web.HttpModules;
using Umbraco.Forms.Web.Models.Mapping;
using Umbraco.Forms.Web.Services;


#nullable enable
namespace Umbraco.Forms.Web.Extensions
{
    public static class UmbracoBuilderExtensions
    {
        public static IUmbracoBuilder AddUmbracoFormsWeb(this IUmbracoBuilder builder)
        {
            builder.Services.AddSingleton<IApplyDefaultFieldsBehavior, ApplyDefaultFieldsBehavior>();
            builder.Services.AddSingleton<IApplyDefaultWorkflowsBehavior, ApplyDefaultWorkflowsBehavior>();
            builder.WithCollectionBuilder<MapDefinitionCollectionBuilder>().Add<FormDesignMapping>();
            builder.Services.AddScoped<IFormRenderingService, FormRenderingService>();
            builder.Services.AddUnique<IFormThemeResolver>(provider =>
            {
                FileSystems requiredService1 = ServiceProviderServiceExtensions.GetRequiredService<FileSystems>(provider);
                IOptions<FormDesignSettings> requiredService2 = ServiceProviderServiceExtensions.GetRequiredService<IOptions<FormDesignSettings>>(provider);
                ThemeCollection requiredService3 = ServiceProviderServiceExtensions.GetRequiredService<ThemeCollection>(provider);
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
            builder.Services.Configure<UmbracoPipelineOptions>(options => options.AddFilter(new UmbracoPipelineFilter("UmbracoFormsOpenApi")
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
                SwaggerGenOptionsExtensions.SwaggerDoc(options, "forms-management", new OpenApiInfo()
                {
                    Title = "Umbraco Forms Management API",
                    Version = "Latest",
                    Description = "Describes the Umbraco Forms Management API available for managing forms data when authenticated as a backoffice user."
                });
                SwaggerGenOptionsExtensions.DocumentFilter<MimeTypeDocumentFilter>(options, new object[1]
                {
           "Umbraco Forms Management API"
                });
                SwaggerGenOptionsExtensions.OperationFilter<SwaggerParameterAttributeFilter>(options, Array.Empty<object>());
                SwaggerGenOptionsExtensions.OperationFilter<BackOfficeSecurityRequirementsOperationFilter>(options, Array.Empty<object>());
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
                policy.RequireClaim("http://umbraco.org/2015/02/identity/claims/backoffice/allowedapp", "forms");
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
