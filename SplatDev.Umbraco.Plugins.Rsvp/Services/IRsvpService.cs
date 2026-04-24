using SplatDev.Umbraco.Plugins.Rsvp.Models;

namespace SplatDev.Umbraco.Plugins.Rsvp.Services;

public record RegistrationResult(bool Success, string Message, RsvpAttendee? Attendee);

public interface IRsvpService
{
    Task<IReadOnlyList<RsvpEvent>> GetEventsAsync(CancellationToken cancellationToken = default);
    Task<RsvpEvent?> GetEventAsync(int id, CancellationToken cancellationToken = default);
    Task<RsvpEvent> CreateEventAsync(RsvpEvent rsvpEvent, CancellationToken cancellationToken = default);
    Task<RsvpEvent?> UpdateEventAsync(RsvpEvent rsvpEvent, CancellationToken cancellationToken = default);
    Task<bool> DeleteEventAsync(int id, CancellationToken cancellationToken = default);
    Task<RegistrationResult> RegisterAsync(RsvpAttendee attendee, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<RsvpAttendee>> GetAttendeesAsync(int eventId, CancellationToken cancellationToken = default);
    Task<bool> CancelRegistrationAsync(int attendeeId, CancellationToken cancellationToken = default);
}
