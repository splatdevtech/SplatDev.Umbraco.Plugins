namespace SplatDev.Html.Helpers
{
    using Microsoft.AspNetCore.Http;
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Text.RegularExpressions;

    public static class HtmlHelpers
    {
        private static readonly Regex _htmlRegex = new Regex("<.*?>", RegexOptions.Compiled);
        private static readonly Random random = new Random();

        public static string BuildUrl(HttpContext context, string page = "", string queryString = "")
        {
            var baseUri = new Uri(RootDomain(context, withProtocol: true));
            var fullUrl = new UriBuilder(Protocol(context), baseUri.Host, baseUri.Port, page, queryString);
            return fullUrl.ToString();
        }

        public static string CleanUrl(this string value)
        {
            if (string.IsNullOrEmpty(value)) return value;
            value = value.Replace("-", " ").Trim().ToLower();
            value = Regex.Replace(value, @"[\s]+", "-");
            value = value.Replace("ß", "ss").Replace("ä", "ae").Replace("ö", "oe").Replace("ü", "ue");
            value = value.Replace("?", "").Replace("!", "").Replace("@", "").Replace("#", "")
                .Replace("$", "").Replace("%", "").Replace("&", "").Replace("*", "")
                .Replace("(", "").Replace(")", "").Replace(".", "").Replace(",", "")
                .Replace(";", "").Replace(":", "").Replace("'", "").Replace("~", "");
            value = RemoveDiacritics(value);
            return Regex.Replace(value, @"[^a-z0-9\s-]", string.Empty);
        }

        public static string ConvertLineBreakToHtml(this string value)
        {
            if (string.IsNullOrEmpty(value)) return "<br />";
            value = value.Replace(Environment.NewLine, "<br />");
            value = value.Replace("\r\n", "<br />");
            value = value.Replace("\r", "<br />");
            return value.Replace("\n", "<br />");
        }

        public static string DecodeUrl(this string url) => WebUtility.UrlDecode(url);

        public static string EncodeUrl(this string url) => WebUtility.UrlEncode(url);

        public static string GetMIMEType(string fileId)
        {
            if (fileId.EndsWith(".js")) return "text/javascript";
            if (fileId.EndsWith(".html")) return "text/html";
            if (fileId.EndsWith(".css")) return "text/css";
            if (fileId.EndsWith(".jpg")) return "image/jpeg";
            if (fileId.EndsWith(".jpeg")) return "image/jpeg";
            if (fileId.EndsWith(".png")) return "image/png";
            if (fileId.EndsWith(".gif")) return "image/gif";
            if (fileId.EndsWith(".bmp")) return "image/bmp";
            if (fileId.EndsWith(".svg")) return "image/svg+xml";
            return "text/plain";
        }

        public static string GetUserIP(HttpContext context)
        {
            var forwardedFor = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (!string.IsNullOrEmpty(forwardedFor))
                return forwardedFor.Split(',')[0].Trim();
            return context.Connection.RemoteIpAddress?.ToString() ?? string.Empty;
        }

        public static bool IsHttps(HttpContext context) => context.Request.IsHttps;

        public static string Protocol(HttpContext context, string url = "")
        {
            var scheme = context.Request.Scheme;
            return string.IsNullOrEmpty(url) ? scheme : $"{scheme}://{url}";
        }

        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public static string RemoveDiacritics(string value)
        {
            if (string.IsNullOrEmpty(value)) return value;
            string normalized = value.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();
            foreach (char c in normalized)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                    sb.Append(c);
            }
            return sb.ToString().Normalize(NormalizationForm.FormC);
        }

        public static string RootDomain(HttpContext context, bool withProtocol = false)
        {
            var host = context.Request.Host.Value;
            return withProtocol ? $"{context.Request.Scheme}://{host}" : host;
        }

        public static string RootUrl(HttpContext context) => BuildUrl(context);

        public static string Sanitize(this string value)
        {
            if (string.IsNullOrEmpty(value)) return string.Empty;
            value = value.Trim().ToLower()
                .Replace("-", "").Replace(" ", "").Replace("?", "").Replace("!", "")
                .Replace("@", "").Replace("#", "").Replace("$", "").Replace("%", "")
                .Replace("&", "").Replace("*", "").Replace("(", "").Replace(")", "")
                .Replace(".", "").Replace(",", "").Replace(";", "").Replace(":", "")
                .Replace("'", "").Replace("~", "");
            return Regex.Replace(value, "[^a-zA-Z0-9 -]", "");
        }

        public static string SanitizeUrl(this string value)
        {
            if (string.IsNullOrEmpty(value)) return string.Empty;
            value = value.Trim().ToLower().Replace("-", "").Replace(" ", "");
            return Regex.Replace(value, "[^a-zA-Z0-9 -]", "");
        }

        public static string SearchEngineFormat(this string text) => text.Replace(" ", "+");

        public static string StripHtmlTags(this string source) => _htmlRegex.Replace(source, string.Empty);

        public static string StripParagraph(this string html)
        {
            if (html.Length > 5)
            {
                html = html.Trim();
                string htmlLower = html.ToLower();
                if (htmlLower.Substring(0, 3) == "<p>"
                    && htmlLower.Substring(html.Length - 4, 4) == "</p>"
                    && htmlLower.IndexOf("<p>", 1) < 0)
                {
                    html = html.Substring(3, html.Length - 7);
                }
            }
            return html;
        }

        public static string StripTagsCharArray(this string source)
        {
            char[] array = new char[source.Length];
            int arrayIndex = 0;
            bool inside = false;
            for (int i = 0; i < source.Length; i++)
            {
                char let = source[i];
                if (let == '<') { inside = true; continue; }
                if (let == '>') { inside = false; continue; }
                if (!inside) { array[arrayIndex] = let; arrayIndex++; }
            }
            return new string(array, 0, arrayIndex);
        }

        public static string GetAbsoluteDomain(HttpContext context, bool withPort = false)
        {
            var request = context.Request;
            string port = withPort ? $":{request.Host.Port}" : "";
            return $"{request.Scheme}://{request.Host.Host}{port}";
        }
    }
}
