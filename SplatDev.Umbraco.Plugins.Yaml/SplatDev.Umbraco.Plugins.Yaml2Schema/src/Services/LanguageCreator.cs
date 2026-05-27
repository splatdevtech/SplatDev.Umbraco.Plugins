using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Services;
using SplatDev.Umbraco.Plugins.Yaml2Schema.Models;

namespace SplatDev.Umbraco.Plugins.Yaml2Schema.Services
{
    public class LanguageCreator
    {
#if NET10_0_OR_GREATER
        private readonly ILanguageService _languageService;
#else
        private readonly ILocalizationService _languageService;
#endif
        private readonly ILogger<LanguageCreator>? _logger;

#if NET10_0_OR_GREATER
        public LanguageCreator(ILanguageService languageService, ILogger<LanguageCreator>? logger = null)
#else
        public LanguageCreator(ILocalizationService languageService, ILogger<LanguageCreator>? logger = null)
#endif
        {
            _languageService = languageService ?? throw new ArgumentNullException(nameof(languageService));
            _logger = logger;
        }

        public void CreateLanguages(List<YamlLanguage> languages)
        {
            if (languages == null) throw new ArgumentNullException(nameof(languages));

            var processedCodes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var yamlLang in languages)
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(yamlLang.IsoCode))
                    {
                        _logger?.LogWarning("Language entry is missing isoCode. Skipping.");
                        continue;
                    }

                    if (processedCodes.Contains(yamlLang.IsoCode))
                    {
                        _logger?.LogWarning("Language '{IsoCode}' is a duplicate and will be skipped.", yamlLang.IsoCode);
                        continue;
                    }

                    // [REMOVE]
                    if (yamlLang.Remove)
                    {
#if NET10_0_OR_GREATER
                        var toDelete = _languageService.GetAsync(yamlLang.IsoCode).GetAwaiter().GetResult();
#else
                        var toDelete = _languageService.GetLanguageByIsoCode(yamlLang.IsoCode);
#endif
                        if (toDelete != null)
                        {
#if NET10_0_OR_GREATER
                            _languageService.DeleteAsync(yamlLang.IsoCode, Constants.Security.SuperUserKey)
                                .GetAwaiter().GetResult();
#else
                            _languageService.Delete(toDelete, Constants.Security.SuperUserId);
#endif
                            _logger?.LogInformation("Language '{IsoCode}' removed.", yamlLang.IsoCode);
                        }
                        else
                        {
                            _logger?.LogDebug("Language '{IsoCode}' not found for removal. Skipping.", yamlLang.IsoCode);
                        }
                        processedCodes.Add(yamlLang.IsoCode);
                        continue;
                    }

#if NET10_0_OR_GREATER
                    var existing = _languageService.GetAsync(yamlLang.IsoCode).GetAwaiter().GetResult();
#else
                    var existing = _languageService.GetLanguageByIsoCode(yamlLang.IsoCode);
#endif

                    // [UPDATE]
                    if (yamlLang.Update && existing != null)
                    {
                        existing.IsDefault = yamlLang.IsDefault;
                        existing.IsMandatory = yamlLang.IsMandatory;
#if NET10_0_OR_GREATER
                        _languageService.UpdateAsync(existing, Constants.Security.SuperUserKey).GetAwaiter().GetResult();
#else
                        _languageService.Save(existing, Constants.Security.SuperUserId);
#endif
                        _logger?.LogInformation("Language '{IsoCode}' updated.", yamlLang.IsoCode);
                        processedCodes.Add(yamlLang.IsoCode);
                        continue;
                    }

                    if (existing != null)
                    {
                        _logger?.LogInformation("Language '{IsoCode}' already exists. Skipping.", yamlLang.IsoCode);
                        processedCodes.Add(yamlLang.IsoCode);
                        continue;
                    }

                    // Create
                    var cultureName = yamlLang.CultureName
                        ?? CultureInfo.GetCultureInfo(yamlLang.IsoCode).DisplayName;

                    var language = new Language(yamlLang.IsoCode, cultureName)
                    {
                        IsDefault = yamlLang.IsDefault,
                        IsMandatory = yamlLang.IsMandatory
                    };

#if NET10_0_OR_GREATER
                    _languageService.CreateAsync(language, Constants.Security.SuperUserKey).GetAwaiter().GetResult();
#else
                    _languageService.Save(language, Constants.Security.SuperUserId);
#endif
                    _logger?.LogInformation("Language '{IsoCode}' created.", yamlLang.IsoCode);
                    processedCodes.Add(yamlLang.IsoCode);
                }
                catch (Exception ex)
                {
                    _logger?.LogError(ex, "Error processing language '{IsoCode}'.", yamlLang.IsoCode);
                    throw;
                }
            }
        }
    }
}
