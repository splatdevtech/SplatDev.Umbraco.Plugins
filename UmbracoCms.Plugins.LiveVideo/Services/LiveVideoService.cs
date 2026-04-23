using UmbracoCms.Plugins.LiveVideo.Models;

namespace UmbracoCms.Plugins.LiveVideo.Services;

public class LiveVideoService : ILiveVideoService
{
    public LiveVideoEmbedDto GetEmbedUrl(LiveVideoPlatform platform, string channelId)
    {
        var embedUrl = platform switch
        {
            LiveVideoPlatform.YouTube => $"https://www.youtube.com/embed/live_stream?channel={channelId}",
            LiveVideoPlatform.Twitch => $"https://player.twitch.tv/?channel={channelId}&parent=localhost",
            LiveVideoPlatform.Vimeo => $"https://vimeo.com/event/{channelId}/embed",
            _ => throw new ArgumentOutOfRangeException(nameof(platform), platform, "Unsupported platform.")
        };

        return new LiveVideoEmbedDto
        {
            EmbedUrl = embedUrl,
            PlatformName = platform.ToString(),
            IsLive = true
        };
    }
}
