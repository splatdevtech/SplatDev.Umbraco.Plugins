using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Services;

namespace UmbracoCms.Tools.PackageActions;

public abstract class ContentNodeAction : IPackageAction
{
    private readonly IContentService _contentService;
    private readonly IContentTypeService _contentTypeService;

    protected ContentNodeAction(IContentService contentService, IContentTypeService contentTypeService)
    {
        _contentService = contentService;
        _contentTypeService = contentTypeService;
    }

    public abstract string Name { get; }
    protected abstract string NodeName { get; }
    protected abstract string ContentTypeAlias { get; }
    protected virtual int ParentId => -1;

    public virtual Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        var contentType = _contentTypeService.Get(ContentTypeAlias);
        if (contentType is null)
            return Task.CompletedTask;

        var existing = _contentService.GetByLevel(1)
            .FirstOrDefault(c => c.ContentType.Alias == ContentTypeAlias && c.Name == NodeName);

        if (existing is not null)
            return Task.CompletedTask;

        var content = _contentService.Create(NodeName, ParentId, ContentTypeAlias);
        ConfigureContent(content);
        _contentService.SaveAndPublish(content);
        return Task.CompletedTask;
    }

    protected virtual void ConfigureContent(IContent content) { }
}
