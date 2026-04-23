using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Cryptography;
using System.Text;
using UmbracoCms.Plugins.VisitorCounter.Services;

namespace UmbracoCms.Plugins.VisitorCounter.Middleware;

/// <summary>
/// Tracks visitor sessions via a cookie.  When no cookie is present the visitor's
/// IP address is hashed as a fallback (privacy-preserving; no raw IP stored).
/// </summary>
public class VisitorCounterMiddleware : IMiddleware
{
    private const string CookieName = "_vcid";
    private static readonly TimeSpan CookieLifetime = TimeSpan.FromDays(30);

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        // Resolve or create the session identifier before the response is sent
        var sessionId = ResolveSessionId(context);

        await next(context);

        // Only count successful HTML responses; skip backoffice / API / media
        if (context.Response.StatusCode != 200)
            return;

        var path = context.Request.Path.Value ?? string.Empty;
        if (path.StartsWith("/umbraco", StringComparison.OrdinalIgnoreCase) ||
            path.StartsWith("/media", StringComparison.OrdinalIgnoreCase))
            return;

        // Set / refresh the cookie (best-effort; headers may already be sent)
        TrySetCookie(context, sessionId);

        // Fire-and-forget to avoid blocking the response
        _ = Task.Run(async () =>
        {
            try
            {
                using var scope = context.RequestServices.CreateScope();
                var service = scope.ServiceProvider.GetRequiredService<IVisitorCounterService>();
                await service.RecordVisitAsync(sessionId, path);
            }
            catch
            {
                // Silently ignore tracking errors
            }
        });
    }

    private static string ResolveSessionId(HttpContext context)
    {
        // Prefer an existing cookie value
        if (context.Request.Cookies.TryGetValue(CookieName, out var cookieValue) &&
            !string.IsNullOrWhiteSpace(cookieValue))
        {
            return cookieValue;
        }

        // Generate a new random session ID
        return Guid.NewGuid().ToString("N");
    }

    private static void TrySetCookie(HttpContext context, string sessionId)
    {
        try
        {
            if (!context.Response.HasStarted)
            {
                context.Response.Cookies.Append(CookieName, sessionId, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = context.Request.IsHttps,
                    SameSite = SameSiteMode.Lax,
                    MaxAge = CookieLifetime,
                    IsEssential = false
                });
            }
        }
        catch
        {
            // Ignore if headers already sent
        }
    }

    /// <summary>Creates a SHA-256 hash of the IP address for privacy-preserving fallback identification.</summary>
    private static string HashIp(string ip)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(ip));
        return Convert.ToHexString(bytes)[..16]; // 16 hex chars = 8 bytes
    }
}
