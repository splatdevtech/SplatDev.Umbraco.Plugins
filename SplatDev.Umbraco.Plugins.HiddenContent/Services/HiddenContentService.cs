using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core.Services;

namespace SplatDev.Umbraco.Plugins.HiddenContent.Services;

public class HiddenContentService : IHiddenContentService
{
    private const string NaviHideAlias = "umbracoNaviHide";

    private readonly IContentService _contentService;
    private readonly ILogger<HiddenContentService> _logger;

    public HiddenContentService(IContentService contentService, ILogger<HiddenContentService> logger)
    {
        _contentService = contentService;
        _logger = logger;
    }

    public Task<IEnumerable<int>> GetHiddenNodesAsync()
    {
        // Walk root content and collect hidden nodes (shallow check on root children)
        var rootNodes = _contentService.GetRootContent();
        var hiddenIds = new List<int>();
        CollectHidden(rootNodes, hiddenIds);
        return Task.FromResult<IEnumerable<int>>(hiddenIds);
    }

    private void CollectHidden(IEnumerable<global::Umbraco.Cms.Core.Models.IContent> nodes, List<int> hiddenIds)
    {
        foreach (var node in nodes)
        {
            var val = node.GetValue<string>(NaviHideAlias);
            if (val == "1")
                hiddenIds.Add(node.Id);

            var children = _contentService.GetPagedChildren(node.Id, 0, int.MaxValue, out _);
            CollectHidden(children, hiddenIds);
        }
    }

    public async Task HideNodeAsync(int nodeId)
    {
        var content = _contentService.GetById(nodeId);
        if (content is null)
        {
            _logger.LogWarning("HideNodeAsync: Content node {NodeId} not found.", nodeId);
            return;
        }

        content.SetValue(NaviHideAlias, "1");
#if NET10_0_OR_GREATER
        _contentService.Save(content);
#else
        _contentService.SaveAndPublish(content);
#endif
        _logger.LogInformation("Node {NodeId} hidden from navigation.", nodeId);
        await Task.CompletedTask;
    }

    public async Task ShowNodeAsync(int nodeId)
    {
        var content = _contentService.GetById(nodeId);
        if (content is null)
        {
            _logger.LogWarning("ShowNodeAsync: Content node {NodeId} not found.", nodeId);
            return;
        }

        content.SetValue(NaviHideAlias, "0");
#if NET10_0_OR_GREATER
        _contentService.Save(content);
#else
        _contentService.SaveAndPublish(content);
#endif
        _logger.LogInformation("Node {NodeId} shown in navigation.", nodeId);
        await Task.CompletedTask;
    }

    public Task<bool> IsHiddenAsync(int nodeId)
    {
        var content = _contentService.GetById(nodeId);
        if (content is null)
            return Task.FromResult(false);

        var val = content.GetValue<string>(NaviHideAlias);
        return Task.FromResult(val == "1");
    }

    public async Task BulkHideAsync(IEnumerable<int> nodeIds)
    {
        foreach (var nodeId in nodeIds)
            await HideNodeAsync(nodeId);
    }

    public async Task BulkShowAsync(IEnumerable<int> nodeIds)
    {
        foreach (var nodeId in nodeIds)
            await ShowNodeAsync(nodeId);
    }
}
