using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Services;
using SplatDev.Umbraco.Plugins.Yaml2Schema.Models;

#pragma warning disable CS0618 // ILocalizationService is deprecated in favour of IDictionaryItemService; still functional in Umbraco 17

namespace SplatDev.Umbraco.Plugins.Yaml2Schema.Services
{
    public class DictionaryCreator
    {
        private readonly ILocalizationService _localizationService;
#if NET10_0_OR_GREATER
        private readonly ILanguageService _languageService;
#else
        private readonly ILocalizationService _languageService;
#endif
        private readonly ILogger<DictionaryCreator>? _logger;

        public DictionaryCreator(
            ILocalizationService localizationService,
#if NET10_0_OR_GREATER
            ILanguageService languageService,
#else
            ILocalizationService languageService,
#endif
            ILogger<DictionaryCreator>? logger = null)
        {
            _localizationService = localizationService ?? throw new ArgumentNullException(nameof(localizationService));
            _languageService = languageService ?? throw new ArgumentNullException(nameof(languageService));
            _logger = logger;
        }

        public void CreateDictionaryItems(List<YamlDictionaryItem> items)
        {
            if (items == null) throw new ArgumentNullException(nameof(items));

            var processedKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var yamlItem in items)
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(yamlItem.Key))
                    {
                        _logger?.LogWarning("Dictionary item is missing a key. Skipping.");
                        continue;
                    }

                    if (processedKeys.Contains(yamlItem.Key))
                    {
                        _logger?.LogWarning("Dictionary key '{Key}' is a duplicate and will be skipped.", yamlItem.Key);
                        continue;
                    }

                    // [REMOVE]
                    if (yamlItem.Remove)
                    {
                        var toDelete = _localizationService.GetDictionaryItemByKey(yamlItem.Key);
                        if (toDelete != null)
                        {
                            _localizationService.Delete(toDelete, Constants.Security.SuperUserId);
                            _logger?.LogInformation("Dictionary item '{Key}' removed.", yamlItem.Key);
                        }
                        else
                        {
                            _logger?.LogDebug("Dictionary item '{Key}' not found for removal. Skipping.", yamlItem.Key);
                        }
                        processedKeys.Add(yamlItem.Key);
                        continue;
                    }

                    var existing = _localizationService.GetDictionaryItemByKey(yamlItem.Key);

                    if (existing != null && !yamlItem.Update)
                    {
                        _logger?.LogInformation("Dictionary item '{Key}' already exists. Skipping.", yamlItem.Key);
                        processedKeys.Add(yamlItem.Key);
                        continue;
                    }

                    if (existing == null && yamlItem.Update)
                    {
                        _logger?.LogInformation("Dictionary item '{Key}' not found. Skipping update.", yamlItem.Key);
                        processedKeys.Add(yamlItem.Key);
                        continue;
                    }

                    if (existing == null)
                    {
                        existing = _localizationService.CreateDictionaryItemWithIdentity(yamlItem.Key, null, null);
                    }

                    // Set translation values
                    foreach (var (isoCode, value) in yamlItem.Translations)
                    {
#if NET10_0_OR_GREATER
                        var language = _languageService.GetAsync(isoCode).GetAwaiter().GetResult();
#else
                        var language = _languageService.GetLanguageByIsoCode(isoCode);
#endif
                        if (language == null)
                        {
                            _logger?.LogWarning(
                                "Language '{IsoCode}' not found. Skipping translation for dictionary key '{Key}'.",
                                isoCode, yamlItem.Key);
                            continue;
                        }
                        _localizationService.AddOrUpdateDictionaryValue(existing, language, value);
                    }

                    _localizationService.Save(existing, Constants.Security.SuperUserId);

                    _logger?.LogInformation(
                        yamlItem.Update ? "Dictionary item '{Key}' updated." : "Dictionary item '{Key}' created.",
                        yamlItem.Key);

                    processedKeys.Add(yamlItem.Key);
                }
                catch (Exception ex)
                {
                    _logger?.LogError(ex, "Error processing dictionary item '{Key}'.", yamlItem.Key);
                    throw;
                }
            }
        }
    }
}
