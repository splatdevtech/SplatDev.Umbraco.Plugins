using System;
using System.Collections.Generic;

namespace UmbracoCms.CodeFirst.Interfaces
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
