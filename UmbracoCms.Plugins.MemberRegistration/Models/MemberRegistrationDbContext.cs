using Microsoft.EntityFrameworkCore;

namespace UmbracoCms.Plugins.MemberRegistration.Models;

public class MemberRegistrationDbContext : DbContext
{
    public MemberRegistrationDbContext(DbContextOptions<MemberRegistrationDbContext> options) : base(options) { }

    public DbSet<RegistrationToken> RegistrationTokens => Set<RegistrationToken>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("memberreg");

        modelBuilder.Entity<RegistrationToken>(entity =>
        {
            entity.HasKey(t => t.Id);
            entity.Property(t => t.Token).HasMaxLength(256).IsRequired();
            entity.Property(t => t.MemberId).IsRequired();
            entity.Property(t => t.CreatedAt).IsRequired();
            entity.Property(t => t.ExpiresAt).IsRequired();
            entity.HasIndex(t => t.Token).IsUnique();
            entity.HasIndex(t => t.MemberId);
        });

        base.OnModelCreating(modelBuilder);
    }
}
