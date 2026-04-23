namespace SplatDev.Content.Attributes
{
    using System;
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false)]
    internal class CountryInfoAttribute : Attribute
    {
        public string LanguageIsoCode;
        public string Language;
    }
}
