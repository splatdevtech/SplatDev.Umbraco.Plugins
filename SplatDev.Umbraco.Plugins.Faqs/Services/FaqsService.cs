using Microsoft.EntityFrameworkCore;
using SplatDev.Umbraco.Plugins.Faqs.Models;

namespace SplatDev.Umbraco.Plugins.Faqs.Services;

public class FaqsService : IFaqsService
{
    private readonly FaqsDbContext _db;

    public FaqsService(FaqsDbContext db)
    {
        _db = db;
    }

    public async Task<IEnumerable<FaqCategory>> GetCategoriesAsync(bool publishedOnly = true)
    {
        var query = _db.FaqCategories
            .Include(c => c.Items.OrderBy(i => i.SortOrder))
            .OrderBy(c => c.SortOrder)
            .ThenBy(c => c.Name)
            .AsQueryable();

        if (publishedOnly)
            query = query.Select(c => new FaqCategory
            {
                Id = c.Id,
                Name = c.Name,
                Slug = c.Slug,
                SortOrder = c.SortOrder,
                Items = c.Items.Where(i => i.IsPublished).OrderBy(i => i.SortOrder).ToList()
            });

        return await query.ToListAsync();
    }

    public async Task<FaqCategory?> GetCategoryBySlugAsync(string slug, bool publishedOnly = true)
    {
        var category = await _db.FaqCategories
            .Include(c => c.Items.OrderBy(i => i.SortOrder))
            .FirstOrDefaultAsync(c => c.Slug == slug);

        if (category is null) return null;

        if (publishedOnly)
            category.Items = category.Items.Where(i => i.IsPublished).ToList();

        return category;
    }

    public async Task<FaqCategory> CreateCategoryAsync(FaqCategory category)
    {
        _db.FaqCategories.Add(category);
        await _db.SaveChangesAsync();
        return category;
    }

    public async Task<FaqCategory> UpdateCategoryAsync(FaqCategory category)
    {
        _db.FaqCategories.Update(category);
        await _db.SaveChangesAsync();
        return category;
    }

    public async Task DeleteCategoryAsync(int categoryId)
    {
        var category = await _db.FaqCategories.FindAsync(categoryId);
        if (category is not null)
        {
            _db.FaqCategories.Remove(category);
            await _db.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<FaqItem>> GetItemsAsync(int? categoryId = null, bool publishedOnly = true)
    {
        var query = _db.FaqItems
            .Include(i => i.Category)
            .AsQueryable();

        if (categoryId.HasValue)
            query = query.Where(i => i.CategoryId == categoryId.Value);

        if (publishedOnly)
            query = query.Where(i => i.IsPublished);

        return await query
            .OrderBy(i => i.Category.SortOrder)
            .ThenBy(i => i.SortOrder)
            .ToListAsync();
    }

    public async Task<FaqItem?> GetItemByIdAsync(int id)
    {
        return await _db.FaqItems
            .Include(i => i.Category)
            .FirstOrDefaultAsync(i => i.Id == id);
    }

    public async Task<IEnumerable<FaqItem>> SearchAsync(string query, bool publishedOnly = true)
    {
        var normalized = query.Trim().ToLower();

        var items = _db.FaqItems
            .Include(i => i.Category)
            .AsQueryable();

        if (publishedOnly)
            items = items.Where(i => i.IsPublished);

        return await items
            .Where(i =>
                i.Question.ToLower().Contains(normalized) ||
                i.Answer.ToLower().Contains(normalized))
            .OrderBy(i => i.Category.SortOrder)
            .ThenBy(i => i.SortOrder)
            .ToListAsync();
    }

    public async Task<FaqItem> CreateItemAsync(FaqItem item)
    {
        _db.FaqItems.Add(item);
        await _db.SaveChangesAsync();
        return item;
    }

    public async Task<FaqItem> UpdateItemAsync(FaqItem item)
    {
        _db.FaqItems.Update(item);
        await _db.SaveChangesAsync();
        return item;
    }

    public async Task DeleteItemAsync(int id)
    {
        var item = await _db.FaqItems.FindAsync(id);
        if (item is not null)
        {
            _db.FaqItems.Remove(item);
            await _db.SaveChangesAsync();
        }
    }

    public async Task PublishItemAsync(int id, bool publish)
    {
        await _db.FaqItems
            .Where(i => i.Id == id)
            .ExecuteUpdateAsync(s => s.SetProperty(i => i.IsPublished, publish));
    }

    public async Task<int> GetTotalItemCountAsync(bool publishedOnly = true)
    {
        var query = _db.FaqItems.AsQueryable();
        if (publishedOnly)
            query = query.Where(i => i.IsPublished);
        return await query.CountAsync();
    }
}
