namespace SplatDev.Routing
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Routing;
    using Microsoft.Extensions.DependencyInjection;
    using SplatDev.Routing.Interfaces;
    using System;
    using System.Linq;
    using System.Reflection;

    public static class RouteServiceExtensions
    {
        public static IEndpointRouteBuilder MapSplatDevRoutes(this IEndpointRouteBuilder endpoints, params Assembly[] assemblies)
        {
            if (assemblies.Length == 0)
            {
                throw new ArgumentException("At least one assembly must be provided.", nameof(assemblies));
            }

            foreach (var assembly in assemblies)
            {
                var routeTypes = assembly.GetTypes()
                    .Where(t => typeof(IRoute).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

                foreach (var routeType in routeTypes)
                {
                    var route = (IRoute)ActivatorUtilities.CreateInstance(endpoints.ServiceProvider, routeType);
                    endpoints.MapControllerRoute(
                        name: route.RouteAlias,
                        pattern: route.Url,
                        defaults: route.Defaults ?? new { controller = route.Controller, action = route.Action }
                    );
                }
            }

            return endpoints;
        }
    }
}
