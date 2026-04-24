using SplatDev.Umbraco.Plugins.SvgViewer.Models;

namespace SplatDev.Umbraco.Plugins.SvgViewer.Services;

public interface ISvgViewerService
{
    Task<SvgInfo?> GetSvgAsync(Guid mediaKey);
    Task<IEnumerable<SvgInfo>> GetAllSvgMediaAsync();
}
