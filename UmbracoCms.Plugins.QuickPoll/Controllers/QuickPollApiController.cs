using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Web.Common.Controllers;
using UmbracoCms.Plugins.QuickPoll.Models;
using UmbracoCms.Plugins.QuickPoll.Services;

namespace UmbracoCms.Plugins.QuickPoll.Controllers;

[Route("umbraco/api/quickpoll/[action]")]
public class QuickPollApiController : UmbracoApiController
{
    private readonly IQuickPollService _service;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public QuickPollApiController(IQuickPollService service, IHttpContextAccessor httpContextAccessor)
    {
        _service = service;
        _httpContextAccessor = httpContextAccessor;
    }

    [HttpGet]
    public async Task<IActionResult> GetActive(CancellationToken cancellationToken = default)
    {
        var poll = await _service.GetActivePollAsync(cancellationToken);
        return poll is null ? NotFound("No active poll found.") : Ok(poll);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken = default)
        => Ok(await _service.GetAllPollsAsync(cancellationToken));

    [HttpGet]
    public async Task<IActionResult> Get(int id, CancellationToken cancellationToken = default)
    {
        var poll = await _service.GetPollAsync(id, cancellationToken);
        return poll is null ? NotFound() : Ok(poll);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Poll poll, CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var created = await _service.CreatePollAsync(poll, cancellationToken);
        return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
    }

    [HttpDelete]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken = default)
    {
        var deleted = await _service.DeletePollAsync(id, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }

    [HttpPost]
    public async Task<IActionResult> Vote([FromBody] VoteRequest request, CancellationToken cancellationToken = default)
    {
        var voterIp = _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        var (success, message) = await _service.VoteAsync(request.PollId, request.OptionId, voterIp, cancellationToken);

        if (!success) return BadRequest(new { message });
        return Ok(new { message });
    }

    [HttpGet]
    public async Task<IActionResult> Results(int pollId, CancellationToken cancellationToken = default)
    {
        var results = await _service.GetResultsAsync(pollId, cancellationToken);
        return results is null ? NotFound() : Ok(results);
    }
}

public class VoteRequest
{
    public int PollId { get; set; }
    public int OptionId { get; set; }
}
