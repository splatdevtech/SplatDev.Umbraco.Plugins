using Microsoft.EntityFrameworkCore;
using UmbracoCms.Plugins.NewsTicker.Models;

namespace UmbracoCms.Plugins.NewsTicker.Services;

public interface INewsTickerService
{
    Task<List<NewsTickerItem>> GetActiveItemsAsync();
    Task<NewsTickerItem> AddItemAsync(NewsTickerItem item);
    Task UpdateItemAsync(NewsTickerItem item);
    Task DeleteItemAsync(int id);
}

public class NewsTickerService : INewsTickerService
{
    private readonly NewsTickerDbContext _db;

    public NewsTickerService(NewsTickerDbContext db)
    {
        _db = db;
    }

    public async Task<List<NewsTickerItem>> GetActiveItemsAsync()
    {
        var now = DateTime.UtcNow;

        return await _db.NewsTickerItems
            .Where(i => i.IsActive
                && (i.StartsAt == null || i.StartsAt <= now)
                && (i.EndsAt == null || i.EndsAt >= now))
            .OrderBy(i => i.SortOrder)
            .ThenBy(i => i.Id)
            .ToListAsync();
    }

    public async Task<NewsTickerItem> AddItemAsync(NewsTickerItem item)
    {
        _db.NewsTickerItems.Add(item);
        await _db.SaveChangesAsync();
        return item;
    }

    public async Task UpdateItemAsync(NewsTickerItem item)
    {
        var existing = await _db.NewsTickerItems.FindAsync(item.Id)
            ?? throw new InvalidOperationException($"NewsTickerItem {item.Id} not found.");

        existing.Text = item.Text;
        existing.Url = item.Url;
        existing.IsActive = item.IsActive;
        existing.SortOrder = item.SortOrder;
        existing.StartsAt = item.StartsAt;
        existing.EndsAt = item.EndsAt;

        await _db.SaveChangesAsync();
    }

    public async Task DeleteItemAsync(int id)
    {
        var item = await _db.NewsTickerItems.FindAsync(id)
            ?? throw new InvalidOperationException($"NewsTickerItem {id} not found.");

        _db.NewsTickerItems.Remove(item);
        await _db.SaveChangesAsync();
    }
}
