using Microsoft.AspNetCore.Hosting;

using System.Collections;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

using Umbraco.Cms.Core.Models.Membership;
using Umbraco.Extensions;

namespace SplatDev.Umbraco.Plugins.Mailer.Extensions
{
    public static partial class TemplateExtensions
    {
        public static string GetRazorHtml(this IWebHostEnvironment environment, string alias)
        {
            var path = environment.ContentRootPath + $"/Views/Partials/Emails/{alias}.cshtml";
            var body = new StringBuilder();
            using (var reader = new StreamReader(path))
            {
                body.Append(reader.ReadToEnd());
            }
            return body.ToString();
        }

        public static string GetFileHtml(this IWebHostEnvironment environment, string folder, string alias)
        {
            var path = environment.ContentRootPath + $"/{folder}/{alias}.cshtml";
            var body = new StringBuilder();
            using (var reader = new StreamReader(path))
            {
                body.Append(reader.ReadToEnd());
            }
            return body.ToString();
        }

        public static string ReplaceTokens(this string body, Dictionary<string, string> tokens)
        {
            foreach (var token in tokens)
            {
                if (!string.IsNullOrEmpty(token.Value))
                    body = body.Replace($"%%{token.Key}%%", WebUtility.HtmlEncode(token.Value));
            }
            return body;
        }

        [GeneratedRegex(@"^(?<base>\w+)\s*\[\s*""(?<key>[^""]+)""\s*\]$")]
        private static partial Regex ClassName();
        private static readonly Regex ClassNameRegex = ClassName();

        public static object? GetValue(List<object> sources, string className, string propertyName)
        {
            var filteredSources = sources.Where(x => x != null).ToList();

            // Check if className contains indexer notation, e.g., "PropertyName[\"Windows\"]"
            var match = ClassNameRegex.Match(className);
            if (match.Success)
            {
                string baseClassName = match.Groups["base"].Value;
                string key = match.Groups["key"].Value;

                // Find the source object (the parent object that has the PropertyName property)
                var source = filteredSources.FirstOrDefault(x => x.GetType().Name == baseClassName);

                // Defensive: Ensure source is not null and is a collection with an indexer
                if (source == null || !source.GetType().Inherits<IList>()) return null;

                // Use the indexer (this[string key]) to get the item
                var indexer = source.GetType()
                    .GetDefaultMembers()
                    .OfType<PropertyInfo>()
                    .FirstOrDefault(p =>
                    {
                        var idxParams = p.GetIndexParameters();
                        return idxParams.Length == 1 && idxParams[0].ParameterType == typeof(string);
                    });

                if (indexer != null)
                {
                    var item = indexer.GetValue(source, [key]);
                    if (item != null)
                    {
                        var subProp = item.GetType().GetProperty(propertyName);
                        var value = subProp?.GetValue(item);

                        // DateTime formatting (handles both DateTime and DateTime?)
                        if (value is DateTime dt)
                            return dt.ToString("MMM dd, yyyy");

                        return value;
                    }
                }
                return null;
            }
            else
            {
                // Original behavior for simple class/property access
                var source = filteredSources.FirstOrDefault(x => x.GetType().Name == className);
                if (source == null) return null;

                var prop = source.GetType().GetProperty(propertyName);
                if (prop != null)
                {
                    return prop.GetValue(source);
                }
                return null;
            }
        }


        public static string ObfuscateTokens(this string body, List<string> tokens)
        {
            foreach (var token in tokens)
            {
                body = body.Replace($"%%{token}%%", "XXXXX");
            }
            return body;
        }

        [GeneratedRegex(@"%%(.*?)%%", RegexOptions.Compiled)]
        private static partial Regex Token();
        private static readonly Regex TokenRegex = Token();

        public static List<string> GetTokens(this string body)
        {
            if (string.IsNullOrEmpty(body))
                return [];

            var matches = TokenRegex.Matches(body);

            // Select the group inside the %%...%%
            var tokens = matches
                .Cast<Match>()
                .Select(m => m.Groups[1].Value)
                .Distinct()
                .ToList();

            return tokens;
        }

        public static Dictionary<string, string> GetTokenValues(this List<object> sources, List<string> properties)
        {
            var dictionary = new Dictionary<string, string>();
            if (sources is null || sources.Count == 0) throw new MissingMemberException();
            foreach (var propertyName in properties)
            {
                var propParts = propertyName.Split('.');
                var className = propParts[0];
                var prop = propParts.Length > 1 ? propParts[1] : propParts[0];
                var value = GetValue(sources, className, prop);

                if (value is not null)
                {
                    var valueString = value.GetType().Equals(typeof(bool)) ? (bool)value ? "Yes" : "No" : value.ToString();
                    dictionary.Add(propertyName, valueString!);
                }
                else
                {
                    dictionary.Add(propertyName, "");
                }
            }
            return dictionary;
        }

        public static Dictionary<string, string> GetTokenValues(this IUser user, List<string> properties)
        {
            var dictionary = new Dictionary<string, string>();
            if (user is null) throw new MissingMemberException();
            foreach (var propertyName in properties)
            {
                if (propertyName.Equals("name")) dictionary.Add(propertyName, user.Name!);
                else if (propertyName.Equals("email")) dictionary.Add(propertyName, user.Email);
                else if (propertyName.Equals("groups")) dictionary.Add(propertyName, string.Join(", ", user.Groups));
                else dictionary.Add(propertyName, "");
            }
            return dictionary;
        }
    }
}