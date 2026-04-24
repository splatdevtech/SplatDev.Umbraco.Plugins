using Microsoft.EntityFrameworkCore;

namespace SplatDev.Umbraco.Plugins.Slider.Models;

public class SliderDbContext(DbContextOptions<SliderDbContext> options) : DbContext(options)
{
    public DbSet<SliderConfig> Sliders => Set<SliderConfig>();
    public DbSet<Slide> Slides => Set<Slide>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.HasDefaultSchema("slider");
        b.Entity<SliderConfig>().ToTable("Sliders");
        b.Entity<Slide>()
            .ToTable("Slides")
            .HasOne(s => s.Slider)
            .WithMany(c => c.Slides)
            .HasForeignKey(s => s.SliderId);
    }
}
