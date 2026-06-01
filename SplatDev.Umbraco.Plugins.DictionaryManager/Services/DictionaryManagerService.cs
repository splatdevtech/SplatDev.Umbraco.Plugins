using System.Text.Json;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Services;
using SplatDev.Umbraco.Plugins.DictionaryManager.Models;

namespace SplatDev.Umbraco.Plugins.DictionaryManager.Services;

public class DictionaryManagerService : IDictionaryManagerService
{
    private readonly ILocalizationService _localizationService;

    public DictionaryManagerService(ILocalizationService localizationService)
    {
        _localizationService = localizationService;
    }

    // ── Read ─────────────────────────────────────────────────────────────────

    public IEnumerable<DictionaryItemDto> GetAll()
    {
        var result = new List<DictionaryItemDto>();
        var languages = _localizationService.GetAllLanguages().ToList();

        foreach (var root in _localizationService.GetRootDictionaryItems())
        {
            result.Add(MapItem(root, null, languages));
            CollectChildren(root, result, languages);
        }

        return result;
    }

    private void CollectChildren(IDictionaryItem parent, List<DictionaryItemDto> result, List<ILanguage> languages)
    {
        foreach (var child in _localizationService.GetDictionaryItemChildren(parent.Key))
        {
            result.Add(MapItem(child, parent.ItemKey, languages));
            CollectChildren(child, result, languages);
        }
    }

    private static DictionaryItemDto MapItem(IDictionaryItem item, string? parentKey, List<ILanguage> languages)
    {
        var translations = new Dictionary<string, string>();
        foreach (var lang in languages)
        {
#if NET10_0_OR_GREATER
            var t = item.Translations.FirstOrDefault(x => x.LanguageIsoCode == lang.IsoCode);
#else
            var t = item.Translations.FirstOrDefault(x => x.Language.IsoCode == lang.IsoCode);
#endif
            translations[lang.IsoCode] = t?.Value ?? string.Empty;
        }

        var defaultLang = languages.FirstOrDefault(l => l.IsDefault)?.IsoCode
                          ?? languages.FirstOrDefault()?.IsoCode
                          ?? string.Empty;

        return new DictionaryItemDto
        {
            Key = item.ItemKey,
            ParentKey = parentKey,
            LanguageCode = defaultLang,
            Value = translations.TryGetValue(defaultLang, out var v) ? v : string.Empty,
            Translations = translations
        };
    }

    // ── Create ────────────────────────────────────────────────────────────────

    public DictionaryItemDto? Create(DictionaryItemDto dto)
    {
        try
        {
            Guid? parentKey = null;
            if (!string.IsNullOrWhiteSpace(dto.ParentKey))
                parentKey = _localizationService.GetDictionaryItemByKey(dto.ParentKey)?.Key;

            var item = new DictionaryItem(dto.Key) { ParentId = parentKey };
            ApplyTranslations(item, dto);
            _localizationService.Save(item);

            var languages = _localizationService.GetAllLanguages().ToList();
            return MapItem(item, dto.ParentKey, languages);
        }
        catch
        {
            return null;
        }
    }

    // ── Update ────────────────────────────────────────────────────────────────

    public DictionaryItemDto? Update(DictionaryItemDto dto)
    {
        var item = _localizationService.GetDictionaryItemByKey(dto.Key);
        if (item is null)
            return null;

        try
        {
            ApplyTranslations(item, dto);
            _localizationService.Save(item);

            var languages = _localizationService.GetAllLanguages().ToList();
            var parentKey = item.ParentId.HasValue
                ? _localizationService.GetDictionaryItemById(item.ParentId.Value)?.ItemKey
                : null;
            return MapItem(item, parentKey, languages);
        }
        catch
        {
            return null;
        }
    }

    // ── Delete ────────────────────────────────────────────────────────────────

    public bool Delete(string key)
    {
        var item = _localizationService.GetDictionaryItemByKey(key);
        if (item is null)
            return false;

        _localizationService.Delete(item);
        return true;
    }

    // ── Import ────────────────────────────────────────────────────────────────

    public Task<IEnumerable<ImportResult>> ImportAsync(List<DictionaryItemDto> items, bool overrideExisting)
    {
        var results = new List<ImportResult>();

        // Process parents before children by ordering items without parentKey first.
        var ordered = items
            .OrderBy(i => string.IsNullOrWhiteSpace(i.ParentKey) ? 0 : 1)
            .ToList();

        foreach (var dto in ordered)
        {
            try
            {
                var existing = _localizationService.GetDictionaryItemByKey(dto.Key);

                if (existing is not null && !overrideExisting)
                {
                    results.Add(new ImportResult { Key = dto.Key, Success = false });
                    continue;
                }

                if (existing is not null)
                {
                    ApplyTranslations(existing, dto);
                    _localizationService.Save(existing);
                }
                else
                {
                    Guid? parentKey = null;
                    if (!string.IsNullOrWhiteSpace(dto.ParentKey))
                        parentKey = _localizationService.GetDictionaryItemByKey(dto.ParentKey)?.Key;

                    var newItem = new DictionaryItem(dto.Key) { ParentId = parentKey };
                    ApplyTranslations(newItem, dto);
                    _localizationService.Save(newItem);
                }

                results.Add(new ImportResult { Key = dto.Key, Success = true });
            }
            catch
            {
                results.Add(new ImportResult { Key = dto.Key, Success = false });
            }
        }

        return Task.FromResult<IEnumerable<ImportResult>>(results);
    }

    // ── Export ────────────────────────────────────────────────────────────────

    public Task<byte[]> ExportAsync()
    {
        var all = GetAll();
        var json = JsonSerializer.Serialize(all, new JsonSerializerOptions { WriteIndented = true });
        var bytes = System.Text.Encoding.UTF8.GetBytes(json);
        return Task.FromResult(bytes);
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private void ApplyTranslations(IDictionaryItem item, DictionaryItemDto dto)
    {
        var existingTranslations = item.Translations.ToList();

        // Apply named translations
        foreach (var (isoCode, value) in dto.Translations)
        {
            var lang = _localizationService.GetLanguageByIsoCode(isoCode);
            if (lang is null)
                continue;

#if NET10_0_OR_GREATER
            var existing = existingTranslations.FirstOrDefault(t => t.LanguageIsoCode == isoCode);
#else
            var existing = existingTranslations.FirstOrDefault(t => t.Language.IsoCode == isoCode);
#endif
            if (existing is not null)
                existing.Value = value;
            else
                existingTranslations.Add(new DictionaryTranslation(lang, value));
        }

        // Also apply the primary Value to the LanguageCode field if populated
        if (!string.IsNullOrWhiteSpace(dto.LanguageCode) && !dto.Translations.ContainsKey(dto.LanguageCode))
        {
            var lang = _localizationService.GetLanguageByIsoCode(dto.LanguageCode);
            if (lang is not null)
            {
#if NET10_0_OR_GREATER
                var existing = existingTranslations.FirstOrDefault(t => t.LanguageIsoCode == dto.LanguageCode);
#else
                var existing = existingTranslations.FirstOrDefault(t => t.Language.IsoCode == dto.LanguageCode);
#endif
                if (existing is not null)
                    existing.Value = dto.Value;
                else
                    existingTranslations.Add(new DictionaryTranslation(lang, dto.Value));
            }
        }

        item.Translations = existingTranslations;
    }
}
