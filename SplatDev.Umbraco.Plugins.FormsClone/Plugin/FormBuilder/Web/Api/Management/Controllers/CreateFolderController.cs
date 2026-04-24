using FormBuilder.Core.Models;
using FormBuilder.Core.Services.Interfaces;

using Microsoft.AspNetCore.Mvc;

using Umbraco.Cms.Core.Models;

using Umbraco.Extensions;

namespace FormBuilder.Web.Api.Management.Controllers
{
    /// <summary>Management API controller for creating a folder.</summary>
    /// <remarks>
    /// Initializes a new instance of the     /// </remarks>
    public class CreateFolderController(IFolderService folderService) : FolderControllerBase(folderService)
    {
        /// <summary>Management API endpoint for creating a folder.</summary>
        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        public IActionResult Create(CreateFolderModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(new SimpleValidationModel(ModelState.ToErrorDictionary()));
            if (GetChildFolders(model.ParentId).Any(x => string.Equals(x.Name, model.Name, StringComparison.InvariantCultureIgnoreCase)))
            {
                ModelState.AddModelError("Name", "A folder already exists with the name '" + model.Name + "'.");
                return BadRequest(new SimpleValidationModel(ModelState.ToErrorDictionary()));
            }
            Core.Models.Folder folder = new()
            {
                Id = model.Id,
                Name = model.Name,
                ParentId = model.ParentId
            };
            FolderService.Insert(folder);
            return CreatedAtAction<GetByKeyFolderController>(controller => "GetByKey", folder.Id);
        }
    }
}