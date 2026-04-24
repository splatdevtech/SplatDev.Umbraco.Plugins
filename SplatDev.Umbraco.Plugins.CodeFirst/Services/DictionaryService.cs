namespace SplatDev.Umbraco.Plugins.CodeFirst.Services
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    using Microsoft.Extensions.Logging;

    using Umbraco.Cms.Core.Models;
    using Umbraco.Cms.Core.Services;

    [Obsolete("SplatDev.Umbraco.Plugins.CodeFirst is deprecated. Use SplatDev.Umbraco.Plugins.Yaml2Schema instead. This package is maintained for backwards compatibility only.")]
    public class DictionaryService
    {
        private readonly ILocalizationService localizationService;
        private readonly ILogger<DictionaryService> logger;

        // TODO: ILogger should be ILogger<DictionaryService> - inject via DI constructor
        public DictionaryService(ILocalizationService localizationService, ILogger<DictionaryService>? logger = null)
        {
            this.localizationService = localizationService;
            this.logger = logger ?? Microsoft.Extensions.Logging.Abstractions.NullLogger<DictionaryService>.Instance;
        }

        public void CreateDictionaryItem(Type item)
        {
            var keyString = item.GetProperty("Key")!.Value<string>();
            var value = item.GetProperty("Value")!.Value<string>();
            var parentKey = item.GetProperty("ParentKey")?.Value<string>();
            var dicItem = localizationService.GetDictionaryItemByKey(keyString) ?? new DictionaryItem(keyString);

            var lang = localizationService.GetLanguageByIsoCode(item.GetProperty("LanguageCode")!.Value<string>());
            if (lang == null)
            {
                lang = new Language(item.GetProperty("LanguageCode")!.Value<string>());
                localizationService.Save(lang);
            }

            List<IDictionaryTranslation> translations = new List<IDictionaryTranslation>();
            foreach (var translation in item.GetProperty("Translations")!.Value<Dictionary<string, string>>() ?? new Dictionary<string, string>())
            {
                var translationLanguage = localizationService.GetLanguageByIsoCode(translation.Key);
                if (translationLanguage == null)
                {
                    translationLanguage = new Language(translation.Key);
                    localizationService.Save(translationLanguage);
                }
                var trans = new DictionaryTranslation(translationLanguage, translation.Value);
                translations.Add(trans);
            }
            dicItem.Translations = translations;
            if (!string.IsNullOrEmpty(parentKey))
            {
                var parentGuid = localizationService.GetDictionaryItemByKey(parentKey)?.GetUdi().Guid;
                if (parentGuid != null)
                {
                    dicItem.ParentId = parentGuid;
                }
            }
            localizationService.AddOrUpdateDictionaryValue(dicItem, lang, value);
            localizationService.Save(dicItem);
        }

        public void CreateDictionaryItems(List<Type> items)
        {
            foreach (var item in items)
                CreateDictionaryItem(item);
        }

        public void CreateLanguages(IEnumerable<Language> languages, string defaultLang = "en-US")
        {
            var lang = localizationService.GetLanguageByIsoCode(defaultLang);
            try
            {
                if (lang == null)
                {
                    lang = new Language(defaultLang)
                    {
                        IsDefault = true,
                        IsMandatory = true
                    };
                    localizationService.Save(lang);
                }

                foreach (var language in languages)
                {
                    var translationLanguage = localizationService.GetLanguageByIsoCode(language.IsoCode);
                    if (translationLanguage == null)
                    {
                        localizationService.Save(language);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error creating languages");
                throw;
            }
        }

        public bool DictionaryItemExists(Type item)
        {
            var keyString = item.GetProperty("Key")!.Value<string>();
            return localizationService.GetDictionaryItemByKey(keyString) != null;
        }

        public bool DictionaryItemParentExists(string parentName)
        {
            return localizationService.GetDictionaryItemByKey(parentName) != null;
        }
    }
}
