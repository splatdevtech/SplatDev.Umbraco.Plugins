using UmbracoCms.Plugins.ShopCart.Models;

namespace UmbracoCms.Plugins.ShopCart.Services;

public interface IShopCartService
{
    Task<List<CartItem>> GetCart(string sessionId);
    Task AddItem(CartItem item);
    Task UpdateQuantity(int id, int qty);
    Task RemoveItem(int id);
    Task ClearCart(string sessionId);
    Task<decimal> GetTotal(string sessionId);
}
