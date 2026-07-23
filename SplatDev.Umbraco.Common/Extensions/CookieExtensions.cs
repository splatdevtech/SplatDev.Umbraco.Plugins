using Microsoft.AspNetCore.Http;

namespace SplatDev.Umbraco.Common.Extensions
{
    public static class CookieExtensions
    {
        // SaveCookie
        // Stores a value inside a cookie
        public static void SaveCookie(this HttpContext context, string cookieName, string cookieValue, CookieOptions options) =>
            context.Response.Cookies.Append(cookieName, cookieValue, options);


        // ReadCookie
        // Reads a value from a cookie
        public static string? ReadCookie(this HttpContext context, string cookieName)
        {
            context.Request.Cookies.TryGetValue(cookieName, out string? cookieValue);
            return cookieValue;
        }
    }
}
