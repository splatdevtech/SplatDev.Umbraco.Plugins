using System.Text.RegularExpressions;

namespace QuoteTab.Umbraco.Core.Extensions
{
    public static partial class RuntimeMinifierExtensions
    {
        public static string Defer(this string value)
        {
            return value.AddAttributes(new Dictionary<string, string>() { { "defer", "" } });
        }

        public static string Async(this string value)
        {
            return value.AddAttributes(new Dictionary<string, string>() { { "async", "" } });
        }

        public static string PreloadJs(this string value)
        {
            return value.AddAttributes(new Dictionary<string, string>() { { "rel", "preload" }, { "as", "script" } });
        }

        public static string PreloadCss(this string value)
        {
            return value.AddAttributes(new Dictionary<string, string>() { { "rel", "preload" }, { "as", "style" } });
        }

        public static string AddAttributes(this string html, Dictionary<string, string> attributes)
        {
            if (string.IsNullOrEmpty(html) || attributes == null || attributes.Count == 0)
            {
                return html;
            }

            var regex = HtmlTag();
            var matches = regex.Matches(html);

            if (matches.Count == 0)
            {
                return html;
            }

            foreach (Match match in matches)
            {
                var tag = match.Value;
                var content = tag[1..^1];

                var parts = content.Split(' ').ToList();
                var element = parts[0];
                var existingAttributes = parts.Skip(1).ToList();

                foreach (var attribute in attributes)
                {
                    var name = attribute.Key;
                    var value = attribute.Value;

                    var index = existingAttributes.FindIndex(a => a.StartsWith(name + "="));

                    if (index >= 0)
                    {
                        existingAttributes[index] = $"{name}=\"{value}\"";
                    }
                    else
                    {
                        if (value == "")
                        {
                            existingAttributes.Add(name);
                        }
                        else
                        {
                            existingAttributes.Add($"{name}=\"{value}\"");
                        }
                    }
                }

                var newTag = $"<{element} {string.Join(" ", existingAttributes)}>";

                html = html.Replace(tag, newTag);
            }

            // Return the new html string
            return html;
        }

        [GeneratedRegex(@"<\w+[^>]*>")]
        public static partial Regex HtmlTag();
    }
}
