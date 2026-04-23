using UmbracoCms.Plugins.SvgViewer.Models;

namespace UmbracoCms.Plugins.SvgViewer.Services;

public interface ISvgViewerService
{
    Task<SvgInfo?> GetSvgAsync(Guid mediaKey);
    Task<IEnumerable<SvgInfo>> GetAllSvgMediaAsync();
}
