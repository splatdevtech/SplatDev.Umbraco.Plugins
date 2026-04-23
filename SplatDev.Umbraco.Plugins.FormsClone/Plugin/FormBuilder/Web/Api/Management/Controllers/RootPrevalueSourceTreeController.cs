using FormBuilder.Core.Models;
using FormBuilder.Core.Prevalues;
using FormBuilder.Core.Services.Interfaces;

using Microsoft.AspNetCore.Mvc;

using Umbraco.Cms.Api.Common.ViewModels.Pagination;

using Umbraco.Cms.Api.Management.ViewModels.Tree;

namespace FormBuilder.Web.Api.Management.Controllers
{
    /// <summary>
    /// Management API controller for rendering the root items of the prevalue source's tree.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the     /// </remarks>
    public class RootPrevalueSourceTreeController(IPrevalueSourceService prevalueSourceService) : PrevalueSourceTreeControllerBase
    {
        private readonly IPrevalueSourceService _prevalueSourceService = prevalueSourceService;

        /// <summary>
        /// Management API endpoint for rendering the root items of the prevalue source's tree.
        /// </summary>
        [HttpGet("root")]
        [ProducesResponseType(typeof(PagedViewModel<PrevalueSourceTreeItemResponseModel>), 200)]
        public ActionResult<IEnumerable<FolderTreeItemResponseModel>> Root()
        {
            List<PrevalueSourceTreeItemResponseModel> source = [];
            foreach (FieldPrevalueSourceSlim prevalueSource in (IEnumerable<FieldPrevalueSourceSlim>)_prevalueSourceService.GetSlim().OrderBy(x => x.Name))
                source.Add(CreateItemResponse(prevalueSource));
            return (ActionResult<IEnumerable<FolderTreeItemResponseModel>>)Ok(new PagedViewModel<PrevalueSourceTreeItemResponseModel>()
            {
                Items = source,
                Total = source.Count
            });
        }

        private static PrevalueSourceTreeItemResponseModel CreateItemResponse(
          FieldPrevalueSourceSlim prevalueSource)
        {
            PrevalueSourceTreeItemResponseModel itemResponse = new()
            {
                Id = prevalueSource.Id,
                HasChildren = false,
                Name = prevalueSource.Name,
                IsFolder = false
            };
            return itemResponse;
        }
    }
}