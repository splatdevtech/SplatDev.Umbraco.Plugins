using FormBuilder.Core.Models;
using FormBuilder.Core.Services.Interfaces;

using Microsoft.AspNetCore.Mvc;

namespace FormBuilder.Web.Api.Management.Controllers
{
    /// <summary>Management API controller for deleting a folder.</summary>
    /// <remarks>
    /// Initializes a new instance of the     /// </remarks>
    public class DeleteFolderController(IFolderService folderService) : FolderControllerBase(folderService)
    {
        /// <summary>Management API endpoint for deleting a folder.</summary>
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public IActionResult Delete(Guid id)
        {
            Folder? folder = FolderService.Get(id);
            if (folder is null)
                return NotFound();
            FolderService.Delete(folder);
            return Ok();
        }
    }
}