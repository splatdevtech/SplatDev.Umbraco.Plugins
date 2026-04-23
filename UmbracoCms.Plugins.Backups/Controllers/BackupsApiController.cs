using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Web.Common.Controllers;
using UmbracoCms.Plugins.Backups.Models;
using UmbracoCms.Plugins.Backups.Services;

namespace UmbracoCms.Plugins.Backups.Controllers;

[Route("umbraco/api/backups/[action]")]
public class BackupsApiController : UmbracoApiController
{
    private readonly IBackupsService _service;

    public BackupsApiController(IBackupsService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var backups = await _service.ListBackupsAsync();
        return Ok(backups);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] BackupRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _service.CreateBackupAsync(request);
        return Ok(result);
    }

    [HttpDelete]
    public async Task<IActionResult> Delete([FromQuery] string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return BadRequest("Backup name is required.");

        try
        {
            await _service.DeleteBackupAsync(name);
            return Ok(new { message = $"Backup '{name}' deleted." });
        }
        catch (FileNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }
}
