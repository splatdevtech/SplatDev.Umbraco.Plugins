
// Type: Umbraco.Forms.Web.Api.ManagementApi.EmailTemplate.Tree.RootEmailTemplateTreeController
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Umbraco.Cms.Api.Common.ViewModels.Pagination;
using Umbraco.Cms.Api.Management.ViewModels.Tree;
using Umbraco.Cms.Core.IO;
using Umbraco.Forms.Core.Interfaces;
using Umbraco.Forms.Core.Providers;


#nullable enable
namespace Umbraco.Forms.Web.Api.ManagementApi.EmailTemplate.Tree
{
  public class RootEmailTemplateTreeController : EmailTemplateTreeControllerBase
  {
    public RootEmailTemplateTreeController(
      IIOHelper ioHelper,
      Umbraco.Cms.Core.Hosting.IHostingEnvironment hostingEnvironment,
      IHostEnvironment hostEnvironment,
      ILoggerFactory loggerFactory,
      EmailTemplateCollection emailTemplateCollection)
      : base(ioHelper, hostingEnvironment, hostEnvironment, loggerFactory, emailTemplateCollection)
    {
    }

    [HttpGet("root")]
    [ProducesResponseType(typeof (PagedViewModel<FileSystemTreeItemPresentationModel>), 200)]
    public async Task<ActionResult<PagedViewModel<FileSystemTreeItemPresentationModel>>> Root()
    {
      RootEmailTemplateTreeController templateTreeController = this;
      ActionResult<PagedViewModel<FileSystemTreeItemPresentationModel>> root = await templateTreeController.GetRoot(0, int.MaxValue);
      if (root.Result == null || !(((ObjectResult) root.Result).Value is PagedViewModel<FileSystemTreeItemPresentationModel> pagedViewModel))
        return root;
      List<FileSystemTreeItemPresentationModel> list = pagedViewModel.Items.ToList<FileSystemTreeItemPresentationModel>();
      foreach (string str in templateTreeController.EmailTemplateCollection.Select<IEmailTemplate, string>((Func<IEmailTemplate, string>) (x => x.FileName)).Reverse<string>())
      {
        List<FileSystemTreeItemPresentationModel> presentationModelList = list;
        FileSystemTreeItemPresentationModel presentationModel = new FileSystemTreeItemPresentationModel();
        presentationModel.Path = "/" + str;
        presentationModel.HasChildren = false;
        presentationModel.IsFolder = false;
        presentationModel.Name = str;
        presentationModelList.Insert(0, presentationModel);
      }
      return (ActionResult<PagedViewModel<FileSystemTreeItemPresentationModel>>) (ActionResult) templateTreeController.Ok((object) new PagedViewModel<FileSystemTreeItemPresentationModel>()
      {
        Items = (IEnumerable<FileSystemTreeItemPresentationModel>) list,
        Total = (long) list.Count
      });
    }
  }
}
