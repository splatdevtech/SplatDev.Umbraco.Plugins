
// Type: Umbraco.Forms.Web.Api.ManagementApi.Folder.FolderControllerBase
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using Umbraco.Forms.Core.Services;


#nullable enable
namespace Umbraco.Forms.Web.Api.ManagementApi.Folder
{
  [ApiExplorerSettings(GroupName = "Folder")]
  [Route("/umbraco/forms/management/api/v1/folder")]
  [Authorize(Policy = "SectionAccessForms")]
  [Authorize(Policy = "ManageForms")]
  public abstract class FolderControllerBase : FormsManagementApiControllerBase
  {
    protected FolderControllerBase(IFolderService folderService) => this.FolderService = folderService;

    protected IFolderService FolderService { get; }

    protected IEnumerable<Umbraco.Forms.Core.Models.Folder> GetChildFolders(
      Guid? parentId)
    {
      return parentId.HasValue ? this.FolderService.GetChildren(parentId.Value) : this.FolderService.GetAtRoot();
    }
  }
}
