using Umbraco.Cms.Core.Services;

#if NET10_0_OR_GREATER
using Umbraco.Cms.Core.Models;
#endif

namespace UmbracoCms.Tools.PackageActions;

public abstract class TemplateAction : IPackageAction
{
#if NET10_0_OR_GREATER
    private readonly ITemplateService _templateService;

    protected TemplateAction(ITemplateService templateService)
    {
        _templateService = templateService;
    }
#else
    private readonly IFileService _fileService;

    protected TemplateAction(IFileService fileService)
    {
        _fileService = fileService;
    }
#endif

    public abstract string Name { get; }
    protected abstract string TemplateAlias { get; }
    protected abstract string TemplateName { get; }
    protected virtual string Content => $"@inherits Umbraco.Cms.Web.Common.Views.UmbracoViewPage\n@{{{Environment.NewLine}    Layout = null;{Environment.NewLine}}}";

    public virtual async Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
#if NET10_0_OR_GREATER
        var existing = await _templateService.GetAsync(TemplateAlias);
        if (existing is not null)
            return;

        var template = new Template(null, TemplateName, TemplateAlias)
        {
            Content = Content,
        };
        await _templateService.CreateAsync(template, userId: -1, cancellationToken);
#else
        var existing = _fileService.GetTemplate(TemplateAlias);
        if (existing is not null)
            return;

        var template = new Umbraco.Cms.Core.Models.Template(null, TemplateName, TemplateAlias)
        {
            Content = Content,
        };
        _fileService.SaveTemplate(template);
        await Task.CompletedTask;
#endif
    }
}
