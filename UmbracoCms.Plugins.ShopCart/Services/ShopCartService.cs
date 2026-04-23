using Microsoft.EntityFrameworkCore;
using UmbracoCms.Plugins.ShopCart.Models;

namespace UmbracoCms.Plugins.ShopCart.Services;

public class ShopCartService : IShopCartService
{
    private readonly ShopCartDbContext _db;

    public ShopCartService(ShopCartDbContext db)
    {
        _db = db;
    }

    public async Task<List<CartItem>> GetCart(string sessionId)
    {
        return await _db.CartItems
            .Where(c => c.SessionId == sessionId)
            .OrderBy(c => c.AddedAt)
            .ToListAsync();
    }

    public async Task AddItem(CartItem item)
    {
        // If the same product already exists in the session cart, increment quantity instead
        var existing = await _db.CartItems
            .FirstOrDefaultAsync(c => c.SessionId == item.SessionId && c.ProductId == item.ProductId);

        if (existing is not null)
        {
            existing.Quantity += item.Quantity;
            _db.CartItems.Update(existing);
        }
        else
        {
            item.AddedAt = DateTime.UtcNow;
            await _db.CartItems.AddAsync(item);
        }

        await _db.SaveChangesAsync();
    }

    public async Task UpdateQuantity(int id, int qty)
    {
        var item = await _db.CartItems.FindAsync(id);
        if (item is null) return;

        if (qty <= 0)
        {
            _db.CartItems.Remove(item);
        }
        else
        {
            item.Quantity = qty;
            _db.CartItems.Update(item);
        }

        await _db.SaveChangesAsync();
    }

    public async Task RemoveItem(int id)
    {
        var item = await _db.CartItems.FindAsync(id);
        if (item is null) return;

        _db.CartItems.Remove(item);
        await _db.SaveChangesAsync();
    }

    public async Task ClearCart(string sessionId)
    {
        var items = await _db.CartItems
            .Where(c => c.SessionId == sessionId)
            .ToListAsync();

        _db.CartItems.RemoveRange(items);
        await _db.SaveChangesAsync();
    }

    public async Task<decimal> GetTotal(string sessionId)
    {
        return await _db.CartItems
            .Where(c => c.SessionId == sessionId)
            .SumAsync(c => c.Price * c.Quantity);
    }
}
