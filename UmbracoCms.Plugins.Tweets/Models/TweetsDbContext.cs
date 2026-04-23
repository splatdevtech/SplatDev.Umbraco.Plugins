using Microsoft.EntityFrameworkCore;

namespace UmbracoCms.Plugins.Tweets.Models;

public class TweetsDbContext : DbContext
{
    public TweetsDbContext(DbContextOptions<TweetsDbContext> options)
        : base(options)
    {
    }

    public DbSet<CachedTweet> CachedTweets => Set<CachedTweet>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<CachedTweet>(entity =>
        {
            entity.ToTable("CachedTweets");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.TweetId).IsRequired().HasMaxLength(50);
            entity.HasIndex(e => e.TweetId).IsUnique();
            entity.Property(e => e.AuthorHandle).IsRequired().HasMaxLength(100);
            entity.Property(e => e.AuthorName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.AuthorAvatarUrl).HasMaxLength(2000);
            entity.Property(e => e.Content).IsRequired().HasMaxLength(4000);
            entity.Property(e => e.TweetUrl).IsRequired().HasMaxLength(2000);
        });
    }
}
