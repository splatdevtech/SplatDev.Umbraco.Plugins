using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using SplatDev.Umbraco.Plugins.CustomLogin.Models;
using SplatDev.Umbraco.Plugins.CustomLogin.Services;

namespace SplatDev.Umbraco.Plugins.CustomLogin.Middleware;

public class CustomLoginAssetsMiddleware(RequestDelegate next)
{
    private const string BasePath = "/App_Plugins/CustomLogin/generated";

    public async Task InvokeAsync(HttpContext context)
    {
        var path = context.Request.Path.Value;

        if (path is not null && path.StartsWith(BasePath, StringComparison.OrdinalIgnoreCase))
        {
            var service = context.RequestServices.GetRequiredService<ICustomLoginService>();
            var settings = service.GetSettings();

            if (path.EndsWith("/login-overrides.css", StringComparison.OrdinalIgnoreCase))
            {
                context.Response.ContentType = "text/css; charset=utf-8";
                context.Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
                await context.Response.WriteAsync(LoginPageCssGenerator.Generate(settings));
                return;
            }

            if (path.EndsWith("/en.js", StringComparison.OrdinalIgnoreCase))
            {
                context.Response.ContentType = "application/javascript; charset=utf-8";
                context.Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
                await context.Response.WriteAsync(LoginPageLocalizationGenerator.GenerateEnglish(settings));
                return;
            }

            if (path.EndsWith("/es.js", StringComparison.OrdinalIgnoreCase))
            {
                context.Response.ContentType = "application/javascript; charset=utf-8";
                context.Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
                await context.Response.WriteAsync(LoginPageLocalizationGenerator.GenerateSpanish(settings));
                return;
            }

            if (path.EndsWith("/login-entry.js", StringComparison.OrdinalIgnoreCase))
            {
                context.Response.ContentType = "application/javascript; charset=utf-8";
                context.Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
                await context.Response.WriteAsync(GenerateEntryPointJs(settings));
                return;
            }
        }

        await next(context);
    }

    private static string GenerateEntryPointJs(CustomLoginSettings settings)
    {
        var sb = new System.Text.StringBuilder();
        sb.AppendLine("// SplatDev Custom Login - Entry Point");
        sb.AppendLine("const link = document.createElement('link');");
        sb.AppendLine("link.rel = 'stylesheet';");
        sb.AppendLine("link.href = '/App_Plugins/CustomLogin/generated/login-overrides.css';");
        sb.AppendLine("document.head.appendChild(link);");

        if (!string.IsNullOrWhiteSpace(settings.FaviconUrl))
        {
            sb.AppendLine("const favicon = document.querySelector(\"link[rel*='icon']\") || document.createElement('link');");
            sb.AppendLine("favicon.rel = 'icon';");
            sb.AppendLine($"favicon.href = '{EscapeJs(settings.FaviconUrl)}';");
            sb.AppendLine("document.head.appendChild(favicon);");
        }

        if (!string.IsNullOrWhiteSpace(settings.LoginPageTitle))
        {
            sb.AppendLine($"document.title = '{EscapeJs(settings.LoginPageTitle)}';");
        }

        return sb.ToString();
    }

    private static string EscapeJs(string value) =>
        value.Replace("\\", "\\\\").Replace("'", "\\'").Replace("\n", "\\n").Replace("\r", "");
}
