using SplatDev.Umbraco.Plugins.DictionaryManager.Models;

namespace SplatDev.Umbraco.Plugins.DictionaryManager.Services;

public interface IDictionaryManagerService
{
    IEnumerable<DictionaryItemDto> GetAll();
    DictionaryItemDto? Create(DictionaryItemDto item);
    DictionaryItemDto? Update(DictionaryItemDto item);
    bool Delete(string key);
    Task<IEnumerable<ImportResult>> ImportAsync(List<DictionaryItemDto> items, bool overrideExisting);
    Task<byte[]> ExportAsync();
}
