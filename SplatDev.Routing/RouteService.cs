namespace SplatDev.Routing
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Routing;
    using SplatDev.Routing.Interfaces;
    using System;
    using System.Linq;
    using System.Reflection;

    public static class RouteServiceExtensions
    {
        /// <summary>
        /// Scans the provided assemblies for IRoute implementations and registers each as a conventional route.
        /// </summary>
        public static IEndpointRouteBuilder MapSplatDevRoutes(this IEndpointRouteBuilder endpoints, params Assembly[] assemblies)
        {
            var targetAssemblies = assemblies.Length > 0 ? assemblies : AppDomain.CurrentDomain.GetAssemblies();

            foreach (var assembly in targetAssemblies)
            {
                var routeTypes = assembly.GetTypes()
                    .Where(t => typeof(IRoute).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

                foreach (var routeType in routeTypes)
                {
                    var route = (IRoute)Activator.CreateInstance(routeType)!;
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
