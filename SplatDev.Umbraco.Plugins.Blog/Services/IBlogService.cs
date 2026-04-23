using SplatDev.Umbraco.Plugins.Blog.Models;

namespace SplatDev.Umbraco.Plugins.Blog.Services;

public interface IBlogService
{
    Task<IEnumerable<BlogPost>> GetPostsAsync(int page = 1, int pageSize = 10, bool publishedOnly = true);
    Task<BlogPost?> GetPostBySlugAsync(string slug);
    Task<BlogPost?> GetPostByIdAsync(int id);
    Task<IEnumerable<BlogCategory>> GetCategoriesAsync();
    Task<BlogCategory?> GetCategoryBySlugAsync(string slug);
    Task<IEnumerable<BlogTag>> GetTagsAsync();
    Task<IEnumerable<BlogPost>> GetPostsByCategoryAsync(string categorySlug, int page = 1, int pageSize = 10);
    Task<IEnumerable<BlogPost>> GetPostsByTagAsync(string tagSlug, int page = 1, int pageSize = 10);
    Task<IEnumerable<BlogPost>> GetPostsByArchiveAsync(int year, int? month = null, int page = 1, int pageSize = 10);
    Task<BlogPost> CreatePostAsync(BlogPost post);
    Task<BlogPost> UpdatePostAsync(BlogPost post);
    Task DeletePostAsync(int id);
    Task IncrementViewCountAsync(int postId);
    Task<IEnumerable<BlogComment>> GetCommentsAsync(int postId, bool approvedOnly = true);
    Task<BlogComment> AddCommentAsync(BlogComment comment);
    Task ApproveCommentAsync(int commentId);
    Task DeleteCommentAsync(int commentId);
    Task<int> GetTotalPostCountAsync(bool publishedOnly = true);
}
