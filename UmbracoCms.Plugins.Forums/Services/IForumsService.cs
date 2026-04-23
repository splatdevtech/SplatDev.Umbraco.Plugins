using UmbracoCms.Plugins.Forums.Models;

namespace UmbracoCms.Plugins.Forums.Services;

public interface IForumsService
{
    Task<IEnumerable<ForumCategory>> GetCategoriesAsync();
    Task<ForumCategory?> GetCategoryBySlugAsync(string slug);
    Task<ForumCategory> CreateCategoryAsync(ForumCategory category);
    Task<ForumCategory> UpdateCategoryAsync(ForumCategory category);
    Task DeleteCategoryAsync(int categoryId);

    Task<IEnumerable<ForumThread>> GetThreadsAsync(int categoryId, int page = 1, int pageSize = 20);
    Task<ForumThread?> GetThreadBySlugAsync(string slug);
    Task<ForumThread?> GetThreadByIdAsync(int id);
    Task<ForumThread> CreateThreadAsync(ForumThread thread);
    Task LockThreadAsync(int threadId, bool locked);
    Task PinThreadAsync(int threadId, bool pinned);
    Task DeleteThreadAsync(int threadId);
    Task IncrementThreadViewCountAsync(int threadId);
    Task<int> GetTotalThreadCountAsync(int categoryId);

    Task<IEnumerable<ForumReply>> GetRepliesAsync(int threadId, bool includeDeleted = false);
    Task<ForumReply> AddReplyAsync(ForumReply reply);
    Task ApproveReplyAsync(int replyId);
    Task SoftDeleteReplyAsync(int replyId);
    Task HardDeleteReplyAsync(int replyId);
}
