using FormBuilder.Core.Models;
using FormBuilder.Core.Security.Interfaces;
using FormBuilder.Core.Services.Interfaces;

using Microsoft.AspNetCore.Mvc;

using Umbraco.Cms.Api.Common.ViewModels.Pagination;

namespace FormBuilder.Web.Api.Management.Controllers
{
    /// <summary>
    /// Management API controller for rendering the child items of the form's tree.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the     /// </remarks>
    public class ChildrenFormTreeController(
      IFormService formService,
      IFolderService folderService,
      IFormsSecurity formsSecurity) : FormTreeControllerBase(formService, folderService, formsSecurity)
    {
        /// <summary>
        /// Management API endpoint for rendering the child items of the form's tree.
        /// </summary>
        [HttpGet("children/{parentId:guid}")]
        [ProducesResponseType(typeof(PagedViewModel<FormTreeItemResponseModel>), 200)]
        public ActionResult<IEnumerable<FormTreeItemResponseModel>> Children(
          Guid parentId,
          bool foldersOnly = false,
          bool ignoreStartFolders = false)
        {
            FormTreeAccess accessForCurrentUser = GetFormTreeAccessForCurrentUser();
            if (accessForCurrentUser == FormTreeAccess.None)
                return (ActionResult<IEnumerable<FormTreeItemResponseModel>>)Ok(PagedViewModel<FormTreeItemResponseModel>.Empty());
            IEnumerable<FormTreeItemResponseModel> foldersAndForms = GetFoldersAndForms(foldersOnly, ignoreStartFolders, new Guid?(parentId));
            return (ActionResult<IEnumerable<FormTreeItemResponseModel>>)Ok(new PagedViewModel<FormTreeItemResponseModel>()
            {
                Items = foldersAndForms,
                Total = foldersAndForms.Count()
            });
        }
    }
}