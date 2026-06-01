namespace SplatDev.Umbraco.Plugins.DictionaryManager.Interfaces;

public interface IDictionaryItemImportExport
{
    string ParentKey { get; set; }
    string Key { get; set; }
    string Value { get; set; }
    string LanguageCode { get; set; }
    string LanguageName { get; set; }
    int Id { get; set; }
}
