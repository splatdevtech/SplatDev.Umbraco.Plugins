using Microsoft.EntityFrameworkCore;

namespace UmbracoCms.Plugins.Gdrp.Models;

public class GdrpDbContext : DbContext
{
    public GdrpDbContext(DbContextOptions<GdrpDbContext> options)
        : base(options)
    {
    }

    public DbSet<ConsentRecord> ConsentRecords => Set<ConsentRecord>();
    public DbSet<DataRequest> DataRequests => Set<DataRequest>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("gdrp");
        base.OnModelCreating(modelBuilder);
    }
}
