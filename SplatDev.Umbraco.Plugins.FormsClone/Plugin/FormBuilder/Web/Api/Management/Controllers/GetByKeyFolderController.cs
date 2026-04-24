using FormBuilder.Core.Models;
using FormBuilder.Core.Services.Interfaces;

using Microsoft.AspNetCore.Mvc;

namespace FormBuilder.Web.Api.Management.Controllers
{
    /// <summary>
    /// Management API controller for retrieving a single datasource by Id.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the     /// </remarks>
    public class GetByKeyFolderController(IFolderService folderService) : FolderControllerBase(folderService)
    {
        /// <summary>
        /// Management API endpoint for retrieving a single datasource by Id.
        /// </summary>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(Folder), 200)]
        [ProducesResponseType(404)]
        public IActionResult GetByKey(Guid id)
        {
            Folder? folder = FolderService.Get(id);
            return folder is null ? NotFound() : Ok(folder);
        }
    }
}