using Microsoft.AspNetCore.Mvc;
using UmbracoCms.Plugins.Rsvp.Services;

namespace UmbracoCms.Plugins.Rsvp.ViewComponents;

public class RsvpViewComponent : ViewComponent
{
    private readonly IRsvpService _service;

    public RsvpViewComponent(IRsvpService service)
    {
        _service = service;
    }

    public async Task<IViewComponentResult> InvokeAsync(int eventId, CancellationToken cancellationToken = default)
    {
        var rsvpEvent = await _service.GetEventAsync(eventId, cancellationToken);
        return View(rsvpEvent);
    }
}
