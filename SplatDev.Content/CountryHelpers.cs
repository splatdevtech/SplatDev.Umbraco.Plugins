namespace SplatDev.Content
{
    using SplatDev.Content.Attributes;
    using SplatDev.Content.Data;
    using SplatDev.Content.Enums;
    using SplatDev.Content.Models;

    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    /// <summary>
    /// Country Codes Helper, modified from ContryInfo - Enum
    /// </summary>
    /// <see cref="https://www.codeproject.com/Reference/987722/CountryInfo-Enum"/>
    public static class CountryHelpers
    {
        /// <summary>
        /// Froms the alpha2.
        /// </summary>
        /// <param name="alpha2">The alpha2.</param>
        /// <returns></returns>
        public static ISO3166Country FromAlpha2(string alpha2)
        {
            return Iso3166Countries.LIST.FirstOrDefault(p => p.Alpha2 == alpha2);
        }

        /// <summary>
        /// Obtain ISO3166-1 Country based on its alpha3 code.
        /// </summary>
        /// <param name="alpha3"></param>
        /// <returns></returns>
        public static ISO3166Country FromAlpha3(string alpha3)
        {
            return Iso3166Countries.LIST.FirstOrDefault(p => p.Alpha3 == alpha3);
        }

        /// <summary>
        /// Gets the list of countries by selected country codes.
        /// </summary>
        /// <param name="code">List of culture codes.</param>
        /// <returns>Returns the list of countries by selected country codes.</returns>
        public static List<RegionInfo> GetCountriesByCode(List<string> codes)
        {
            List<RegionInfo> countries = new List<RegionInfo>();
            if (codes != null && codes.Count > 0)
            {
                foreach (string code in codes)
                {
                    try
                    {
                        countries.Add(new RegionInfo(code));
                    }
                    catch
                    {
                        //  Ignores the invalid culture code.
                    }
                }
            }
            return countries.OrderBy(p => p.EnglishName).ToList();
        }

        /// <summary>
        /// Gets the list of countries based on ISO 3166-1
        /// </summary>
        /// <returns>Returns the list of countries based on ISO 3166-1</returns>
        public static List<RegionInfo> GetCountriesByIso3166()
        {
            List<RegionInfo> countries = new List<RegionInfo>();
            foreach (CultureInfo culture in CultureInfo.GetCultures(CultureTypes.SpecificCultures))
            {
                RegionInfo country = new RegionInfo(culture.LCID);
                if (countries.Where(p => p.Name == country.Name).Count() == 0)
                    countries.Add(country);
            }
            return countries.OrderBy(p => p.EnglishName).ToList();
        }

        /// <summary>
        /// Gets the country by code.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <returns></returns>
        public static RegionInfo GetCountryByCode(string code)
        {
            return new RegionInfo(code);
        }

        /// <summary>
        /// Gets the country code.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static string GetCountryCode(this Enum value)
        {
            CountryInfoAttribute[] name = value.GetType().GetField(value.ToString()).GetCustomAttributes(typeof(CountryInfoAttribute), false) as CountryInfoAttribute[];
            return name[0].LanguageIsoCode;
        }

        /// <summary>
        /// Gets the country language iso code.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static string GetCountryLanguageIsoCode(this Enum value)
        {
            CountryInfoAttribute[] name = value.GetType().GetField(value.ToString()).GetCustomAttributes(typeof(CountryInfoAttribute), false) as CountryInfoAttribute[];
            return name[0].LanguageIsoCode;
        }

        /// <summary>
        /// Gets the name of the country language.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static string GetCountryLanguageName(this Enum value)
        {
            CountryInfoAttribute[] name = value.GetType().GetField(value.ToString()).GetCustomAttributes(typeof(CountryInfoAttribute), false) as CountryInfoAttribute[];
            return name[0].Language;
        }

        /// <summary>
        /// Gets the name of the country language.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static string GetCountryLanguageName(string value)
        {
            return new CultureInfo(value).DisplayName;
        }

        /// <summary>
        /// Gets the name of the country.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static string GetCountryName(this Enum value)
        {
            CountryInfoAttribute[] name = value.GetType().GetField(value.ToString()).GetCustomAttributes(typeof(CountryInfoAttribute), false) as CountryInfoAttribute[];
            return name[0].Language;
        }
        /// <summary>
        /// Gets the name of the country.
        /// </summary>
        /// <param name="countryCode">The country code.</param>
        /// <returns></returns>
        public static string GetCountryName(string countryCode)
        {
            var alpha2 = Iso3166Countries.LIST.SingleOrDefault(x => x.Alpha3 == countryCode)?.Alpha2;
            var region = new RegionInfo(alpha2);
            return region.NativeName;
        }

        /// <summary>
        /// Gets the enum.
        /// </summary>
        /// <param name="searchField">The search field.</param>
        /// <returns></returns>
        public static Countries GetEnum(string searchField)
        {
            return Countries.GetValues(typeof(Countries)).Cast<Countries>().FirstOrDefault(v => string.Equals(v.GetCountryCode(), searchField, StringComparison.OrdinalIgnoreCase) || string.Equals(v.GetCountryName(), searchField, StringComparison.OrdinalIgnoreCase));
        }
    }
}
