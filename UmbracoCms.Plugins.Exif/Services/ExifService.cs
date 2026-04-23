using MetadataExtractor;
using MetadataExtractor.Formats.Exif;
using MetadataExtractor.Formats.Jpeg;
using Umbraco.Cms.Core.Services;
using UmbracoCms.Plugins.Exif.Models;

namespace UmbracoCms.Plugins.Exif.Services;

public class ExifService : IExifService
{
    private readonly IMediaService _mediaService;
    private readonly IWebHostEnvironment _env;

    public ExifService(IMediaService mediaService, IWebHostEnvironment env)
    {
        _mediaService = mediaService;
        _env = env;
    }

    public Task<ExifData?> ReadExifAsync(string filePath)
    {
        if (!File.Exists(filePath))
            return Task.FromResult<ExifData?>(null);

        try
        {
            var directories = ImageMetadataReader.ReadMetadata(filePath);
            var data = new ExifData();

            // IFD0 — make/model/dimensions
            var ifd0 = directories.OfType<ExifIfd0Directory>().FirstOrDefault();
            if (ifd0 != null)
            {
                data.Camera = BuildCamera(
                    ifd0.GetDescription(ExifIfd0Directory.TagMake),
                    ifd0.GetDescription(ExifIfd0Directory.TagModel));
                data.Width = ifd0.TryGetInt32(ExifIfd0Directory.TagImageWidth, out var w) ? w : null;
                data.Height = ifd0.TryGetInt32(ExifIfd0Directory.TagImageHeight, out var h) ? h : null;
            }

            // SubIFD — exposure/aperture/iso/lens
            var sub = directories.OfType<ExifSubIfdDirectory>().FirstOrDefault();
            if (sub != null)
            {
                data.ExposureTime = sub.GetDescription(ExifSubIfdDirectory.TagExposureTime);
                data.FNumber = sub.GetDescription(ExifSubIfdDirectory.TagFNumber);
                data.Iso = sub.GetDescription(ExifSubIfdDirectory.TagIsoEquivalent);
                data.Lens = sub.GetDescription(ExifSubIfdDirectory.TagLensModel);
                data.DateTaken = sub.GetDescription(ExifSubIfdDirectory.TagDateTimeOriginal);
            }

            // JPEG dimensions fallback
            if (data.Width == null)
            {
                var jpeg = directories.OfType<JpegDirectory>().FirstOrDefault();
                if (jpeg != null)
                {
                    data.Width = jpeg.TryGetInt32(JpegDirectory.TagImageWidth, out var jw) ? jw : null;
                    data.Height = jpeg.TryGetInt32(JpegDirectory.TagImageHeight, out var jh) ? jh : null;
                }
            }

            // GPS
            var gps = directories.OfType<GpsDirectory>().FirstOrDefault();
            if (gps != null)
            {
                data.GpsLatitude = gps.GetDescription(GpsDirectory.TagLatitude);
                data.GpsLongitude = gps.GetDescription(GpsDirectory.TagLongitude);
            }

            return Task.FromResult<ExifData?>(data);
        }
        catch
        {
            return Task.FromResult<ExifData?>(null);
        }
    }

    public async Task<ExifData?> ReadExifFromMediaAsync(Guid mediaKey)
    {
        var media = _mediaService.GetById(mediaKey);
        if (media == null) return null;

        var umbracoFile = media.GetValue<string>("umbracoFile");
        if (string.IsNullOrEmpty(umbracoFile)) return null;

        // umbracoFile may be a JSON string like {"src":"/media/..."}
        string relativePath = umbracoFile;
        if (umbracoFile.TrimStart().StartsWith("{"))
        {
            using var doc = System.Text.Json.JsonDocument.Parse(umbracoFile);
            if (doc.RootElement.TryGetProperty("src", out var src))
                relativePath = src.GetString() ?? umbracoFile;
        }

        var physicalPath = Path.Combine(_env.WebRootPath, relativePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
        return await ReadExifAsync(physicalPath);
    }

    private static string? BuildCamera(string? make, string? model)
    {
        if (string.IsNullOrEmpty(make) && string.IsNullOrEmpty(model)) return null;
        if (string.IsNullOrEmpty(make)) return model;
        if (string.IsNullOrEmpty(model)) return make;
        return model.StartsWith(make, StringComparison.OrdinalIgnoreCase) ? model : $"{make} {model}";
    }
}
