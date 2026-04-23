using Microsoft.EntityFrameworkCore;

namespace SplatDev.Umbraco.Plugins.Newsletters.Models;

public class NewslettersDbContext : DbContext
{
    public NewslettersDbContext(DbContextOptions<NewslettersDbContext> options)
        : base(options)
    {
    }

    public DbSet<NewsletterSubscriber> Subscribers => Set<NewsletterSubscriber>();
    public DbSet<NewsletterCampaign> Campaigns => Set<NewsletterCampaign>();
    public DbSet<NewsletterSend> Sends => Set<NewsletterSend>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<NewsletterSubscriber>(entity =>
        {
            entity.ToTable("NewsletterSubscribers");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.Email).IsRequired().HasMaxLength(320);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.FirstName).HasMaxLength(100);
            entity.Property(e => e.LastName).HasMaxLength(100);
        });

        modelBuilder.Entity<NewsletterCampaign>(entity =>
        {
            entity.ToTable("NewsletterCampaigns");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.Subject).IsRequired().HasMaxLength(500);
            entity.Property(e => e.HtmlContent).IsRequired();
            entity.Property(e => e.PlainTextContent).IsRequired();
            entity.Property(e => e.Status).HasConversion<int>();
        });

        modelBuilder.Entity<NewsletterSend>(entity =>
        {
            entity.ToTable("NewsletterSends");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();

            entity.HasOne(e => e.Campaign)
                .WithMany()
                .HasForeignKey(e => e.CampaignId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Subscriber)
                .WithMany()
                .HasForeignKey(e => e.SubscriberId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
