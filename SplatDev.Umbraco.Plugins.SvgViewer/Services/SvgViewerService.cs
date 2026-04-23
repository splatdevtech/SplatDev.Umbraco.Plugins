using System.Text.RegularExpressions;
using System.Xml;
using Umbraco.Cms.Core.Services;
using SplatDev.Umbraco.Plugins.SvgViewer.Models;

namespace SplatDev.Umbraco.Plugins.SvgViewer.Services;

public partial class SvgViewerService : ISvgViewerService
{
    private readonly IMediaService _mediaService;
    private readonly IWebHostEnvironment _env;

    public SvgViewerService(IMediaService mediaService, IWebHostEnvironment env)
    {
        _mediaService = mediaService;
        _env = env;
    }

    public async Task<SvgInfo?> GetSvgAsync(Guid mediaKey)
    {
        var media = _mediaService.GetById(mediaKey);
        if (media == null) return null;

        var filePath = ResolvePhysicalPath(media);
        if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath)) return null;

        var raw = await File.ReadAllTextAsync(filePath);
        var sanitized = SanitizeSvg(raw);
        var (width, height) = ParseDimensions(sanitized);

        return new SvgInfo
        {
            MediaKey = media.Key,
            FileName = Path.GetFileName(filePath),
            SanitizedContent = sanitized,
            Width = width,
            Height = height
        };
    }

    public async Task<IEnumerable<SvgInfo>> GetAllSvgMediaAsync()
    {
        var results = new List<SvgInfo>();
        var page = 0;
        const int pageSize = 100;

        while (true)
        {
            var items = _mediaService.GetPagedRootMedia(page, pageSize, out var total).ToList();

            foreach (var media in items)
            {
                var path = ResolvePhysicalPath(media);
                if (string.IsNullOrEmpty(path)) continue;
                var ext = Path.GetExtension(path);
                if (!ext.Equals(".svg", StringComparison.OrdinalIgnoreCase)) continue;
                if (!File.Exists(path)) continue;

                var raw = await File.ReadAllTextAsync(path);
                var sanitized = SanitizeSvg(raw);
                var (w, h) = ParseDimensions(sanitized);
                results.Add(new SvgInfo
                {
                    MediaKey = media.Key,
                    FileName = Path.GetFileName(path),
                    SanitizedContent = sanitized,
                    Width = w,
                    Height = h
                });
            }

            if ((page + 1) * pageSize >= total) break;
            page++;
        }

        return results;
    }

    private string? ResolvePhysicalPath(Umbraco.Cms.Core.Models.IMedia media)
    {
        var umbracoFile = media.GetValue<string>("umbracoFile");
        if (string.IsNullOrEmpty(umbracoFile)) return null;

        string relativePath = umbracoFile;
        if (umbracoFile.TrimStart().StartsWith("{"))
        {
            try
            {
                using var doc = System.Text.Json.JsonDocument.Parse(umbracoFile);
                if (doc.RootElement.TryGetProperty("src", out var src))
                    relativePath = src.GetString() ?? umbracoFile;
            }
            catch { /* leave as-is */ }
        }

        return Path.Combine(_env.WebRootPath, relativePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
    }

    internal static string SanitizeSvg(string svgContent)
    {
        // Remove script tags
        svgContent = ScriptTagRegex().Replace(svgContent, string.Empty);
        // Remove event handler attributes
        svgContent = OnEventRegex().Replace(svgContent, string.Empty);
        // Remove javascript: hrefs
        svgContent = JavascriptHrefRegex().Replace(svgContent, "href=\"\"");
        // Remove javascript: xlink:hrefs
        svgContent = JavascriptXlinkRegex().Replace(svgContent, "xlink:href=\"\"");
        return svgContent;
    }

    private static (int Width, int Height) ParseDimensions(string svgContent)
    {
        try
        {
            var doc = new XmlDocument();
            doc.LoadXml(svgContent);
            var root = doc.DocumentElement;
            if (root == null) return (0, 0);

            var wAttr = root.GetAttribute("width");
            var hAttr = root.GetAttribute("height");

            if (int.TryParse(wAttr?.TrimEnd('p', 'x', ' '), out var w) &&
                int.TryParse(hAttr?.TrimEnd('p', 'x', ' '), out var h))
                return (w, h);

            var vb = root.GetAttribute("viewBox");
            if (!string.IsNullOrEmpty(vb))
            {
                var parts = vb.Split(new[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length >= 4 &&
                    int.TryParse(parts[2], out var vbw) &&
                    int.TryParse(parts[3], out var vbh))
                    return (vbw, vbh);
            }
        }
        catch { /* ignore parse errors */ }
        return (0, 0);
    }

    [GeneratedRegex(@"<script[\s\S]*?</script>", RegexOptions.IgnoreCase)]
    private static partial Regex ScriptTagRegex();

    [GeneratedRegex(@"\s+on\w+\s*=\s*(""[^""]*""|'[^']*')", RegexOptions.IgnoreCase)]
    private static partial Regex OnEventRegex();

    [GeneratedRegex(@"href\s*=\s*""javascript:[^""]*""", RegexOptions.IgnoreCase)]
    private static partial Regex JavascriptHrefRegex();

    [GeneratedRegex(@"xlink:href\s*=\s*""javascript:[^""]*""", RegexOptions.IgnoreCase)]
    private static partial Regex JavascriptXlinkRegex();
}
