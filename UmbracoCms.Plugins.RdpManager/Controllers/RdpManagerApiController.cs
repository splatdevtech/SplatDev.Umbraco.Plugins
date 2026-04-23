using Microsoft.AspNetCore.Mvc;

using Umbraco.Cms.Web.Common.Controllers;

using UmbracoCms.Plugins.RdpManager.Models;
using UmbracoCms.Plugins.RdpManager.Services;

namespace UmbracoCms.Plugins.RdpManager.Controllers
{
    [Route("umbraco/api/RdpManagerApi/[action]")]
    public class RdpManagerApiController(IRdpManagerService rdpManagerService) : UmbracoApiController
    {
        private readonly IRdpManagerService _rdpManagerService = rdpManagerService;

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var connections = await _rdpManagerService.GetAllAsync();
            return Ok(connections);
        }

        [HttpGet]
        public async Task<IActionResult> GetById(int id)
        {
            var connection = await _rdpManagerService.GetByIdAsync(id);
            if (connection is null)
                return NotFound();
            return Ok(connection);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] RdpConnection connection)
        {
            var created = await _rdpManagerService.CreateAsync(connection);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] RdpConnection connection)
        {
            var updated = await _rdpManagerService.UpdateAsync(connection);
            if (updated is null)
                return NotFound();
            return Ok(updated);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            await _rdpManagerService.DeleteAsync(id);
            return NoContent();
        }

        [HttpGet]
        public async Task<IActionResult> DownloadRdpFile(int id)
        {
            try
            {
                var rdpContent = await _rdpManagerService.GenerateRdpContentAsync(id);
                var connection = await _rdpManagerService.GetByIdAsync(id);
                var fileName = $"{connection?.Name ?? $"connection-{id}"}.rdp"
                    .Replace(" ", "_")
                    .Replace("/", "-")
                    .Replace("\\", "-");

                var bytes = System.Text.Encoding.UTF8.GetBytes(rdpContent);
                return new FileContentResult(bytes, "application/x-rdp")
                {
                    FileDownloadName = fileName
                };
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }
    }
}
