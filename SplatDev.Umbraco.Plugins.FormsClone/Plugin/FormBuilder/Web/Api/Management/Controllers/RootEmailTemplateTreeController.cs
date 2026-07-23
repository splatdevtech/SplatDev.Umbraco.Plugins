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
    /// Management API controller for rendering the root items of the email template's tree.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the     /// </remarks>
    public class RootEmailTemplateTreeController(
      IIOHelper ioHelper,
      Umbraco.Cms.Core.Hosting.IHostingEnvironment hostingEnvironment,
      IHostEnvironment hostEnvironment,
      ILoggerFactory loggerFactory,
      EmailTemplateCollection emailTemplateCollection) : EmailTemplateTreeControllerBase(ioHelper, hostingEnvironment, hostEnvironment, loggerFactory, emailTemplateCollection)
    {
        /// <summary>
        /// Management API endpoint for rendering the root items of the email template's tree.
        /// </summary>
        [HttpGet("root")]
        [ProducesResponseType(typeof(PagedViewModel<FileSystemTreeItemPresentationModel>), 200)]
        public async Task<ActionResult<PagedViewModel<FileSystemTreeItemPresentationModel>>> Root()
        {
            RootEmailTemplateTreeController templateTreeController = this;
            ActionResult<PagedViewModel<FileSystemTreeItemPresentationModel>> root = await templateTreeController.GetRoot(0, int.MaxValue);
            if (root.Result is null || ((ObjectResult)root.Result).Value is not PagedViewModel<FileSystemTreeItemPresentationModel> pagedViewModel)
                return root;
            List<FileSystemTreeItemPresentationModel> list = [.. pagedViewModel.Items];
            foreach (string str in templateTreeController.EmailTemplateCollection.Select(x => x.FileName).Reverse())
            {
                List<FileSystemTreeItemPresentationModel> presentationModelList = list;
                FileSystemTreeItemPresentationModel presentationModel = new()
                {
                    Path = "/" + str,
                    HasChildren = false,
                    IsFolder = false,
                    Name = str
                };
                presentationModelList.Insert(0, presentationModel);
            }
            return (ActionResult<PagedViewModel<FileSystemTreeItemPresentationModel>>)templateTreeController.Ok(new PagedViewModel<FileSystemTreeItemPresentationModel>()
            {
                Items = list,
                Total = list.Count
            });
        }
    }
}