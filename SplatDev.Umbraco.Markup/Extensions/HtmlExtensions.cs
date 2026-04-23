using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace SplatDev.Umbraco.Common.Extensions
{
    public static partial class HtmlExtensions
    {
        public static string ConvertHtmlToPlainText(this string html)
        {
            if (string.IsNullOrEmpty(html))
                return string.Empty;

            // Replace <br> and <br/> with \n
            html = Br().Replace(html, "\n");

            // Replace <p> with \n
            html = P().Replace(html, "\n");

            // Remove the closing </p> tags
            html = PTag().Replace(html, "");

            // Remove all other HTML tags
            html = Tag().Replace(html, string.Empty);

            // Optional: Convert HTML entities to their corresponding characters
            html = System.Net.WebUtility.HtmlDecode(html);

            return html.Trim();
        }

        public static string GetWithoutAccent(this HtmlHelper html, string value)
        {
            ArgumentNullException.ThrowIfNull(html);

            var normalizedString = value.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }

        public static string RemoveParagraphWrapperTags(this string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return text;
            }

            string trimmedText = text.Trim();
            string upperText = trimmedText.ToUpper();
            int paragraphIndex = upperText.IndexOf("<P>");

            if (paragraphIndex != 0 ||
                paragraphIndex != upperText.LastIndexOf("<P>") ||
                upperText.Substring(upperText.Length - 4, 4) != "</P>")
            {
                // Paragraph not used as a wrapper element
                return text;
            }

            // Remove paragraph wrapper tags
            return trimmedText[3..^4];
        }

        #region Regex
        [GeneratedRegex(@"<br\s*/?>", RegexOptions.IgnoreCase)]
        private static partial Regex Br();
        [GeneratedRegex(@"<p\s*/?>", RegexOptions.IgnoreCase)]
        private static partial Regex P();
        [GeneratedRegex(@"</p>", RegexOptions.IgnoreCase)]
        private static partial Regex PTag();
        [GeneratedRegex(@"<[^>]+>")]
        private static partial Regex Tag();
        #endregion
    }
}