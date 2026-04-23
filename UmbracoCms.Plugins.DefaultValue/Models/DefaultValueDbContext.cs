using Microsoft.EntityFrameworkCore;

namespace UmbracoCms.Plugins.DefaultValue.Models;

public class DefaultValueDbContext : DbContext
{
    public DefaultValueDbContext(DbContextOptions<DefaultValueDbContext> options) : base(options) { }

    public DbSet<DefaultValueRule> DefaultValueRules => Set<DefaultValueRule>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("defaultvalue");

        modelBuilder.Entity<DefaultValueRule>()
            .Property(r => r.DocumentTypeAlias)
            .HasMaxLength(500)
            .IsRequired();

        modelBuilder.Entity<DefaultValueRule>()
            .Property(r => r.PropertyAlias)
            .HasMaxLength(500)
            .IsRequired();

        modelBuilder.Entity<DefaultValueRule>()
            .HasIndex(r => new { r.DocumentTypeAlias, r.PropertyAlias });

        base.OnModelCreating(modelBuilder);
    }
}
