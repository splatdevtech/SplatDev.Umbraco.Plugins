using Microsoft.EntityFrameworkCore;

namespace SplatDev.Umbraco.Plugins.ShopCart.Models;

public class ShopCartDbContext : DbContext
{
    public ShopCartDbContext(DbContextOptions<ShopCartDbContext> options)
        : base(options)
    {
    }

    public DbSet<CartItem> CartItems => Set<CartItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("shopcart");
        base.OnModelCreating(modelBuilder);
    }
}
