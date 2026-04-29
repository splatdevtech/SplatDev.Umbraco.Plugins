using Microsoft.EntityFrameworkCore;

namespace SplatDev.Umbraco.Plugins.Payments.BancoInter.Models;

public class BancoInterDbContext(DbContextOptions<BancoInterDbContext> options) : DbContext(options)
{
    public DbSet<BancoInterTransaction> Transactions => Set<BancoInterTransaction>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("bancointer");
        base.OnModelCreating(modelBuilder);
    }
}
