using FormBuilder.Core.DataSources;
using FormBuilder.Core.Providers.Collections;
using FormBuilder.Core.Services.Interfaces;

using Microsoft.AspNetCore.Mvc;

namespace FormBuilder.Web.Api.Management.Controllers
{
    /// <summary>Management API controller for deleting a datasource.</summary>
    /// <remarks>
    /// Initializes a new instance of the         /// </remarks>
    public class DeleteDataSourceController(
      IDataSourceService dataSourceService,
      DataSourceTypeCollection dataSourceTypeCollection) : DataSourceControllerBase(dataSourceService, dataSourceTypeCollection)
    {

        /// <summary>Management API endpoint for deleting a datasource.</summary>
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public IActionResult Delete(Guid id)
        {
            FormDataSource? formDataSource = DataSourceService.Get(id);
            if (formDataSource is null)
                return NotFound();
            DataSourceService.Delete(formDataSource);
            return Ok();
        }
    }
}