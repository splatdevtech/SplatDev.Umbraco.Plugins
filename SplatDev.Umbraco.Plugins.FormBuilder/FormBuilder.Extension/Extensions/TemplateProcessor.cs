using FormBuilder.Extension.Entities;

using System.Text.RegularExpressions;

namespace FormBuilder.Extension.Extensions
{
    public static partial class TemplateProcessor
    {
        private static readonly Regex PlaceholderRegex = PlaceholderRegexPattern();

        public static string ReplacePlaceholders(string template, IDictionary<string, FormField> data)
        {
            return PlaceholderRegex.Replace(template, match =>
            {
                var key = match.Groups[1].Value;
                return data.TryGetValue(key, out var value)
                    ? value?.ToString() ?? string.Empty
                    : match.Value;
            });
        }

        [GeneratedRegex(@"\{\{(\w+)\}\}", RegexOptions.Compiled)]
        public static partial Regex PlaceholderRegexPattern();
    }
}
