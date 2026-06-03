using Microsoft.EntityFrameworkCore;
using SplatDev.DigitalBookCurator.Core.Models;

namespace SplatDev.DigitalBookCurator.Core.Context;

public class CuratorDbContext(DbContextOptions<CuratorDbContext> options) : DbContext(options)
{
    public DbSet<Book> Books => Set<Book>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Book>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).HasMaxLength(500);
            entity.Property(e => e.Author).HasMaxLength(300);
            entity.Property(e => e.FileName).HasMaxLength(500);
            entity.Property(e => e.FileType).HasMaxLength(50);
            entity.Property(e => e.Isbn).HasMaxLength(20);
            entity.HasQueryFilter(e => !e.IsDeleted);
        });
    }
}
