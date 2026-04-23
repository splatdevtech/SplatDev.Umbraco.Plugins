using UmbracoCms.Plugins.Faqs.Models;

namespace UmbracoCms.Plugins.Faqs.Services;

public interface IFaqsService
{
    Task<IEnumerable<FaqCategory>> GetCategoriesAsync(bool publishedOnly = true);
    Task<FaqCategory?> GetCategoryBySlugAsync(string slug, bool publishedOnly = true);
    Task<FaqCategory> CreateCategoryAsync(FaqCategory category);
    Task<FaqCategory> UpdateCategoryAsync(FaqCategory category);
    Task DeleteCategoryAsync(int categoryId);

    Task<IEnumerable<FaqItem>> GetItemsAsync(int? categoryId = null, bool publishedOnly = true);
    Task<FaqItem?> GetItemByIdAsync(int id);
    Task<IEnumerable<FaqItem>> SearchAsync(string query, bool publishedOnly = true);
    Task<FaqItem> CreateItemAsync(FaqItem item);
    Task<FaqItem> UpdateItemAsync(FaqItem item);
    Task DeleteItemAsync(int id);
    Task PublishItemAsync(int id, bool publish);
    Task<int> GetTotalItemCountAsync(bool publishedOnly = true);
}
