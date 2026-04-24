using Microsoft.EntityFrameworkCore;

namespace SplatDev.Umbraco.Plugins.Payments.MercadoPago.Models;

public class MercadoPagoDbContext : DbContext
{
    public MercadoPagoDbContext(DbContextOptions<MercadoPagoDbContext> options)
        : base(options)
    {
    }

    public DbSet<MercadoPagoOrder> Orders => Set<MercadoPagoOrder>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("mpago");
        base.OnModelCreating(modelBuilder);
    }
}
