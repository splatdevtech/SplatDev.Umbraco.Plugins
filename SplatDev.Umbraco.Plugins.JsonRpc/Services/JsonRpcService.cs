using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;

#if NET10_0_OR_GREATER
using Umbraco.Cms.Core.Routing;
using Umbraco.Cms.Core.PublishedCache;
#else
using Umbraco.Extensions;
#endif

namespace SplatDev.Umbraco.Plugins.JsonRpc.Services;

public class JsonRpcService : IJsonRpcService
{
    private readonly IContentService _contentService;
    private readonly IUmbracoContextAccessor _contextAccessor;
#if NET10_0_OR_GREATER
    private readonly IPublishedUrlProvider _publishedUrlProvider;

    public JsonRpcService(
        IContentService contentService,
        IUmbracoContextAccessor contextAccessor,
        IPublishedUrlProvider publishedUrlProvider)
    {
        _contentService = contentService;
        _contextAccessor = contextAccessor;
        _publishedUrlProvider = publishedUrlProvider;
    }
#else
    public JsonRpcService(IContentService contentService, IUmbracoContextAccessor contextAccessor)
    {
        _contentService = contentService;
        _contextAccessor = contextAccessor;
    }
#endif

#if NET10_0_OR_GREATER
    public async Task<object?> GetContentByAlias(string alias)
    {
        if (!_contextAccessor.TryGetUmbracoContext(out var ctx) || ctx?.Content is null)
            return null;

        // In Umbraco 17 GetByXPath was removed; walk via IContentService by name
        var rootItems = _contentService.GetRootContent();
        global::Umbraco.Cms.Core.Models.IContent? found = null;
        foreach (var root in rootItems)
        {
            found = FindByName(root, alias);
            if (found is not null) break;
        }

        if (found is null)
            return null;

        var published = await ctx.Content.GetByIdAsync(found.Id);
        if (published is null)
            return null;

        return new
        {
            published.Id,
            published.Name,
            Alias = published.ContentType.Alias,
            Url = _publishedUrlProvider.GetUrl(published),
            published.CreateDate,
            published.UpdateDate
        };
    }

    private global::Umbraco.Cms.Core.Models.IContent? FindByName(global::Umbraco.Cms.Core.Models.IContent node, string alias)
    {
        // Match by name (case-insensitive) or by URL-friendly slug comparison
        var nodeName = node.Name?.Replace(" ", "-").ToLowerInvariant();
        if (string.Equals(nodeName, alias, StringComparison.OrdinalIgnoreCase)
            || string.Equals(node.Name, alias, StringComparison.OrdinalIgnoreCase))
            return node;

        var children = _contentService.GetPagedChildren(node.Id, 0, int.MaxValue, out _);
        foreach (var child in children)
        {
            var result = FindByName(child, alias);
            if (result is not null) return result;
        }
        return null;
    }
#else
    public Task<object?> GetContentByAlias(string alias)
    {
        if (!_contextAccessor.TryGetUmbracoContext(out var ctx) || ctx?.Content is null)
            return Task.FromResult<object?>(null);

        var content = ctx.Content.GetByXPath($"//*[@isDoc][@urlName='{alias}']")
            .FirstOrDefault();

        if (content is null)
            return Task.FromResult<object?>(null);

        return Task.FromResult<object?>(new
        {
            content.Id,
            content.Name,
            Alias  = content.ContentType.Alias,
            Url = content.Url(),
            content.CreateDate,
            content.UpdateDate
        });
    }
#endif

#if NET10_0_OR_GREATER
    public async Task<object?> GetContentById(int id)
    {
        if (!_contextAccessor.TryGetUmbracoContext(out var ctx) || ctx?.Content is null)
            return null;

        var content = await ctx.Content.GetByIdAsync(id);

        if (content is null)
            return null;

        return new
        {
            content.Id,
            content.Name,
            Alias  = content.ContentType.Alias,
            Url = _publishedUrlProvider.GetUrl(content),
            content.CreateDate,
            content.UpdateDate
        };
    }
#else
    public Task<object?> GetContentById(int id)
    {
        if (!_contextAccessor.TryGetUmbracoContext(out var ctx) || ctx?.Content is null)
            return Task.FromResult<object?>(null);

        var content = ctx.Content.GetById(id);

        if (content is null)
            return Task.FromResult<object?>(null);

        return Task.FromResult<object?>(new
        {
            content.Id,
            content.Name,
            Alias  = content.ContentType.Alias,
            Url = content.Url(),
            content.CreateDate,
            content.UpdateDate
        });
    }
#endif

#if NET10_0_OR_GREATER
    public async Task<object?> SearchContent(string term)
    {
        if (!_contextAccessor.TryGetUmbracoContext(out var ctx) || ctx?.Content is null)
            return null;

        // Use IContentService to search by name, then resolve published versions
        var rootItems = _contentService.GetRootContent();
        var matches = new List<object>();

        foreach (var root in rootItems)
        {
            await CollectMatchesAsync(ctx.Content, root, term, matches);
            if (matches.Count >= 20) break;
        }

        return matches.Take(20);
    }

    private async Task CollectMatchesAsync(IPublishedContentCache cache, global::Umbraco.Cms.Core.Models.IContent node, string term, List<object> matches)
    {
        if (matches.Count >= 20) return;

        if (node.Name?.Contains(term, StringComparison.OrdinalIgnoreCase) == true)
        {
            var published = await cache.GetByIdAsync(node.Id);
            if (published is not null)
            {
                matches.Add(new
                {
                    published.Id,
                    published.Name,
                    Alias = published.ContentType.Alias,
                    Url = _publishedUrlProvider.GetUrl(published)
                });
            }
        }

        var children = _contentService.GetPagedChildren(node.Id, 0, int.MaxValue, out _);
        foreach (var child in children)
        {
            await CollectMatchesAsync(cache, child, term, matches);
            if (matches.Count >= 20) return;
        }
    }
#else
    public Task<object?> SearchContent(string term)
    {
        if (!_contextAccessor.TryGetUmbracoContext(out var ctx) || ctx?.Content is null)
            return Task.FromResult<object?>(null);

        var results = ctx.Content.GetByXPath($"//*[@isDoc][contains(translate(@nodeName,'ABCDEFGHIJKLMNOPQRSTUVWXYZ','abcdefghijklmnopqrstuvwxyz'),'{term.ToLower()}')]")
            .Take(20)
            .Select(c => new
            {
                c.Id,
                c.Name,
                Alias = c.ContentType.Alias,
                Url = c.Url()
            });

        return Task.FromResult<object?>(results);
    }
#endif
}
