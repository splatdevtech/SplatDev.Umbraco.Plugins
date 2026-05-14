using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.DependencyInjection;

namespace SplatDev.Api.Common.ApiVersioning;

public static class ApiVersioningExtensions
{
    public static IServiceCollection AddSplatApiVersioning(this IServiceCollection services)
    {
        // Centralized API Versioning configuration
        services.AddApiVersioning(options => {
            options.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0);
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.ReportApiVersions = true;
            options.ApiVersionReader = Microsoft.AspNetCore.Mvc.Versioning.ApiVersionReader.Combine(
                new Microsoft.AspNetCore.Mvc.Versioning.QueryStringApiVersionReader("api-version"),
                new Microsoft.AspNetCore.Mvc.Versioning.HeaderApiVersionReader("X-API-Version"));
        });
        services.AddVersionedApiExplorer(setup => {
            setup.GroupNameFormat = "'v'VVVV";
            setup.SubstituteApiVersionInUrl = true;
        });
        return services;
    }
}
