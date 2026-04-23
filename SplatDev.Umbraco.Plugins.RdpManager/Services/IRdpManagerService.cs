using SplatDev.Umbraco.Plugins.RdpManager.Models;

namespace SplatDev.Umbraco.Plugins.RdpManager.Services
{
    public interface IRdpManagerService
    {
        Task<IEnumerable<RdpConnection>> GetAllAsync();
        Task<RdpConnection?> GetByIdAsync(int id);
        Task<RdpConnection> CreateAsync(RdpConnection connection);
        Task<RdpConnection?> UpdateAsync(RdpConnection connection);
        Task DeleteAsync(int id);
        Task<string> GenerateRdpContentAsync(int id);
    }
}
