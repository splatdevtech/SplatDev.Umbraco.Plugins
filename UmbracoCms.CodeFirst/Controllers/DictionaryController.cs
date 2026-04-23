namespace UmbracoCms.CodeFirst.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Microsoft.Extensions.Logging;

    using Umbraco.Cms.Core.Services;

    using UmbracoCms.CodeFirst.Services;

    // NOTE: UmbracoController from Umbraco.Web.Mvc is replaced by UmbracoApiController or
    // inherit from ControllerBase with Umbraco 13+ conventions.
    // TODO: Replace base class with appropriate Umbraco 13+ controller base (e.g., UmbracoApiController).
    public class DictionaryController
    {
        private readonly ILocalizationService _localizationService;
        private readonly UmbracoCms.CodeFirst.Services.DictionaryService _languageDictionaryService;

        public DictionaryController(ILocalizationService localizationService, ILogger<DictionaryController> logger)
        {
            _localizationService = localizationService;
            _languageDictionaryService = new DictionaryService(localizationService);
        }

        /// <summary>
        /// Adds the update dictionaries.
        /// </summary>
        /// <param name="ns">The namespace to get the dictionary items from</param>
        public void AddUpdateDictionaries(string assembly, string ns)
        {
            string nspace = ns;
            var idictionary = typeof(Interfaces.IDictionaryItem);
            var dictionaryTypes = from t in Assembly.Load(assembly).GetTypes()
                                  where t.IsClass && t.Namespace == nspace && idictionary.IsAssignableFrom(t)
                                  select t;

            var updatedDictionaries = new List<string>();
            foreach (var dictionaryType in dictionaryTypes)
            {
                var keyString = dictionaryType.GetProperty("Key")!.Value<string>();
                if (!_localizationService.DictionaryItemExists(keyString))
                {
                    _languageDictionaryService.CreateDictionaryItem(dictionaryType);
                    updatedDictionaries.Add(keyString);
                }
            }
        }
    }
}
