using FormBuilder.Core.Helpers;

using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

using Umbraco.Cms.Core.Dictionary;

namespace FormBuilder.Extension.Forms.Core.Helpers
{
    public partial class DictionaryHelper(ICultureDictionaryFactory cultureDictionaryFactory) : IDictionaryHelper
    {
        private readonly ICultureDictionaryFactory _cultureDictionaryFactory = cultureDictionaryFactory;

        public string GetText(string text)
        {
            if (IsSingleDictionaryReplacement(text))
                return ReplaceSingleDictionaryKey(text);
            return IsHtmlWithDictionaryReplacements(text, out MatchCollection? matches) ? ReplaceMatches(text, matches) : text;
        }

        private static bool IsSingleDictionaryReplacement(string text) => text.Trim().StartsWith('#');

        private string ReplaceSingleDictionaryKey(string text)
        {
            string key = text.Trim().TrimStart('#');
            string str = _cultureDictionaryFactory.CreateDictionary()[key] ?? string.Empty;
            return str.Length <= 0 ? key : str;
        }

        private static bool IsHtmlWithDictionaryReplacements(string text, [NotNullWhen(true)] out MatchCollection? matches)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                matches = null;
                return false;
            }
            Regex regex = Replacement();
            matches = regex.Matches(text);
            return matches.Count > 0;
        }

        private string ReplaceMatches(string text, MatchCollection matches)
        {
            string str1 = text;
            foreach (Match match in matches)
            {
                string str2 = ReplaceSingleDictionaryKey(match.Value.Trim('>', '<', '#'));
                str1 = str1.Replace(match.Value, ">" + str2 + "<");
            }
            return str1;
        }

        [GeneratedRegex("\\>#(.+?)\\<")]
        public static partial Regex Replacement();
    }
}