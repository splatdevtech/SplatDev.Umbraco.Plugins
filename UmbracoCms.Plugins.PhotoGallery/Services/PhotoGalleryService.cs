using Microsoft.EntityFrameworkCore;
using UmbracoCms.Plugins.PhotoGallery.Models;

namespace UmbracoCms.Plugins.PhotoGallery.Services;

public class PhotoGalleryService : IPhotoGalleryService
{
    private readonly PhotoGalleryDbContext _db;

    public PhotoGalleryService(PhotoGalleryDbContext db)
    {
        _db = db;
    }

    public async Task<IEnumerable<Album>> GetAlbumsAsync()
        => await _db.Albums.Include(a => a.Photos).ToListAsync();

    public async Task<Album?> GetAlbumAsync(int id)
        => await _db.Albums.Include(a => a.Photos).FirstOrDefaultAsync(a => a.Id == id);

    public async Task<Album> CreateAlbumAsync(Album album)
    {
        _db.Albums.Add(album);
        await _db.SaveChangesAsync();
        return album;
    }

    public async Task<Album> UpdateAlbumAsync(Album album)
    {
        _db.Albums.Update(album);
        await _db.SaveChangesAsync();
        return album;
    }

    public async Task DeleteAlbumAsync(int id)
    {
        var album = await _db.Albums.FindAsync(id);
        if (album is not null)
        {
            _db.Albums.Remove(album);
            await _db.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<Photo>> GetPhotosAsync(int albumId)
        => await _db.Photos.Where(p => p.AlbumId == albumId).OrderBy(p => p.SortOrder).ToListAsync();

    public async Task<Photo> AddPhotoAsync(Photo photo)
    {
        _db.Photos.Add(photo);
        await _db.SaveChangesAsync();
        return photo;
    }

    public async Task DeletePhotoAsync(int id)
    {
        var photo = await _db.Photos.FindAsync(id);
        if (photo is not null)
        {
            _db.Photos.Remove(photo);
            await _db.SaveChangesAsync();
        }
    }
}
