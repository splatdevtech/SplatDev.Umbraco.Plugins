using Microsoft.EntityFrameworkCore;
using SplatDev.Umbraco.Plugins.Forums.Models;

namespace SplatDev.Umbraco.Plugins.Forums.Services;

public class ForumsService : IForumsService
{
    private readonly ForumsDbContext _db;

    public ForumsService(ForumsDbContext db)
    {
        _db = db;
    }

    // --- Categories ---

    public async Task<IEnumerable<ForumCategory>> GetCategoriesAsync()
    {
        return await _db.ForumCategories
            .OrderBy(c => c.SortOrder)
            .ThenBy(c => c.Name)
            .ToListAsync();
    }

    public async Task<ForumCategory?> GetCategoryBySlugAsync(string slug)
    {
        return await _db.ForumCategories
            .Include(c => c.Threads)
            .FirstOrDefaultAsync(c => c.Slug == slug);
    }

    public async Task<ForumCategory> CreateCategoryAsync(ForumCategory category)
    {
        _db.ForumCategories.Add(category);
        await _db.SaveChangesAsync();
        return category;
    }

    public async Task<ForumCategory> UpdateCategoryAsync(ForumCategory category)
    {
        _db.ForumCategories.Update(category);
        await _db.SaveChangesAsync();
        return category;
    }

    public async Task DeleteCategoryAsync(int categoryId)
    {
        var category = await _db.ForumCategories.FindAsync(categoryId);
        if (category is not null)
        {
            _db.ForumCategories.Remove(category);
            await _db.SaveChangesAsync();
        }
    }

    // --- Threads ---

    public async Task<IEnumerable<ForumThread>> GetThreadsAsync(int categoryId, int page = 1, int pageSize = 20)
    {
        return await _db.ForumThreads
            .Include(t => t.Category)
            .Where(t => t.CategoryId == categoryId)
            .OrderByDescending(t => t.IsPinned)
            .ThenByDescending(t => t.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<ForumThread?> GetThreadBySlugAsync(string slug)
    {
        return await _db.ForumThreads
            .Include(t => t.Category)
            .Include(t => t.Replies.Where(r => !r.IsDeleted && r.IsApproved))
            .FirstOrDefaultAsync(t => t.Slug == slug);
    }

    public async Task<ForumThread?> GetThreadByIdAsync(int id)
    {
        return await _db.ForumThreads
            .Include(t => t.Category)
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<ForumThread> CreateThreadAsync(ForumThread thread)
    {
        thread.CreatedAt = DateTime.UtcNow;
        thread.ReplyCount = 0;
        thread.ViewCount = 0;
        _db.ForumThreads.Add(thread);
        await _db.SaveChangesAsync();
        return thread;
    }

    public async Task LockThreadAsync(int threadId, bool locked)
    {
        await _db.ForumThreads
            .Where(t => t.Id == threadId)
            .ExecuteUpdateAsync(s => s.SetProperty(t => t.IsLocked, locked));
    }

    public async Task PinThreadAsync(int threadId, bool pinned)
    {
        await _db.ForumThreads
            .Where(t => t.Id == threadId)
            .ExecuteUpdateAsync(s => s.SetProperty(t => t.IsPinned, pinned));
    }

    public async Task DeleteThreadAsync(int threadId)
    {
        var thread = await _db.ForumThreads.FindAsync(threadId);
        if (thread is not null)
        {
            _db.ForumThreads.Remove(thread);
            await _db.SaveChangesAsync();
        }
    }

    public async Task IncrementThreadViewCountAsync(int threadId)
    {
        await _db.ForumThreads
            .Where(t => t.Id == threadId)
            .ExecuteUpdateAsync(s => s.SetProperty(t => t.ViewCount, t => t.ViewCount + 1));
    }

    public async Task<int> GetTotalThreadCountAsync(int categoryId)
    {
        return await _db.ForumThreads.CountAsync(t => t.CategoryId == categoryId);
    }

    // --- Replies ---

    public async Task<IEnumerable<ForumReply>> GetRepliesAsync(int threadId, bool includeDeleted = false)
    {
        var query = _db.ForumReplies.Where(r => r.ThreadId == threadId);
        if (!includeDeleted)
            query = query.Where(r => !r.IsDeleted);
        return await query.OrderBy(r => r.CreatedAt).ToListAsync();
    }

    public async Task<ForumReply> AddReplyAsync(ForumReply reply)
    {
        reply.CreatedAt = DateTime.UtcNow;
        reply.IsApproved = true;
        reply.IsDeleted = false;
        _db.ForumReplies.Add(reply);

        // Increment reply count on thread
        await _db.ForumThreads
            .Where(t => t.Id == reply.ThreadId)
            .ExecuteUpdateAsync(s => s.SetProperty(t => t.ReplyCount, t => t.ReplyCount + 1));

        await _db.SaveChangesAsync();
        return reply;
    }

    public async Task ApproveReplyAsync(int replyId)
    {
        await _db.ForumReplies
            .Where(r => r.Id == replyId)
            .ExecuteUpdateAsync(s => s.SetProperty(r => r.IsApproved, true));
    }

    public async Task SoftDeleteReplyAsync(int replyId)
    {
        await _db.ForumReplies
            .Where(r => r.Id == replyId)
            .ExecuteUpdateAsync(s => s.SetProperty(r => r.IsDeleted, true));
    }

    public async Task HardDeleteReplyAsync(int replyId)
    {
        var reply = await _db.ForumReplies.FindAsync(replyId);
        if (reply is not null)
        {
            _db.ForumReplies.Remove(reply);
            await _db.SaveChangesAsync();
        }
    }
}
