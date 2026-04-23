using UmbracoCms.Plugins.Gdrp.Models;

namespace UmbracoCms.Plugins.Gdrp.Services;

public interface IGdrpService
{
    Task RecordConsent(string sessionId, string consentType, string? ip, string? userAgent);
    Task<ConsentRecord?> GetConsent(string sessionId);
    Task<DataRequest> SubmitDataRequest(string email, string requestType);
    Task<List<DataRequest>> GetDataRequests();
    Task CompleteDataRequest(int id);
}
