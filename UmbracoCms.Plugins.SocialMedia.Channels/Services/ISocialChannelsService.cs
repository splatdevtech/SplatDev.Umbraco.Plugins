using UmbracoCms.Plugins.SocialMedia.Channels.Models;

namespace UmbracoCms.Plugins.SocialMedia.Channels.Services
{
    public interface ISocialChannelsService
    {
        Task<IEnumerable<SocialChannel>> GetChannelsAsync();
        Task<SocialChannel> AddChannelAsync(SocialChannel channel);
        Task RemoveChannelAsync(int id);
        Task<IEnumerable<ScheduledPost>> GetScheduledPostsAsync();
        Task<ScheduledPost> SchedulePostAsync(ScheduledPost post);
        Task DeletePostAsync(int id);
    }
}
