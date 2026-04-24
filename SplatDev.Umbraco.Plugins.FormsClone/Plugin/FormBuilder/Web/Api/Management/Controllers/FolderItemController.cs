using FormBuilder.Core.Models;
using FormBuilder.Core.Services.Interfaces;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FormBuilder.Web.Api.Management.Controllers
{
    /// <summary>Management API controller for retrieving folder items.</summary>
    /// <remarks>
    /// Initializes a new instance of the     /// </remarks>
    [ApiExplorerSettings(GroupName = "Folder")]
    [Authorize(Policy = "BackOfficeAccess")]
    [Route("/formBuilder/management/api/v1/item/folder")]
    public class FolderItemController(IFolderService folderService) : FormsManagementApiControllerBase
    {
        private readonly IFolderService _folderService = folderService;

        /// <summary>Management API controller for retrieving folder items.</summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<FolderItemResponseModel>), 200)]
        public IActionResult Item([FromQuery(Name = "id")] HashSet<Guid> ids)
        {
            List<Folder> source = [];
            foreach (Guid id in ids)
            {
                Folder? folder = _folderService.Get(id);
                if (folder is not null)
                    source.Add(folder);
            }
            return Ok(source.Select(x =>
            {
                return new FolderItemResponseModel()
                {
                    Id = x.Id,
                    Name = x.Name
                };
            }).ToList());
        }
    }
}