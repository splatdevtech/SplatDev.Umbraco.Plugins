using FormBuilder.Core.Providers.Collections;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using Umbraco.Cms.Api.Common.ViewModels.Pagination;

using Umbraco.Cms.Api.Management.ViewModels.Tree;

using Umbraco.Cms.Core.IO;

namespace FormBuilder.Web.Api.Management.Controllers
{
    /// <summary>
    /// Management API controller for rendering the child items of the enail tenmplate's tree.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the         /// </remarks>
    public class ChildrenEmailTemplateTreeController(
      IIOHelper ioHelper,
      Umbraco.Cms.Core.Hosting.IHostingEnvironment hostingEnvironment,
      IHostEnvironment hostEnvironment,
      ILoggerFactory loggerFactory,
      EmailTemplateCollection emailTemplateCollection) : EmailTemplateTreeControllerBase(ioHelper, hostingEnvironment, hostEnvironment, loggerFactory, emailTemplateCollection)
    {

        /// <summary>
        /// Management API endpoint for rendering the child items of the email template's tree.
        /// </summary>
        [HttpGet("children/{parentPath}")]
        [ProducesResponseType(typeof(PagedViewModel<FileSystemTreeItemPresentationModel>), 200)]
        public async Task<ActionResult<PagedViewModel<FileSystemTreeItemPresentationModel>>> Children(
          string parentPath)
        {
            return await GetChildren(parentPath, 0, int.MaxValue);
        }
    }
}