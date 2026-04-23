using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Services;

namespace UmbracoCms.Tools.PackageActions;

public abstract class DocumentTypeAction : IPackageAction
{
    private readonly IContentTypeService _contentTypeService;

    protected DocumentTypeAction(IContentTypeService contentTypeService)
    {
        _contentTypeService = contentTypeService;
    }

    public abstract string Name { get; }
    protected abstract string Alias { get; }
    protected abstract string DisplayName { get; }
    protected abstract string Icon { get; }

    public virtual Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        if (_contentTypeService.Get(Alias) is not null)
            return Task.CompletedTask;

        var contentType = new ContentType(null, -1)
        {
            Alias = Alias,
            Name = DisplayName,
            Icon = Icon,
            IsElement = false,
        };

        _contentTypeService.Save(contentType);
        return Task.CompletedTask;
    }
}
