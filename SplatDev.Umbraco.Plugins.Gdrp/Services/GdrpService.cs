using Microsoft.EntityFrameworkCore;
using SplatDev.Umbraco.Plugins.Gdrp.Models;

namespace SplatDev.Umbraco.Plugins.Gdrp.Services;

public class GdrpService : IGdrpService
{
    private readonly GdrpDbContext _db;

    public GdrpService(GdrpDbContext db)
    {
        _db = db;
    }

    public async Task RecordConsent(string sessionId, string consentType, string? ip, string? userAgent)
    {
        // Upsert: update existing consent record for session if present
        var existing = await _db.ConsentRecords
            .FirstOrDefaultAsync(c => c.SessionId == sessionId);

        if (existing is not null)
        {
            existing.ConsentType  = consentType;
            existing.ConsentDate  = DateTime.UtcNow;
            existing.IpAddress    = ip;
            existing.UserAgent    = userAgent;
            _db.ConsentRecords.Update(existing);
        }
        else
        {
            await _db.ConsentRecords.AddAsync(new ConsentRecord
            {
                SessionId   = sessionId,
                ConsentType = consentType,
                ConsentDate = DateTime.UtcNow,
                IpAddress   = ip,
                UserAgent   = userAgent
            });
        }

        await _db.SaveChangesAsync();
    }

    public async Task<ConsentRecord?> GetConsent(string sessionId)
    {
        return await _db.ConsentRecords
            .FirstOrDefaultAsync(c => c.SessionId == sessionId);
    }

    public async Task<DataRequest> SubmitDataRequest(string email, string requestType)
    {
        var request = new DataRequest
        {
            Email       = email,
            RequestType = requestType,
            Status      = "pending",
            RequestedAt = DateTime.UtcNow
        };

        await _db.DataRequests.AddAsync(request);
        await _db.SaveChangesAsync();
        return request;
    }

    public async Task<List<DataRequest>> GetDataRequests()
    {
        return await _db.DataRequests
            .OrderByDescending(r => r.RequestedAt)
            .ToListAsync();
    }

    public async Task CompleteDataRequest(int id)
    {
        var request = await _db.DataRequests.FindAsync(id);
        if (request is null) return;

        request.Status      = "completed";
        request.CompletedAt = DateTime.UtcNow;
        _db.DataRequests.Update(request);
        await _db.SaveChangesAsync();
    }
}
