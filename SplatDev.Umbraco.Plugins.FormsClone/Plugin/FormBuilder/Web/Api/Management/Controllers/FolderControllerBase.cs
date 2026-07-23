using FormBuilder.Core.Models;
using FormBuilder.Core.Services.Interfaces;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FormBuilder.Web.Api.Management.Controllers
{
    /// <summary>
    /// Management API base controller for common functionality when working with folders.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the     /// </remarks>
    [ApiExplorerSettings(GroupName = "Folder")]
    [Route("/formBuilder/management/api/v1/folder")]
    [Authorize(Policy = "SectionAccessForms")]
    [Authorize(Policy = "ManageForms")]
    public abstract class FolderControllerBase(IFolderService folderService) : FormsManagementApiControllerBase
    {
        /// <summary>
        /// Gets the         /// </summary>
        protected IFolderService FolderService { get; } = folderService;

        /// <summary>
        /// Gets a collection of folders found below the provided parent.
        /// </summary>
        protected IEnumerable<Folder> GetChildFolders(
          Guid? parentId)
        {
            return parentId.HasValue ? FolderService.GetChildren(parentId.Value) : FolderService.GetAtRoot();
        }
    }
}