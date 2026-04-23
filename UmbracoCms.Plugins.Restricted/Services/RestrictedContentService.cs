using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Services;

namespace UmbracoCms.Plugins.Restricted.Services;

public class RestrictedContentService : IRestrictedContentService
{
    private readonly IPublicAccessService _publicAccessService;
    private readonly IContentService _contentService;
    private readonly ILogger<RestrictedContentService> _logger;

    public RestrictedContentService(
        IPublicAccessService publicAccessService,
        IContentService contentService,
        ILogger<RestrictedContentService> logger)
    {
        _publicAccessService = publicAccessService;
        _contentService = contentService;
        _logger = logger;
    }

    public Task<IEnumerable<int>> GetRestrictedNodesAsync()
    {
        var entries = _publicAccessService.GetAll();
        var nodeIds = entries.Select(e => e.ProtectedNodeId);
        return Task.FromResult(nodeIds);
    }

    public Task RestrictNodeAsync(int nodeId, string loginPageNodeId, string errorPageNodeId, IEnumerable<string> memberGroups)
    {
        var content = _contentService.GetById(nodeId);
        if (content is null)
        {
            _logger.LogWarning("RestrictNodeAsync: Content node {NodeId} not found.", nodeId);
            return Task.CompletedTask;
        }

        // Resolve login and error pages to IContent
        if (!int.TryParse(loginPageNodeId, out var loginId) || !int.TryParse(errorPageNodeId, out var errorId))
        {
            _logger.LogWarning("RestrictNodeAsync: loginPageNodeId or errorPageNodeId is not a valid integer.");
            return Task.CompletedTask;
        }

        var loginContent = _contentService.GetById(loginId);
        var errorContent = _contentService.GetById(errorId);
        if (loginContent is null || errorContent is null)
        {
            _logger.LogWarning("RestrictNodeAsync: Login page {LoginId} or error page {ErrorId} not found.", loginId, errorId);
            return Task.CompletedTask;
        }

        var existing = _publicAccessService.GetEntryForContent(content);
        if (existing is not null)
        {
            // Update existing entry — remove old rules and re-add
            _publicAccessService.Delete(existing);
        }

        var entryId = Guid.NewGuid();
        var rules = memberGroups.Select(g =>
        {
            var rule = new PublicAccessRule(Guid.NewGuid(), entryId)
            {
                RuleType = Constants.Conventions.PublicAccess.MemberRoleRuleType,
                RuleValue = g
            };
            return rule;
        });

        var entry = new PublicAccessEntry(content, loginContent, errorContent, rules);

        _publicAccessService.Save(entry);
        _logger.LogInformation("Node {NodeId} restricted to groups: {Groups}", nodeId, string.Join(", ", memberGroups));
        return Task.CompletedTask;
    }

    public Task UnrestrictNodeAsync(int nodeId)
    {
        var content = _contentService.GetById(nodeId);
        if (content is null)
        {
            _logger.LogWarning("UnrestrictNodeAsync: Content node {NodeId} not found.", nodeId);
            return Task.CompletedTask;
        }

        var existing = _publicAccessService.GetEntryForContent(content);
        if (existing is not null)
        {
            _publicAccessService.Delete(existing);
            _logger.LogInformation("Node {NodeId} unrestricted.", nodeId);
        }
        return Task.CompletedTask;
    }

    public Task<IEnumerable<string>> GetRequiredGroupsAsync(int nodeId)
    {
        var content = _contentService.GetById(nodeId);
        if (content is null)
            return Task.FromResult(Enumerable.Empty<string>());

        var entry = _publicAccessService.GetEntryForContent(content);
        if (entry is null)
            return Task.FromResult(Enumerable.Empty<string>());

        var groups = entry.Rules
            .Where(r => r.RuleType == Constants.Conventions.PublicAccess.MemberRoleRuleType)
            .Select(r => r.RuleValue);

        return Task.FromResult(groups);
    }

    public Task SetRequiredGroupsAsync(int nodeId, string loginPageNodeId, string errorPageNodeId, IEnumerable<string> memberGroups)
    {
        return RestrictNodeAsync(nodeId, loginPageNodeId, errorPageNodeId, memberGroups);
    }
}
