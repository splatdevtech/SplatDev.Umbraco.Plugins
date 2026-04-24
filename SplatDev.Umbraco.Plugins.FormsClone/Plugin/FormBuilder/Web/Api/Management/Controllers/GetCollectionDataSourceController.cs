using FormBuilder.Core.DataSources;
using FormBuilder.Core.Providers.Collections;
using FormBuilder.Core.Services.Interfaces;

using Microsoft.AspNetCore.Mvc;

using Umbraco.Cms.Api.Common.ViewModels.Pagination;

namespace FormBuilder.Web.Api.Management.Controllers
{
    /// <summary>
    /// Management API controller for retrieving a collection of data sources.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the         /// </remarks>
    public class GetCollectionDataSourceController(
      IDataSourceService dataSourceService,
      DataSourceTypeCollection dataSourceTypeCollectione) : DataSourceControllerBase(dataSourceService, dataSourceTypeCollectione)
    {

        /// <summary>
        /// Management API endpoint for retrieving a collection of data sources.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(PagedViewModel<FormDataSource>), 200)]
        public IActionResult GetCollection(int skip = 0, int take = 2147483647)
        {
            List<FormDataSource> list = [.. DataSourceService.Get()];
            return Ok(new PagedViewModel<FormDataSource>()
            {
                Total = list.Count,
                Items = list.Skip(skip).Take(take)
            });
        }
    }
}