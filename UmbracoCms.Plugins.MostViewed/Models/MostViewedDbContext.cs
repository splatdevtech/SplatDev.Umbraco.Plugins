using Microsoft.EntityFrameworkCore;

namespace UmbracoCms.Plugins.MostViewed.Models;

public class MostViewedDbContext : DbContext
{
    public MostViewedDbContext(DbContextOptions<MostViewedDbContext> options)
        : base(options) { }

    public DbSet<PageView> PageViews => Set<PageView>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<PageView>(entity =>
        {
            entity.HasIndex(e => e.ContentKey)
                  .HasDatabaseName("IX_MostViewed_ContentKey");

            entity.HasIndex(e => e.ViewedAt)
                  .HasDatabaseName("IX_MostViewed_ViewedAt");
        });
    }
}
