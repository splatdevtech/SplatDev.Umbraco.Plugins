using UmbracoCms.Plugins.VideoPreview.Models;

namespace UmbracoCms.Plugins.VideoPreview.Services;

public interface IVideoPreviewService
{
    Task<VideoInfo?> GetVideoInfoAsync(string url);
}
