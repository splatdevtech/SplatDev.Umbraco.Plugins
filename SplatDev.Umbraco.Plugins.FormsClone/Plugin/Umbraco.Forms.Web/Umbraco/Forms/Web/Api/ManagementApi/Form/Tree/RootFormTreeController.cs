
// Type: Umbraco.Forms.Web.Api.ManagementApi.Form.Tree.RootFormTreeController
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Cms.Api.Common.ViewModels.Pagination;
using Umbraco.Forms.Core.Security;
using Umbraco.Forms.Core.Services;
using Umbraco.Forms.Web.Models.ManagementApi.Form.Tree;


#nullable enable
namespace Umbraco.Forms.Web.Api.ManagementApi.Form.Tree
{
  public class RootFormTreeController : FormTreeControllerBase
  {
    public RootFormTreeController(
      IFormService formService,
      IFolderService folderService,
      IFormsSecurity formsSecurity)
      : base(formService, folderService, formsSecurity)
    {
    }

    [HttpGet("root")]
    [ProducesResponseType(typeof (PagedViewModel<FormTreeItemResponseModel>), 200)]
    public ActionResult<PagedViewModel<FormTreeItemResponseModel>> Root(
      bool foldersOnly = false,
      bool ignoreStartFolders = false)
    {
      FormTreeControllerBase.FormTreeAccess accessForCurrentUser = this.GetFormTreeAccessForCurrentUser();
      if (accessForCurrentUser == FormTreeControllerBase.FormTreeAccess.None)
        return (ActionResult<PagedViewModel<FormTreeItemResponseModel>>) (ActionResult) this.Ok((object) PagedViewModel<FormTreeItemResponseModel>.Empty());
      IEnumerable<FormTreeItemResponseModel> foldersAndForms = this.GetFoldersAndForms(foldersOnly, ignoreStartFolders, accessForCurrentUser);
      return (ActionResult<PagedViewModel<FormTreeItemResponseModel>>) (ActionResult) this.Ok((object) new PagedViewModel<FormTreeItemResponseModel>()
      {
        Items = foldersAndForms,
        Total = (long) foldersAndForms.Count<FormTreeItemResponseModel>()
      });
    }
  }
}
