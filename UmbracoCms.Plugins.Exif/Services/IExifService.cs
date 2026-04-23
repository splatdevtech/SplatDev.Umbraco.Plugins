using UmbracoCms.Plugins.Exif.Models;

namespace UmbracoCms.Plugins.Exif.Services;

public interface IExifService
{
    Task<ExifData?> ReadExifAsync(string filePath);
    Task<ExifData?> ReadExifFromMediaAsync(Guid mediaKey);
}
