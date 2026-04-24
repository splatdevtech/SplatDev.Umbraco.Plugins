using SplatDev.Umbraco.Plugins.LiveVideo.Models;

namespace SplatDev.Umbraco.Plugins.LiveVideo.Services;

public interface ILiveVideoService
{
    LiveVideoEmbedDto GetEmbedUrl(LiveVideoPlatform platform, string channelId);
}
