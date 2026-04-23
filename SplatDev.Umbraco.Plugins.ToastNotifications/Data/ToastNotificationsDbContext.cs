using Microsoft.EntityFrameworkCore;
using SplatDev.Umbraco.Plugins.ToastNotifications.Models;

namespace SplatDev.Umbraco.Plugins.ToastNotifications.Data;

public class ToastNotificationsDbContext : DbContext
{
    public ToastNotificationsDbContext(DbContextOptions<ToastNotificationsDbContext> options)
        : base(options)
    {
    }

    public DbSet<ToastMessage> ToastMessages => Set<ToastMessage>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("toastnotifications");

        modelBuilder.Entity<ToastMessage>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(256);
            entity.Property(e => e.Body).IsRequired().HasMaxLength(2000);
            entity.Property(e => e.Type).IsRequired().HasMaxLength(32);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
        });
    }
}
