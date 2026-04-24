using SplatDev.Umbraco.Plugins.Exif.Models;

namespace SplatDev.Umbraco.Plugins.Exif.Services;

public interface IExifService
{
    Task<ExifData?> ReadExifAsync(string filePath);
    Task<ExifData?> ReadExifFromMediaAsync(Guid mediaKey);
}
