using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.Web;
using UmbracoCms.Plugins.MostViewed.Services;

namespace UmbracoCms.Plugins.MostViewed.Middleware;

/// <summary>
/// Records a page view whenever a request is served by the Umbraco content pipeline.
/// Must be registered after Umbraco routing middleware so the Umbraco context is available.
/// </summary>
public class PageViewMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        await next(context);

        // Only track successful HTML responses for Umbraco content
        if (context.Response.StatusCode != 200)
            return;

        // Ignore backoffice, API, and media routes
        var path = context.Request.Path.Value ?? string.Empty;
        if (path.StartsWith("/umbraco", StringComparison.OrdinalIgnoreCase) ||
            path.StartsWith("/media", StringComparison.OrdinalIgnoreCase))
            return;

        var umbracoContextAccessor = context.RequestServices
            .GetService<IUmbracoContextAccessor>();

        if (umbracoContextAccessor is null ||
            !umbracoContextAccessor.TryGetUmbracoContext(out var umbracoContext))
            return;

        var publishedRequest = umbracoContext.PublishedRequest;
        if (publishedRequest?.PublishedContent is null)
            return;

        var content = publishedRequest.PublishedContent;
        var service = context.RequestServices.GetService<IMostViewedService>();
        if (service is null) return;

        var viewerIp = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        var nodeName = content.Name ?? string.Empty;
        var nodeUrl = path;

        // Fire-and-forget to avoid slowing down the response
        _ = Task.Run(async () =>
        {
            try
            {
                using var scope = context.RequestServices.CreateScope();
                var scopedService = scope.ServiceProvider.GetRequiredService<IMostViewedService>();
                await scopedService.RecordViewAsync(content.Key, nodeName, nodeUrl, viewerIp);
            }
            catch
            {
                // Silently ignore tracking errors
            }
        });
    }
}
