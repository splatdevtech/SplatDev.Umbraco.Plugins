using Microsoft.EntityFrameworkCore;

namespace UmbracoCms.Plugins.Payments.PagSeguro.Models;

public class PagSeguroDbContext : DbContext
{
    public PagSeguroDbContext(DbContextOptions<PagSeguroDbContext> options)
        : base(options)
    {
    }

    public DbSet<PagSeguroOrder> Orders => Set<PagSeguroOrder>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("pagseguro");
        base.OnModelCreating(modelBuilder);
    }
}
