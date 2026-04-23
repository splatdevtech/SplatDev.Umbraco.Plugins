namespace UmbracoCms.Plugins.JsonRpc.Services;

public interface IJsonRpcService
{
    Task<object?> GetContentByAlias(string alias);
    Task<object?> GetContentById(int id);
    Task<object?> SearchContent(string term);
}
