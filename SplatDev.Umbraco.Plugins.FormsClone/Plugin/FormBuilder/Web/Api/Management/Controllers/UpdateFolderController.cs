using FormBuilder.Core.Models;
using FormBuilder.Core.Services.Interfaces;

using Microsoft.AspNetCore.Mvc;

using Umbraco.Cms.Core.Models;

using Umbraco.Extensions;

namespace FormBuilder.Web.Api.Management.Controllers
{
    /// <summary>Management API controller for updating a folder.</summary>
    /// <remarks>
    /// Initializes a new instance of the     /// </remarks>
    public class UpdateFolderController(IFolderService folderService) : FolderControllerBase(folderService)
    {
        /// <summary>Management API endpont for updating a folder.</summary>
        [HttpPut("{id:guid}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(typeof(ProblemDetails), 500)]
        public IActionResult Update(Guid id, UpdateFolderModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(new SimpleValidationModel(ModelState.ToErrorDictionary()));
            Core.Models.Folder? folder = FolderService.Get(id);
            if (folder is null)
                return NotFound();
            if (GetChildFolders(folder.ParentId).Any(x => x.Id != id && string.Equals(x.Name, model.Name, StringComparison.InvariantCultureIgnoreCase)))
            {
                ModelState.AddModelError("Name", "A folder already exists with the name '" + model.Name + "'.");
                return BadRequest(new SimpleValidationModel(ModelState.ToErrorDictionary()));
            }
            folder.Name = model.Name;
            FolderService.Update(folder);
            return Ok();
        }
    }
}