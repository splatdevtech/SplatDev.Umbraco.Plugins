using FormBuilder.Core.DataSources;
using FormBuilder.Core.Providers.Collections;
using FormBuilder.Core.Services.Interfaces;

using Microsoft.AspNetCore.Mvc;

namespace FormBuilder.Web.Api.Management.Controllers
{
    /// <summary>
    /// Management API controller for retrieving a single datasource by Id.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the         /// </remarks>
    public class GetByKeyDataSourceController(
      IDataSourceService dataSourceService,
      DataSourceTypeCollection dataSourceTypeCollection) : DataSourceControllerBase(dataSourceService, dataSourceTypeCollection)
    {

        /// <summary>
        /// Management API endpoint for retrieving a single datasource by Id.
        /// </summary>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(FormDataSource), 200)]
        [ProducesResponseType(404)]
        public IActionResult GetByKey(Guid id)
        {
            FormDataSource? datasource = DataSourceService.Get(id);
            if (datasource is null)
                return NotFound();
            FormDataSourceType? formDataSourceType = datasource.GetFormDataSourceType(DataSourcesTypeCollection);
            if (formDataSourceType is null)
                return NotFound();
            formDataSourceType.LoadSettings(datasource);
            datasource.Valid = formDataSourceType.ValidateSettings().Count == 0;
            return Ok(datasource);
        }
    }
}