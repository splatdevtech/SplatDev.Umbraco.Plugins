using FormBuilder.Core.Models;
using FormBuilder.Core.Security.Interfaces;
using FormBuilder.Core.Services.Interfaces;

using Microsoft.AspNetCore.Mvc;

using Umbraco.Cms.Api.Common.ViewModels.Pagination;

namespace FormBuilder.Web.Api.Management.Controllers
{
    /// <summary>
    /// Management API controller for rendering the root items of the form's tree.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the     /// </remarks>
    public class RootFormTreeController(
      IFormService formService,
      IFolderService folderService,
      IFormsSecurity formsSecurity) : FormTreeControllerBase(formService, folderService, formsSecurity)
    {
        /// <summary>
        /// Management API endpoint for rendering the root items of the form's tree.
        /// </summary>
        [HttpGet("root")]
        [ProducesResponseType(typeof(PagedViewModel<FormTreeItemResponseModel>), 200)]
        public ActionResult<PagedViewModel<FormTreeItemResponseModel>> Root(
          bool foldersOnly = false,
          bool ignoreStartFolders = false)
        {
            FormTreeAccess accessForCurrentUser = GetFormTreeAccessForCurrentUser();
            if (accessForCurrentUser == FormTreeAccess.None)
                return (ActionResult<PagedViewModel<FormTreeItemResponseModel>>)Ok(PagedViewModel<FormTreeItemResponseModel>.Empty());
            IEnumerable<FormTreeItemResponseModel> foldersAndForms = GetFoldersAndForms(foldersOnly, ignoreStartFolders);
            return (ActionResult<PagedViewModel<FormTreeItemResponseModel>>)Ok(new PagedViewModel<FormTreeItemResponseModel>()
            {
                Items = foldersAndForms,
                Total = foldersAndForms.Count()
            });
        }
    }
}