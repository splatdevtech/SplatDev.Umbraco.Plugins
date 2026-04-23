using Microsoft.EntityFrameworkCore;

namespace UmbracoCms.Plugins.NewsTicker.Models;

public class NewsTickerDbContext : DbContext
{
    public NewsTickerDbContext(DbContextOptions<NewsTickerDbContext> options)
        : base(options)
    {
    }

    public DbSet<NewsTickerItem> NewsTickerItems => Set<NewsTickerItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<NewsTickerItem>(entity =>
        {
            entity.ToTable("NewsTickerItems");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.Text).IsRequired().HasMaxLength(1000);
            entity.Property(e => e.Url).HasMaxLength(2000);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.SortOrder).HasDefaultValue(0);
        });
    }
}
