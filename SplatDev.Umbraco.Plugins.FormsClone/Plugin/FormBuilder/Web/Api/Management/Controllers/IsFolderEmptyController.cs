using FormBuilder.Core.Services.Interfaces;

using Microsoft.AspNetCore.Mvc;

namespace FormBuilder.Web.Api.Management.Controllers
{
    /// <summary>
    /// Management API controller for checking if a folder is empty.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the     /// </remarks>
    public class IsFolderEmptyController(IFolderService folderService) : FolderControllerBase(folderService)
    {
        /// <summary>
        /// Management API endpoint for checking if a folder is empty.
        /// </summary>
        [HttpGet("{id:guid}/is-empty")]
        [ProducesResponseType(typeof(bool), 200)]
        public IActionResult IeEmpty(Guid id) => Ok(FolderService.ExistsAndIsEmpty(id));
    }
}