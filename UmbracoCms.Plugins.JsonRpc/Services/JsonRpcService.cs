using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;

namespace UmbracoCms.Plugins.JsonRpc.Services;

public class JsonRpcService : IJsonRpcService
{
    private readonly IContentService _contentService;
    private readonly IUmbracoContextAccessor _contextAccessor;

    public JsonRpcService(IContentService contentService, IUmbracoContextAccessor contextAccessor)
    {
        _contentService = contentService;
        _contextAccessor = contextAccessor;
    }

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
            content.Url,
            content.CreateDate,
            content.UpdateDate
        });
    }

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
            content.Url,
            content.CreateDate,
            content.UpdateDate
        });
    }

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
                c.Url
            });

        return Task.FromResult<object?>(results);
    }
}
