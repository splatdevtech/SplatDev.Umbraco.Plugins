using UmbracoCms.Plugins.LiveVideo.Models;

namespace UmbracoCms.Plugins.LiveVideo.Services;

public interface ILiveVideoService
{
    LiveVideoEmbedDto GetEmbedUrl(LiveVideoPlatform platform, string channelId);
}
