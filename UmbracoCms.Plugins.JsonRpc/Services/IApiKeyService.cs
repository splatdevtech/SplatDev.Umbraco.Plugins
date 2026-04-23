using UmbracoCms.Plugins.JsonRpc.Models;

namespace UmbracoCms.Plugins.JsonRpc.Services;

public interface IApiKeyService
{
    Task<List<ApiKey>> GetAll();
    Task<ApiKey> Create(string name, string permissions);
    Task Revoke(int id);
    Task<ApiKey?> ValidateKey(string rawKey);
}
