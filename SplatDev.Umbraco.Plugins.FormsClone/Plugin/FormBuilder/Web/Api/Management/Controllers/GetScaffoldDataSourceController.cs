using FormBuilder.Core.DataSources;
using FormBuilder.Core.Providers.Collections;
using FormBuilder.Core.Services.Interfaces;

using Microsoft.AspNetCore.Mvc;

namespace FormBuilder.Web.Api.Management.Controllers
{
    /// <summary>
    /// Management API controller for retrieving a data source scaffold.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the         /// </remarks>
    public class GetScaffoldDataSourceController(
      IDataSourceService dataSourceService,
      DataSourceTypeCollection dataSourceTypeCollectione) : DataSourceControllerBase(dataSourceService, dataSourceTypeCollectione)
    {

        /// <summary>
        /// Management API endpoint for retrieving a data source scaffold.
        /// </summary>
        [HttpGet("scaffold")]
        [ProducesResponseType(typeof(FormDataSource), 200)]
        [ProducesResponseType(404)]
        public IActionResult GetScaffold() => Ok(new FormDataSource()
        {
            Id = Guid.NewGuid()
        });
    }
}