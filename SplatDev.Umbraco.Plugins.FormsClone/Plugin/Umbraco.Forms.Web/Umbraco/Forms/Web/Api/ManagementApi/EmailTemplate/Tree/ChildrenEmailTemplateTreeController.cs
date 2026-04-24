
// Type: Umbraco.Forms.Web.Api.ManagementApi.EmailTemplate.Tree.ChildrenEmailTemplateTreeController
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using Umbraco.Cms.Api.Common.ViewModels.Pagination;
using Umbraco.Cms.Api.Management.ViewModels.Tree;
using Umbraco.Cms.Core.IO;
using Umbraco.Forms.Core.Providers;


#nullable enable
namespace Umbraco.Forms.Web.Api.ManagementApi.EmailTemplate.Tree
{
    public class ChildrenEmailTemplateTreeController : EmailTemplateTreeControllerBase
    {
        public ChildrenEmailTemplateTreeController(
          IIOHelper ioHelper,
          Umbraco.Cms.Core.Hosting.IHostingEnvironment hostingEnvironment,
          IHostEnvironment hostEnvironment,
          ILoggerFactory loggerFactory,
          EmailTemplateCollection emailTemplateCollection)
          : base(ioHelper, hostingEnvironment, hostEnvironment, loggerFactory, emailTemplateCollection)
        {
        }

        [HttpGet("children/{parentPath}")]
        [ProducesResponseType(typeof(PagedViewModel<FileSystemTreeItemPresentationModel>), 200)]
        public async Task<ActionResult<PagedViewModel<FileSystemTreeItemPresentationModel>>> Children(
          string parentPath)
        {
            return await this.GetChildren(parentPath, 0, int.MaxValue);
        }
    }
}
