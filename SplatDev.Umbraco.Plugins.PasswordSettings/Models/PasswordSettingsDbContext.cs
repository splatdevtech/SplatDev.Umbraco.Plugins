using Microsoft.EntityFrameworkCore;

namespace SplatDev.Umbraco.Plugins.PasswordSettings.Models;

public class PasswordSettingsDbContext : DbContext
{
    public PasswordSettingsDbContext(DbContextOptions<PasswordSettingsDbContext> options) : base(options) { }

    public DbSet<PasswordHistory> PasswordHistories => Set<PasswordHistory>();
    public DbSet<PasswordPolicy> PasswordPolicies => Set<PasswordPolicy>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("passwordsettings");

        modelBuilder.Entity<PasswordHistory>()
            .HasIndex(h => h.MemberId);

        modelBuilder.Entity<PasswordHistory>()
            .HasIndex(h => h.ChangedAt);

        modelBuilder.Entity<PasswordPolicy>()
            .Property(p => p.MinLength)
            .HasDefaultValue(8);

        base.OnModelCreating(modelBuilder);
    }
}
