using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Services;

#if NET10_0_OR_GREATER
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Models.ContentPublishing;
#endif

namespace SplatDev.Umbraco.Tools.PackageActions;

public abstract class ContentNodeAction : IPackageAction
{
    private readonly IContentService _contentService;
    private readonly IContentTypeService _contentTypeService;
#if NET10_0_OR_GREATER
    private readonly IContentPublishingService _contentPublishingService;

    protected ContentNodeAction(
        IContentService contentService,
        IContentTypeService contentTypeService,
        IContentPublishingService contentPublishingService)
    {
        _contentService = contentService;
        _contentTypeService = contentTypeService;
        _contentPublishingService = contentPublishingService;
    }
#else
    protected ContentNodeAction(IContentService contentService, IContentTypeService contentTypeService)
    {
        _contentService = contentService;
        _contentTypeService = contentTypeService;
    }
#endif

    public abstract string Name { get; }
    protected abstract string NodeName { get; }
    protected abstract string ContentTypeAlias { get; }
    protected virtual int ParentId => -1;

    public virtual async Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        var contentType = _contentTypeService.Get(ContentTypeAlias);
        if (contentType is null)
            return;

        var existing = _contentService.GetByLevel(1)
            .FirstOrDefault(c => c.ContentType.Alias == ContentTypeAlias && c.Name == NodeName);

        if (existing is not null)
            return;

        var content = _contentService.Create(NodeName, ParentId, ContentTypeAlias);
        ConfigureContent(content);
#if NET10_0_OR_GREATER
        _contentService.Save(content);
        var cultures = new[] { new CulturePublishScheduleModel { Culture = null, Schedule = null } };
        await _contentPublishingService.PublishAsync(content.Key, cultures, Constants.Security.SuperUserKey);
#else
        _contentService.SaveAndPublish(content);
        await Task.CompletedTask;
#endif
    }

    protected virtual void ConfigureContent(IContent content) { }
}
