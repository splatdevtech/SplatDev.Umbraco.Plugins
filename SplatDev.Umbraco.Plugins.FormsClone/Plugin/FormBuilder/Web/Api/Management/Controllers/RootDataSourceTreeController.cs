using FormBuilder.Core.DataSources;
using FormBuilder.Core.Models;
using FormBuilder.Core.Services.Interfaces;

using Microsoft.AspNetCore.Mvc;

using Umbraco.Cms.Api.Common.ViewModels.Pagination;

namespace FormBuilder.Web.Api.Management.Controllers
{
    /// <summary>
    /// Management API controller for rendering the root items of the data source's tree.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the     /// </remarks>
    public class RootDataSourceTreeController(IDataSourceService dataSourceService) : DataSourceTreeControllerBase
    {
        private readonly IDataSourceService _dataSourceService = dataSourceService;

        /// <summary>
        /// Management API endpoint for rendering the root items of the prevalue source's tree.
        /// </summary>
        [HttpGet("root")]
        [ProducesResponseType(typeof(PagedViewModel<DataSourceTreeItemResponseModel>), 200)]
        public ActionResult<IEnumerable<DataSourceTreeItemResponseModel>> Root()
        {
            List<DataSourceTreeItemResponseModel> source = [];
            foreach (FormDataSourceSlim dataSource in (IEnumerable<FormDataSourceSlim>)_dataSourceService.GetSlim().OrderBy(x => x.Name))
                source.Add(CreateItemResponse(dataSource));
            return (ActionResult<IEnumerable<DataSourceTreeItemResponseModel>>)Ok(new PagedViewModel<DataSourceTreeItemResponseModel>()
            {
                Items = source,
                Total = source.Count
            });
        }

        private static DataSourceTreeItemResponseModel CreateItemResponse(
          FormDataSourceSlim dataSource)
        {
            DataSourceTreeItemResponseModel itemResponse = new()
            {
                Id = dataSource.Id,
                HasChildren = false,
                Name = dataSource.Name,
                IsFolder = false
            };
            return itemResponse;
        }
    }
}