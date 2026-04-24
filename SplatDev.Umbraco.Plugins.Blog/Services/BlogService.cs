using Microsoft.EntityFrameworkCore;
using SplatDev.Umbraco.Plugins.Blog.Models;

namespace SplatDev.Umbraco.Plugins.Blog.Services;

public class BlogService : IBlogService
{
    private readonly BlogDbContext _db;

    public BlogService(BlogDbContext db)
    {
        _db = db;
    }

    public async Task<IEnumerable<BlogPost>> GetPostsAsync(int page = 1, int pageSize = 10, bool publishedOnly = true)
    {
        var query = _db.BlogPosts
            .Include(p => p.Category)
            .Include(p => p.BlogPostTags).ThenInclude(bpt => bpt.BlogTag)
            .AsQueryable();

        if (publishedOnly)
            query = query.Where(p => p.IsPublished);

        return await query
            .OrderByDescending(p => p.PublishedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<BlogPost?> GetPostBySlugAsync(string slug)
    {
        return await _db.BlogPosts
            .Include(p => p.Category)
            .Include(p => p.BlogPostTags).ThenInclude(bpt => bpt.BlogTag)
            .Include(p => p.Comments.Where(c => c.IsApproved))
            .FirstOrDefaultAsync(p => p.Slug == slug);
    }

    public async Task<BlogPost?> GetPostByIdAsync(int id)
    {
        return await _db.BlogPosts
            .Include(p => p.Category)
            .Include(p => p.BlogPostTags).ThenInclude(bpt => bpt.BlogTag)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<IEnumerable<BlogCategory>> GetCategoriesAsync()
    {
        return await _db.BlogCategories
            .OrderBy(c => c.Name)
            .ToListAsync();
    }

    public async Task<BlogCategory?> GetCategoryBySlugAsync(string slug)
    {
        return await _db.BlogCategories.FirstOrDefaultAsync(c => c.Slug == slug);
    }

    public async Task<IEnumerable<BlogTag>> GetTagsAsync()
    {
        return await _db.BlogTags
            .OrderBy(t => t.Name)
            .ToListAsync();
    }

    public async Task<IEnumerable<BlogPost>> GetPostsByCategoryAsync(string categorySlug, int page = 1, int pageSize = 10)
    {
        return await _db.BlogPosts
            .Include(p => p.Category)
            .Include(p => p.BlogPostTags).ThenInclude(bpt => bpt.BlogTag)
            .Where(p => p.IsPublished && p.Category != null && p.Category.Slug == categorySlug)
            .OrderByDescending(p => p.PublishedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<IEnumerable<BlogPost>> GetPostsByTagAsync(string tagSlug, int page = 1, int pageSize = 10)
    {
        return await _db.BlogPosts
            .Include(p => p.Category)
            .Include(p => p.BlogPostTags).ThenInclude(bpt => bpt.BlogTag)
            .Where(p => p.IsPublished && p.BlogPostTags.Any(bpt => bpt.BlogTag.Slug == tagSlug))
            .OrderByDescending(p => p.PublishedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<IEnumerable<BlogPost>> GetPostsByArchiveAsync(int year, int? month = null, int page = 1, int pageSize = 10)
    {
        var query = _db.BlogPosts
            .Include(p => p.Category)
            .Where(p => p.IsPublished && p.PublishedAt.Year == year);

        if (month.HasValue)
            query = query.Where(p => p.PublishedAt.Month == month.Value);

        return await query
            .OrderByDescending(p => p.PublishedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<BlogPost> CreatePostAsync(BlogPost post)
    {
        post.PublishedAt = DateTime.UtcNow;
        _db.BlogPosts.Add(post);
        await _db.SaveChangesAsync();
        return post;
    }

    public async Task<BlogPost> UpdatePostAsync(BlogPost post)
    {
        _db.BlogPosts.Update(post);
        await _db.SaveChangesAsync();
        return post;
    }

    public async Task DeletePostAsync(int id)
    {
        var post = await _db.BlogPosts.FindAsync(id);
        if (post is not null)
        {
            _db.BlogPosts.Remove(post);
            await _db.SaveChangesAsync();
        }
    }

    public async Task IncrementViewCountAsync(int postId)
    {
        await _db.BlogPosts
            .Where(p => p.Id == postId)
            .ExecuteUpdateAsync(s => s.SetProperty(p => p.ViewCount, p => p.ViewCount + 1));
    }

    public async Task<IEnumerable<BlogComment>> GetCommentsAsync(int postId, bool approvedOnly = true)
    {
        var query = _db.BlogComments.Where(c => c.BlogPostId == postId);
        if (approvedOnly)
            query = query.Where(c => c.IsApproved);
        return await query.OrderBy(c => c.CreatedAt).ToListAsync();
    }

    public async Task<BlogComment> AddCommentAsync(BlogComment comment)
    {
        comment.CreatedAt = DateTime.UtcNow;
        comment.IsApproved = false;
        _db.BlogComments.Add(comment);
        await _db.SaveChangesAsync();
        return comment;
    }

    public async Task ApproveCommentAsync(int commentId)
    {
        await _db.BlogComments
            .Where(c => c.Id == commentId)
            .ExecuteUpdateAsync(s => s.SetProperty(c => c.IsApproved, true));
    }

    public async Task DeleteCommentAsync(int commentId)
    {
        var comment = await _db.BlogComments.FindAsync(commentId);
        if (comment is not null)
        {
            _db.BlogComments.Remove(comment);
            await _db.SaveChangesAsync();
        }
    }

    public async Task<int> GetTotalPostCountAsync(bool publishedOnly = true)
    {
        var query = _db.BlogPosts.AsQueryable();
        if (publishedOnly)
            query = query.Where(p => p.IsPublished);
        return await query.CountAsync();
    }
}
