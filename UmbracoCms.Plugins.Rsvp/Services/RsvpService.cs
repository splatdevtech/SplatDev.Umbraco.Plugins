using Microsoft.EntityFrameworkCore;
using UmbracoCms.Plugins.Rsvp.Models;

namespace UmbracoCms.Plugins.Rsvp.Services;

public class RsvpService : IRsvpService
{
    private readonly RsvpDbContext _db;

    public RsvpService(RsvpDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<RsvpEvent>> GetEventsAsync(CancellationToken cancellationToken = default)
    {
        return await _db.RsvpEvents
            .Include(e => e.Attendees)
            .OrderBy(e => e.EventDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<RsvpEvent?> GetEventAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _db.RsvpEvents
            .Include(e => e.Attendees.OrderByDescending(a => a.RegisteredAt))
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    public async Task<RsvpEvent> CreateEventAsync(RsvpEvent rsvpEvent, CancellationToken cancellationToken = default)
    {
        _db.RsvpEvents.Add(rsvpEvent);
        await _db.SaveChangesAsync(cancellationToken);
        return rsvpEvent;
    }

    public async Task<RsvpEvent?> UpdateEventAsync(RsvpEvent rsvpEvent, CancellationToken cancellationToken = default)
    {
        var existing = await _db.RsvpEvents.FindAsync(new object[] { rsvpEvent.Id }, cancellationToken);
        if (existing is null) return null;

        existing.Title = rsvpEvent.Title;
        existing.Description = rsvpEvent.Description;
        existing.EventDate = rsvpEvent.EventDate;
        existing.Location = rsvpEvent.Location;
        existing.MaxCapacity = rsvpEvent.MaxCapacity;
        existing.RegistrationDeadline = rsvpEvent.RegistrationDeadline;
        existing.IsPublished = rsvpEvent.IsPublished;

        await _db.SaveChangesAsync(cancellationToken);
        return existing;
    }

    public async Task<bool> DeleteEventAsync(int id, CancellationToken cancellationToken = default)
    {
        var rsvpEvent = await _db.RsvpEvents.FindAsync(new object[] { id }, cancellationToken);
        if (rsvpEvent is null) return false;
        _db.RsvpEvents.Remove(rsvpEvent);
        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<RegistrationResult> RegisterAsync(RsvpAttendee attendee, CancellationToken cancellationToken = default)
    {
        var rsvpEvent = await _db.RsvpEvents
            .Include(e => e.Attendees)
            .FirstOrDefaultAsync(e => e.Id == attendee.EventId, cancellationToken);

        if (rsvpEvent is null)
            return new RegistrationResult(false, "Event not found.", null);

        if (!rsvpEvent.IsPublished)
            return new RegistrationResult(false, "This event is not open for registration.", null);

        if (rsvpEvent.RegistrationDeadline.HasValue && rsvpEvent.RegistrationDeadline.Value < DateTime.UtcNow)
            return new RegistrationResult(false, "Registration deadline has passed.", null);

        // Check for duplicate registration
        var existing = rsvpEvent.Attendees.FirstOrDefault(a =>
            string.Equals(a.Email, attendee.Email, StringComparison.OrdinalIgnoreCase)
            && a.Status != AttendeeStatus.Cancelled);
        if (existing is not null)
            return new RegistrationResult(false, "You are already registered for this event.", null);

        // Determine status based on capacity
        var confirmedCount = rsvpEvent.Attendees.Count(a => a.Status == AttendeeStatus.Confirmed);
        attendee.Status = (rsvpEvent.MaxCapacity.HasValue && confirmedCount >= rsvpEvent.MaxCapacity.Value)
            ? AttendeeStatus.Waitlisted
            : AttendeeStatus.Confirmed;

        attendee.RegisteredAt = DateTime.UtcNow;
        _db.RsvpAttendees.Add(attendee);
        await _db.SaveChangesAsync(cancellationToken);

        var message = attendee.Status == AttendeeStatus.Waitlisted
            ? "You have been added to the waitlist."
            : "Registration confirmed!";

        return new RegistrationResult(true, message, attendee);
    }

    public async Task<IReadOnlyList<RsvpAttendee>> GetAttendeesAsync(int eventId, CancellationToken cancellationToken = default)
    {
        return await _db.RsvpAttendees
            .Where(a => a.EventId == eventId)
            .OrderBy(a => a.Status)
            .ThenByDescending(a => a.RegisteredAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> CancelRegistrationAsync(int attendeeId, CancellationToken cancellationToken = default)
    {
        var attendee = await _db.RsvpAttendees
            .Include(a => a.Event)
                .ThenInclude(e => e!.Attendees)
            .FirstOrDefaultAsync(a => a.Id == attendeeId, cancellationToken);

        if (attendee is null) return false;

        attendee.Status = AttendeeStatus.Cancelled;

        // Promote first waitlisted attendee if capacity opens up
        if (attendee.Event?.MaxCapacity.HasValue == true)
        {
            var waitlisted = attendee.Event.Attendees
                .Where(a => a.Status == AttendeeStatus.Waitlisted)
                .OrderBy(a => a.RegisteredAt)
                .FirstOrDefault();
            if (waitlisted is not null)
                waitlisted.Status = AttendeeStatus.Confirmed;
        }

        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }
}
