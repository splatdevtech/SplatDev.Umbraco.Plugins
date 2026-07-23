using FormBuilder.Core.Models;
using FormBuilder.Core.Services;
using FormBuilder.Core.Services.Interfaces;

using Microsoft.AspNetCore.Mvc;

using Umbraco.Cms.Core.Models;
using Umbraco.Extensions;

namespace FormBuilder.Web.Api.Management.Controllers
{
    /// <summary>Management API controller for moving a folder.</summary>
    /// <remarks>
    /// Initializes a new instance of the     /// </remarks>
    public class MoveFolderController(IFolderService folderService) : FolderControllerBase(folderService)
    {
        /// <summary>Management API endpoint for moving a folder.</summary>
        [HttpPut("{id:guid}/move")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult Move(Guid id, MoveFolderModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(new SimpleValidationModel(ModelState.ToErrorDictionary()));
            var folder = FolderService.Get(id);
            if (folder is null)
                return NotFound();
            if (GetChildFolders(model.ParentId).Any(x => x.Id != id && string.Equals(x.Name, folder.Name, StringComparison.OrdinalIgnoreCase)))
            {
                ModelState.AddModelError(string.Empty, "A folder already exists with the name '" + folder.Name + "' at the location selected.");
                return BadRequest(new SimpleValidationModel(ModelState.ToErrorDictionary()));
            }
            if (model.ParentId.HasValue && Array.IndexOf(FolderService.GetPath(model.ParentId.Value).Split(','), folder.Id.ToString()) > -1)
            {
                ModelState.AddModelError(string.Empty, "The destination folder selected is not valid.  You can't move a folder to a location below one of it's descendents.");
                return BadRequest(new SimpleValidationModel(ModelState.ToErrorDictionary()));
            }
            Guid? parentId = folder.ParentId;
            folder.ParentId = model.ParentId;

            if (parentId is null) return NotFound();

            if (FolderService is FolderService folderService)
            {
                Dictionary<string, object> additionalData = new()
                {
                  {
                    "MovedFromFolderId",
                     parentId
                  }
                };
                folderService.Update(folder, additionalData!);
            }
            else
                FolderService.Update(folder);
            return Ok();
        }
    }
}