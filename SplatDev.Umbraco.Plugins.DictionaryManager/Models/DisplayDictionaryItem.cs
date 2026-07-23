namespace SplatDev.Umbraco.Plugins.DictionaryManager.Models
{
    using System.ComponentModel.DataAnnotations;

    using SplatDev.Umbraco.Plugins.DictionaryManager.Interfaces;

    public class DisplayDictionaryItem : IDictionaryItemImportExport
    {
        [Display(Name = "Parent")]
        public string ParentKey { get; set; }

        [Display(Name = "Key")]
        public string Key { get; set; }

        [Display(Name = "Translation")]
        public string Value { get; set; }

        [Display(Name = "Language Code")]
        public string LanguageCode { get; set; }

        [Display(Name = "Language Name")]
        public string LanguageName { get; set; }

        public int Id { get; set; }
    }
}
