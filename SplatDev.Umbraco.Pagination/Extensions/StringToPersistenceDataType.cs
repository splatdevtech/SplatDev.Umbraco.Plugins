// Ignore Spelling: Dev

using Microsoft.AspNetCore.Mvc.Rendering;

using SplatDev.Umbraco.Pagination.Models;

namespace SplatDev.Umbraco.Pagination.Extensions
{
    public static class StringToPersistenceDataType
    {
        public static readonly string[] SEARCH_TYPES = [
            "All",                             //[0]
            "Content",                         //[1]
            "Media",                           //[2]
            ];
        public static SelectList SELECT_LIST => new(SEARCH_TYPES);

        public static PersistenceDataType ToPersistenceDataType(this string searchType)
        {
            return searchType switch
            {
                string when SEARCH_TYPES[0] == searchType => PersistenceDataType.All,
                string when SEARCH_TYPES[1] == searchType => PersistenceDataType.Content,
                string when SEARCH_TYPES[2] == searchType => PersistenceDataType.Media,
                _ => PersistenceDataType.All,
            };
        }

        public static string ToSearchTypeString(this PersistenceDataType dataType)
        {
            return dataType switch
            {
                PersistenceDataType.Content => SEARCH_TYPES[1],
                PersistenceDataType.Media => SEARCH_TYPES[2],
                _ => SEARCH_TYPES[0],
            };
        }

        public static string ToCamelCase(this PersistenceDataType dataType)
        {
            return dataType switch
            {

                PersistenceDataType.Media => ToLowerCamelCasing(PersistenceDataType.Media),
                PersistenceDataType.All => ToLowerCamelCasing(PersistenceDataType.All),
                PersistenceDataType.Content => ToLowerCamelCasing(PersistenceDataType.Content),
                _ => "",
            };
        }

        private static string ToLowerCamelCasing(PersistenceDataType value) => string.Concat(value.ToString()[..1].ToLower(System.Globalization.CultureInfo.CurrentCulture), value.ToString().AsSpan(1));
    }
}
