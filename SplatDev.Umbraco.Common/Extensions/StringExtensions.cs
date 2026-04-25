using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;

using Umbraco.Extensions;

namespace SplatDev.Umbraco.Common.Extensions
{
    public static partial class StringExtensions
    {
        public static string ApplyTrailingSlash(this string url)
        {
            var trailingSlash = "/";

            if (url.EndsWith(trailingSlash))
                return url;

            return $"{url}{trailingSlash}";
        }

        public static string CamelCaseToDashed(this string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            StringBuilder result = new(input.Length + 10); // +10 is an estimate for dashes
            result.Append(input[0]);

            for (int i = 1; i < input.Length; i++)
            {
                if (char.IsUpper(input[i]))
                {
                    result.Append('-');
                }
                result.Append(input[i]);
            }

            return result.ToString().ToLower(); // Convert to lowercase if needed
        }

        public static string CapitalizeFirstLetters(this string input)
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;

            return string.Join(" ", input.Split(' ')
                .Select(word => char.ToUpper(word[0]) + word[1..].ToLower()));
        }

        public static string CapitalizeFirstLettersDash(this string input)
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;

            return string.Join("", input.Split('_')
                .Select(word => char.ToUpper(word[0]) + word[1..]));
        }

        public static string ComputeFourDigitStringHash(string filePath)
        {
            string filename = System.IO.Path.GetFileNameWithoutExtension(filePath);
            int hash = filename.GetHashCode() % 10000;
            return hash.ToString("0000");
        }

        public static int CountWords(this string text)
        {
            int wordCount = 0, index = 0;

            // skip whitespace until first word
            while (index < text.Length && char.IsWhiteSpace(text[index]))
                index++;
            while (index < text.Length)
            {
                // check if current char is part of a word
                while (index < text.Length && !char.IsWhiteSpace(text[index]))
                    index++;

                wordCount++;

                // skip whitespace until next word
                while (index < text.Length && char.IsWhiteSpace(text[index]))
                    index++;
            }

            return wordCount;
        }


        public static string DashCaseToSpacedWords(this string input)
        {
            StringBuilder sb = new();

            CultureInfo cultureInfo = new("en-US");
            TextInfo textInfo = cultureInfo.TextInfo;

            string[] words = input.Split('-');
            foreach (string word in words)
            {
                if (!string.IsNullOrEmpty(word))
                {
                    sb.Append(textInfo.ToTitleCase(word));
                    sb.Append(' ');
                }
            }
            string result = sb.ToString().TrimEnd();
            return result;
        }

        public static string DateSlug(int day, int month)
        {
            try
            {
                DateTime date = new(DateTime.Now.Year, month, day);
                return date.ToString("MMMM-dd");
            }
            catch // if can't create new date, return blank
            {
                return "";
            }
        }

        public static string Encode(this string text)
        {
            return text.Replace("'", "\'");
        }
        public static string FirstCharToUpper(this string input) =>
            input switch
            {
                null => throw new ArgumentNullException(nameof(input)),
                "" => throw new ArgumentException($"{nameof(input)} cannot be empty", nameof(input)),
                _ => string.Concat(input[0].ToString().ToUpper(), input.AsSpan(1))
            };

        public static string FormatDateString(this DateTime date, string format = "dd MMMM yyyy")
        {
            return date.ToString(format);
        }

        public static string FormattedDate(int day, int month, int year, bool abbreviated = false)
        {
            DateTime date = new(year, month, day);
            return abbreviated ? date.ToString("dd MMM yy") : date.ToString("dd MMMM yyyy");
        }

        public static string GenerateRandomUrl(int size = 7)
        {
            return RandomString(size);
        }

        public static string GenerateSlug(this string phrase)
        {
            var s = phrase.ToLower();
            s = InvalidChars().Replace(s, ""); // remove invalid characters
            s = SingleSpace().Replace(s, " ").Trim(); // single space
            s = s[..(s.Length <= 45 ? s.Length : 45)].Trim(); // cut and trim
            s = Hyphens().Replace(s, "-"); // insert hyphens

            if (string.IsNullOrEmpty(s.Trim()))
                s = "title";

            return s.ToLower();
        }

        public static string GetMonthName(int year, int month, bool abbreviated = false)
        {
            DateTime date = new(year, month, 1);
            return abbreviated ? date.ToString("MMM") : date.ToString("MMMM");
        }

        public static string GetTwoCharLangCode(this string langCode, bool isUppercase = false)
        {
            var code = langCode.Contains('-') ? langCode.Split('-').First().ToUpperInvariant() : langCode.ToUpperInvariant();

            return isUppercase ? code.ToUpperInvariant() : code.ToLowerInvariant();
        }

        public static string GetXmlAsUtf8String(XDocument xDocument)
        {
            MemoryStream memoryStream = new();
            using XmlWriter xmlWriter = new XmlTextWriter(memoryStream, Encoding.UTF8);
            xDocument.Save(xmlWriter);
            xmlWriter.Flush();
            StreamReader streamReader = new(memoryStream);
            memoryStream.Seek(0, SeekOrigin.Begin);
            return streamReader.ReadToEnd();
        }

        public static string LowerCaseFirstLetter(this string text)
        {
            return (text[0].ToString().ToLower() + text[1..]).Replace(" ", "");
        }

        public static string NameToId(this string name)
        {
            return name.ToLower().Replace(".", "").Replace(",", "").Replace(" ", "-");
        }

        public static string NameToSlug(this string name)
        {
            if (string.IsNullOrEmpty(name))
                return string.Empty;

            // Convert to lowercase
            name = name.ToLowerInvariant();

            // Remove diacritics (accents)
            name = Encoding.ASCII.GetString(
                Encoding.GetEncoding("Cyrillic").GetBytes(name)
            );

            // Replace spaces and special characters with hyphens
            name = NamePatternOne().Replace(name, "");

            // Replace multiple spaces with single hyphen
            name = NamePatternTwo().Replace(name, "-");

            // Trim hyphens from start and end
            name = name.Trim('-');

            return name;
        }

        public static string NameToSnake(this string name)
        {
            if (string.IsNullOrEmpty(name))
                return string.Empty;

            // Convert to lowercase
            name = name.ToLowerInvariant();

            // Remove diacritics (accents)
            name = Encoding.ASCII.GetString(
                Encoding.GetEncoding("Cyrillic").GetBytes(name)
            );

            // Replace non-alphanumeric characters with underscores
            name = NamePatternThree().Replace(name, "_");

            // Remove leading and trailing underscores
            name = name.Trim('_');

            // Replace multiple consecutive underscores with a single underscore
            name = Underscores().Replace(name, "_");

            return name;
        }

        public static int ParseMonthNameToInt(string monthName)
        {
            if (string.IsNullOrEmpty(monthName)) return -1;

            if (DateTime.TryParseExact(monthName, "MMMM", CultureInfo.CurrentCulture, DateTimeStyles.AssumeUniversal, out var date))
            {
                return date.Month;
            }
            return -1;
        }

        public static string Slugify(this string name)
        {
            return name.NameToSlug();
        }

        public static string SnakeToCamel(this string text)
        {
            return string.Concat(text.Split('_').Select((word, index) => index == 0 ? word : char.ToUpper(word[0]) + word[1..]));
        }

        public static string SnakeToCapitalized(this string text, bool addSpaces = false)
        {
            return string.Concat(text.Split('_').Select((word, index) => index == 0 ? word : char.ToUpper(word[0]) + word[1..]), (addSpaces ? " " : ""));
        }

        public static string StripHtmlTags(this string input)
        {
            return HtmlTags().Replace(input, string.Empty);
        }

        public static string TextToId(this string text)
        {
            return text.ToLower().Replace(" ", "-");
        }

        public static string ToBase64(this string target)
        {
            if (string.IsNullOrEmpty(target))
                return target;

            return System.Convert.ToBase64String(Encoding.UTF8.GetBytes(target));
        }

        public static string ToHash(this string? str)
        {
            if (!string.IsNullOrEmpty(str))
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(str);
                byte[] hashBytes = MD5.HashData(inputBytes);
                StringBuilder sb = new();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("x2"));
                }
                return sb.ToString();
            }
            else
            {
                return "default";
            }
        }

        public static IEnumerable<string> Tokenize(string input)
        {
            return [.. TokenizeRegex().Matches(input)
                .Cast<Match>()
                .Select(m => m.Value.Trim('\"').ToLower())];
        }

        public static string ToMailLink(this string email)
        {
            return string.Format("mail:{0}", email);
        }

        public static string ToPhoneLink(this string phoneNumber)
        {
            return string.Format("tel:{0}", Number().Replace(phoneNumber, ""));
        }
        /// <summary>
        /// Generates a random string
        /// </summary>
        /// <param name="length"></param>
        /// <returns>Random String</returns>
        /// <![CDATA[Taken from http://stackoverflow.com/questions/1344221/how-can-i-generate-random-alphanumeric-strings-in-c]]>
        private static string RandomString(int length)
        {
            const string chars = "abcdefghijklmnopqrstuvxywz0123456789";
            var random = new Random();
            return new string([.. Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)])]);
        }
        #region Regex Properties

        [GeneratedRegex(@"[^a-z0-9\s-]")]
        public static partial Regex NamePatternOne();

        [GeneratedRegex(@"[^a-z0-9]+")]
        public static partial Regex NamePatternThree();

        [GeneratedRegex(@"\s+")]
        public static partial Regex NamePatternTwo();

        [GeneratedRegex(@"[\""].+?[\""]|[^ ]+")]
        public static partial Regex TokenizeRegex();
        [GeneratedRegex(@"_+")]
        public static partial Regex Underscores();
        [GeneratedRegex("<.*?>")]
        private static partial Regex HtmlTags();

        [GeneratedRegex(@"\s")]
        private static partial Regex Hyphens();

        [GeneratedRegex(@"[^a-z0-9\s-]")]
        private static partial Regex InvalidChars();

        [GeneratedRegex(@"[^0-9]")]
        private static partial Regex Number();
        [GeneratedRegex(@"\s+")]
        private static partial Regex SingleSpace();
        #endregion
    }
}
