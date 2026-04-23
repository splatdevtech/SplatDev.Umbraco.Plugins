using Microsoft.EntityFrameworkCore;

namespace UmbracoCms.Plugins.OnOff.Models;

public class OnOffDbContext : DbContext
{
    public OnOffDbContext(DbContextOptions<OnOffDbContext> options) : base(options) { }

    public DbSet<FeatureToggle> FeatureToggles => Set<FeatureToggle>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("onoff");

        modelBuilder.Entity<FeatureToggle>()
            .HasIndex(f => f.Alias)
            .IsUnique();

        modelBuilder.Entity<FeatureToggle>()
            .Property(f => f.Name)
            .HasMaxLength(200)
            .IsRequired();

        modelBuilder.Entity<FeatureToggle>()
            .Property(f => f.Alias)
            .HasMaxLength(200)
            .IsRequired();

        base.OnModelCreating(modelBuilder);
    }
}
