using Microsoft.EntityFrameworkCore;

namespace UmbracoCms.Plugins.Settings.Models
{
    public class SettingsDbContext : DbContext
    {
        public SettingsDbContext(DbContextOptions<SettingsDbContext> options) : base(options) { }

        public DbSet<SettingGroup> SettingGroups => Set<SettingGroup>();
        public DbSet<SiteSetting> SiteSettings => Set<SiteSetting>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("settings");

            modelBuilder.Entity<SettingGroup>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(256);
                entity.Property(e => e.Alias).IsRequired().HasMaxLength(256);
                entity.Property(e => e.Description).HasMaxLength(1024);
                entity.HasIndex(e => e.Alias).IsUnique();
            });

            modelBuilder.Entity<SiteSetting>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Key).IsRequired().HasMaxLength(512);
                entity.Property(e => e.Value).HasMaxLength(4000);
                entity.Property(e => e.Type).IsRequired().HasMaxLength(50).HasDefaultValue("text");
                entity.Property(e => e.Description).HasMaxLength(1024);
                entity.HasIndex(e => e.Key).IsUnique();
                entity.HasOne(e => e.Group)
                      .WithMany(g => g.Settings)
                      .HasForeignKey(e => e.GroupId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
