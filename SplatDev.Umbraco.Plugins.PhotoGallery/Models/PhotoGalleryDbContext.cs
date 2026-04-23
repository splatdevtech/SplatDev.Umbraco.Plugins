using Microsoft.EntityFrameworkCore;

namespace SplatDev.Umbraco.Plugins.PhotoGallery.Models;

public class PhotoGalleryDbContext(DbContextOptions<PhotoGalleryDbContext> options) : DbContext(options)
{
    public DbSet<Album> Albums => Set<Album>();
    public DbSet<Photo> Photos => Set<Photo>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.HasDefaultSchema("photogallery");
        b.Entity<Album>().ToTable("Albums");
        b.Entity<Photo>()
            .ToTable("Photos")
            .HasOne(p => p.Album)
            .WithMany(a => a.Photos)
            .HasForeignKey(p => p.AlbumId);
    }
}
