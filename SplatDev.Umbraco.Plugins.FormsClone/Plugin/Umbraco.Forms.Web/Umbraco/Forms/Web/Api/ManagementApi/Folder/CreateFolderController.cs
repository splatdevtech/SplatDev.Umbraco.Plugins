
// Type: Umbraco.Forms.Web.Api.ManagementApi.Folder.CreateFolderController
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Linq.Expressions;
using Umbraco.Cms.Core.Models;
using Umbraco.Extensions;
using Umbraco.Forms.Core.Services;
using Umbraco.Forms.Web.Models.ManagementApi.Form;


#nullable enable
namespace Umbraco.Forms.Web.Api.ManagementApi.Folder
{
  public class CreateFolderController : FolderControllerBase
  {
    public CreateFolderController(IFolderService folderService)
      : base(folderService)
    {
    }

    [HttpPost]
    [ProducesResponseType(201)]
    [ProducesResponseType(400)]
    public IActionResult Create(CreateFolderModel model)
    {
      if (!this.ModelState.IsValid)
        return (IActionResult) this.BadRequest((object) new SimpleValidationModel(ModelStateExtensions.ToErrorDictionary(this.ModelState)));
      if (this.GetChildFolders(model.ParentId).Any<Umbraco.Forms.Core.Models.Folder>((Func<Umbraco.Forms.Core.Models.Folder, bool>) (x => string.Equals(x.Name, model.Name, StringComparison.InvariantCultureIgnoreCase))))
      {
        this.ModelState.AddModelError("Name", "A folder already exists with the name '" + model.Name + "'.");
        return (IActionResult) this.BadRequest((object) new SimpleValidationModel(ModelStateExtensions.ToErrorDictionary(this.ModelState)));
      }
      Umbraco.Forms.Core.Models.Folder folder = new Umbraco.Forms.Core.Models.Folder()
      {
        Id = model.Id,
        Name = model.Name,
        ParentId = model.ParentId
      };
      this.FolderService.Insert(folder);
      return (IActionResult) this.CreatedAtAction<GetByKeyFolderController>((Expression<Func<GetByKeyFolderController, string>>) (controller => "GetByKey"), folder.Id);
    }
  }
}
