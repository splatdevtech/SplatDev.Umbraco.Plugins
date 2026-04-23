using Microsoft.EntityFrameworkCore;

namespace UmbracoCms.Plugins.StarRatings.Models;

public class StarRatingsDbContext : DbContext
{
    public StarRatingsDbContext(DbContextOptions<StarRatingsDbContext> options)
        : base(options) { }

    public DbSet<ContentRating> ContentRatings => Set<ContentRating>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ContentRating>(entity =>
        {
            entity.HasIndex(e => new { e.ContentKey, e.VoterIp })
                  .HasDatabaseName("IX_StarRatings_ContentKey_VoterIp");

            entity.HasIndex(e => e.ContentKey)
                  .HasDatabaseName("IX_StarRatings_ContentKey");
        });
    }
}
