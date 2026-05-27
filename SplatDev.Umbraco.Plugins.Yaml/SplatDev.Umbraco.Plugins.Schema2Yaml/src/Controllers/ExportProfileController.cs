using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SplatDev.Umbraco.Plugins.Schema2Yaml.Models;
using SplatDev.Umbraco.Plugins.Schema2Yaml.Services;
using Umbraco.Cms.Web.Common.Authorization;

namespace SplatDev.Umbraco.Plugins.Schema2Yaml.Controllers;

[ApiController]
[Route("umbraco/api/[controller]/[action]")]
[Authorize(Policy = AuthorizationPolicies.SectionAccessSettings)]
public class ExportProfileController : ControllerBase
{
    private readonly IExportProfileService _profiles;
    private readonly ILogger<ExportProfileController> _logger;

    public ExportProfileController(IExportProfileService profiles, ILogger<ExportProfileController> logger)
    {
        _profiles = profiles ?? throw new ArgumentNullException(nameof(profiles));
        _logger   = logger   ?? throw new ArgumentNullException(nameof(logger));
    }

    // GET /umbraco/api/exportprofile/list
    [HttpGet]
    public async Task<IActionResult> List()
    {
        try { return Ok(await _profiles.GetAllAsync()); }
        catch (Exception ex) { return Err(ex, "list profiles"); }
    }

    // GET /umbraco/api/exportprofile/active
    [HttpGet]
    public async Task<IActionResult> Active()
    {
        try
        {
            var p = await _profiles.GetActiveAsync();
            return p is null ? NoContent() : Ok(p);
        }
        catch (Exception ex) { return Err(ex, "get active profile"); }
    }

    // GET /umbraco/api/exportprofile/get/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        try { return Ok(await _profiles.GetByIdAsync(id)); }
        catch (KeyNotFoundException) { return NotFound(); }
        catch (Exception ex) { return Err(ex, $"get profile {id}"); }
    }

    // POST /umbraco/api/exportprofile/create
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ProfileRequest request)
    {
        if (string.IsNullOrWhiteSpace(request?.Name))
            return BadRequest(new { error = "Name is required" });
        try { return Ok(await _profiles.CreateAsync(request.Name, request.Selection ?? new())); }
        catch (Exception ex) { return Err(ex, "create profile"); }
    }

    // PUT /umbraco/api/exportprofile/update/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] ProfileRequest request)
    {
        if (string.IsNullOrWhiteSpace(request?.Name))
            return BadRequest(new { error = "Name is required" });
        try { return Ok(await _profiles.UpdateAsync(id, request.Name, request.Selection ?? new())); }
        catch (KeyNotFoundException) { return NotFound(); }
        catch (Exception ex) { return Err(ex, $"update profile {id}"); }
    }

    // DELETE /umbraco/api/exportprofile/delete/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try { await _profiles.DeleteAsync(id); return NoContent(); }
        catch (Exception ex) { return Err(ex, $"delete profile {id}"); }
    }

    // POST /umbraco/api/exportprofile/activate/{id}
    [HttpPost("{id}")]
    public async Task<IActionResult> Activate(int id)
    {
        try { await _profiles.ActivateAsync(id); return Ok(); }
        catch (KeyNotFoundException) { return NotFound(); }
        catch (Exception ex) { return Err(ex, $"activate profile {id}"); }
    }

    // POST /umbraco/api/exportprofile/deactivate
    [HttpPost]
    public async Task<IActionResult> Deactivate()
    {
        try { await _profiles.DeactivateAsync(); return Ok(); }
        catch (Exception ex) { return Err(ex, "deactivate profile"); }
    }

    private ObjectResult Err(Exception ex, string op)
    {
        _logger.LogError(ex, "Failed to {Op}", op);
        return StatusCode(500, new { error = $"Failed to {op}", message = ex.Message });
    }
}

public class ProfileRequest
{
    public string Name { get; set; } = string.Empty;
    public ExportSelection? Selection { get; set; }
}
