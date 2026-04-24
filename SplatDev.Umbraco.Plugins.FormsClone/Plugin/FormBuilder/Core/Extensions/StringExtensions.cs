using FormBuilder.Core.Models;
using FormBuilder.Core.Persistence.Fields;
using FormBuilder.Core.Services.Interfaces;

using Microsoft.AspNetCore.DataProtection;

using System.Collections;

namespace FormBuilder.Core.Extensions
{
    public static class StringExtensions
    {
        public static string? ParsePlaceHolders(
          this string text,
          IPlaceholderParsingService? placeholderParsingService,
          bool htmlEncodeReplacedValues,
          Record? record = null,
          Form? form = null,
          Hashtable? pageElements = null,
          IDictionary<string, string?>? additionalData = null)
        {
            return placeholderParsingService?.ParsePlaceHolders(text, htmlEncodeReplacedValues, record, form, pageElements, additionalData);
        }

        public static string? ApplyPrevalueCaptions(
          this string value,
          Guid fieldId,
          Dictionary<Guid, Dictionary<string, string?>?>? prevalueMaps)
        {
            if (prevalueMaps is null || prevalueMaps.Count == 0) return null;
            return !(!string.IsNullOrWhiteSpace(value) && prevalueMaps.ContainsKey(fieldId)) ? value : ReplaceValueWithPrevalueCaptions(value, prevalueMaps[fieldId]!);
        }

        private static string ReplaceValueWithPrevalueCaptions(
          string value,
          Dictionary<string, string?> prevalues)
        {
            foreach (KeyValuePair<string, string?> prevalue in prevalues)
            {
                if (prevalue.Key == value)
                    return string.IsNullOrWhiteSpace(prevalue.Value) ? prevalue.Key : prevalue.Value;
            }
            if (!value.Contains(", "))
                return value;
            string[] strArray = value.Split([", "], StringSplitOptions.RemoveEmptyEntries);
            List<string> values = [];
            foreach (string str in strArray)
            {
                bool flag = false;
                foreach (KeyValuePair<string, string?> prevalue in prevalues)
                {
                    if (prevalue.Key == str)
                    {
                        values.Add(string.IsNullOrWhiteSpace(prevalue.Value) ? prevalue.Key : prevalue.Value);
                        flag = true;
                        break;
                    }
                }
                if (!flag)
                    values.Add(str);
            }
            return string.Join(", ", values);
        }

        public static string EncryptFilePath(
          this string path,
          IDataProtector dataProtector,
          string filePathAndFileNameSeparator)
        {
            string str = path.Split('/').Last();
            return dataProtector.Protect(path) + filePathAndFileNameSeparator + str;
        }

        public static string DecryptFilePath(
          this string path,
          IDataProtector dataProtector,
          string filePathAndFileNameSeparator)
        {
            string protectedData = path.Split(
            [
        filePathAndFileNameSeparator
            ], StringSplitOptions.None).First();
            return dataProtector.Unprotect(protectedData);
        }

        internal static string ReplaceNewlines(this string input, string replacement) => input.Replace("\r\n", replacement).Replace("\n", replacement).Replace("\r", replacement);
    }
}