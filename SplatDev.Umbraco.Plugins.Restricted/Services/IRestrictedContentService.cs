namespace SplatDev.Umbraco.Plugins.Restricted.Services;

public interface IRestrictedContentService
{
    Task<IEnumerable<int>> GetRestrictedNodesAsync();
    Task RestrictNodeAsync(int nodeId, string loginPageNodeId, string errorPageNodeId, IEnumerable<string> memberGroups);
    Task UnrestrictNodeAsync(int nodeId);
    Task<IEnumerable<string>> GetRequiredGroupsAsync(int nodeId);
    Task SetRequiredGroupsAsync(int nodeId, string loginPageNodeId, string errorPageNodeId, IEnumerable<string> memberGroups);
}
