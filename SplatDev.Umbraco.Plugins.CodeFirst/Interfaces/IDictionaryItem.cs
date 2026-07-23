using System;
using System.Collections.Generic;

namespace SplatDev.Umbraco.Plugins.CodeFirst.Interfaces
{
    public interface IDictionaryItem
    {
        string ParentKey { get; }
        string Key { get; }
        string Value { get; }
        string LanguageCode { get; }
        Dictionary<string, string> Translations { get; }
    }
}
