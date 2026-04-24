using SplatDev.Umbraco.Plugins.PhotoGallery.Models;

namespace SplatDev.Umbraco.Plugins.PhotoGallery.Services;

public interface IPhotoGalleryService
{
    Task<IEnumerable<Album>> GetAlbumsAsync();
    Task<Album?> GetAlbumAsync(int id);
    Task<Album> CreateAlbumAsync(Album album);
    Task<Album> UpdateAlbumAsync(Album album);
    Task DeleteAlbumAsync(int id);
    Task<IEnumerable<Photo>> GetPhotosAsync(int albumId);
    Task<Photo> AddPhotoAsync(Photo photo);
    Task DeletePhotoAsync(int id);
}
