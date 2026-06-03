using Asp.Versioning;
using Microsoft.Extensions.DependencyInjection;

namespace SplatDev.Api.Common.ApiVersioning;

public static class ApiVersioningExtensions
{
    public static IServiceCollection AddSplatApiVersioning(this IServiceCollection services)
    {
        services.AddApiVersioning(options =>
        {
            options.DefaultApiVersion = new ApiVersion(1, 0);
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.ReportApiVersions = true;
            options.ApiVersionReader = ApiVersionReader.Combine(
                new QueryStringApiVersionReader("api-version"),
                new HeaderApiVersionReader("X-API-Version"));
        })
        .AddApiExplorer(setup =>
        {
            setup.GroupNameFormat = "'v'VVVV";
            setup.SubstituteApiVersionInUrl = true;
        });
        return services;
    }
}
