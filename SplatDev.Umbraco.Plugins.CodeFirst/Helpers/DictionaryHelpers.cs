namespace SplatDev.Umbraco.Plugins.CodeFirst.Helpers
{
    using System.Globalization;
    using System.Linq;

    using Umbraco.Cms.Core.Services;

    public static class DictionaryHelpers
    {
        /// <summary>
        /// Gets the Dictionary Value from Key
        /// </summary>
        /// <param name="key">The Dictionary Key</param>
        /// <param name="culture">The Localization Culture</param>
        /// <param name="service">The ILocalizationService</param>
        /// <see cref="https://stackoverflow.com/questions/28811485/umbraco-get-dictionary-item-by-language-how"/>
        /// <returns>The String Value for The Dictionary</returns>
        public static string GetDictionaryValue(string key, CultureInfo culture, ILocalizationService service)
        {
            var dictionaryItem = service.GetDictionaryItemByKey(key);
            if (dictionaryItem != null)
            {
                var translation = dictionaryItem.Translations.SingleOrDefault(x => x.Language.CultureInfo.Equals(culture));
                if (translation != null)
                    return translation.Value;
            }
            return key; // if not found, return key
        }
    }
}
