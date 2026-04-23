using Microsoft.EntityFrameworkCore;

namespace UmbracoCms.Plugins.RdpManager.Models
{
    public class RdpManagerDbContext : DbContext
    {
        public RdpManagerDbContext(DbContextOptions<RdpManagerDbContext> options) : base(options) { }

        public DbSet<RdpConnection> RdpConnections => Set<RdpConnection>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("rdpmanager");

            modelBuilder.Entity<RdpConnection>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(256);
                entity.Property(e => e.Host).IsRequired().HasMaxLength(512);
                entity.Property(e => e.Username).HasMaxLength(256);
                entity.Property(e => e.Domain).HasMaxLength(256);
                entity.Property(e => e.Notes).HasMaxLength(4000);
                entity.Property(e => e.Port).HasDefaultValue(3389);
                entity.Property(e => e.ColorDepth).HasDefaultValue(32);
                entity.Property(e => e.Width).HasDefaultValue(1920);
                entity.Property(e => e.Height).HasDefaultValue(1080);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            });
        }
    }
}
