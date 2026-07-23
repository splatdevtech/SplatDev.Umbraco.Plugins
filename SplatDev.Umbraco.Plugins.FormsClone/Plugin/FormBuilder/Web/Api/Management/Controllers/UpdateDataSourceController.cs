using FormBuilder.Core.DataSources;
using FormBuilder.Core.Providers;
using FormBuilder.Core.Providers.Collections;
using FormBuilder.Core.Services.Interfaces;

using Microsoft.AspNetCore.Mvc;

namespace FormBuilder.Web.Api.Management.Controllers
{
    /// <summary>Management API controller for updating a datasource.</summary>
    /// <remarks>
    /// Initializes a new instance of the         /// </remarks>
    public class UpdateDataSourceController(
      IDataSourceService dataSourceService,
      DataSourceTypeCollection dataSourceTypeCollection) : DataSourceControllerBase(dataSourceService, dataSourceTypeCollection)
    {

        /// <summary>Management API endpoint for updating a datasource.</summary>
        [HttpPut("{id:guid}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(ProblemDetails), 400)]
        [ProducesResponseType(typeof(ProblemDetails), 403)]
        [ProducesResponseType(typeof(ProblemDetails), 500)]
        public IActionResult Update(Guid id, FormDataSource dataSource)
        {
            _ = id;
            switch (TryValidateProviderType(dataSource, out ProblemDetails? problemDetails))
            {
                case ProviderValidationResult.FailedTypeNotFound:
                    return NotFound();

                case ProviderValidationResult.FailedValidation:
                    return BadRequest(problemDetails);

                default:
                    DataSourceService.Update(dataSource);
                    return Ok();
            }
        }
    }
}