using Microsoft.EntityFrameworkCore;

using UmbracoCms.Plugins.SocialMedia.Channels.Models;

namespace UmbracoCms.Plugins.SocialMedia.Channels.Services
{
    public class SocialChannelsService(SocialChannelsDbContext dbContext) : ISocialChannelsService
    {
        private readonly SocialChannelsDbContext _dbContext = dbContext;

        public async Task<IEnumerable<SocialChannel>> GetChannelsAsync()
        {
            return await _dbContext.SocialChannels
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        public async Task<SocialChannel> AddChannelAsync(SocialChannel channel)
        {
            channel.ConnectedAt = DateTime.UtcNow;
            _dbContext.SocialChannels.Add(channel);
            await _dbContext.SaveChangesAsync();
            return channel;
        }

        public async Task RemoveChannelAsync(int id)
        {
            var channel = await _dbContext.SocialChannels.FindAsync(id);
            if (channel is not null)
            {
                _dbContext.SocialChannels.Remove(channel);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<ScheduledPost>> GetScheduledPostsAsync()
        {
            return await _dbContext.ScheduledPosts
                .Include(p => p.Channel)
                .OrderBy(p => p.ScheduledAt)
                .ToListAsync();
        }

        public async Task<ScheduledPost> SchedulePostAsync(ScheduledPost post)
        {
            post.Status = "pending";
            post.PublishedAt = null;
            _dbContext.ScheduledPosts.Add(post);
            await _dbContext.SaveChangesAsync();
            return post;
        }

        public async Task DeletePostAsync(int id)
        {
            var post = await _dbContext.ScheduledPosts.FindAsync(id);
            if (post is not null)
            {
                _dbContext.ScheduledPosts.Remove(post);
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
