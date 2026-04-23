namespace UmbracoCms.Plugins.HiddenContent.Services;

public interface IHiddenContentService
{
    Task<IEnumerable<int>> GetHiddenNodesAsync();
    Task HideNodeAsync(int nodeId);
    Task ShowNodeAsync(int nodeId);
    Task<bool> IsHiddenAsync(int nodeId);
    Task BulkHideAsync(IEnumerable<int> nodeIds);
    Task BulkShowAsync(IEnumerable<int> nodeIds);
}
