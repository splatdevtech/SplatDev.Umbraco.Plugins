using FormBuilder.Core.DataSources;
using FormBuilder.Core.Providers;
using FormBuilder.Core.Providers.Collections;
using FormBuilder.Core.Services.Interfaces;

using Microsoft.AspNetCore.Mvc;

namespace FormBuilder.Web.Api.Management.Controllers
{
    /// <summary>Management API controller for creating a datasource.</summary>
    /// <remarks>
    /// Initializes a new instance of the     /// </remarks>
    public class CreateDataSourceController(
      IDataSourceService dataSourceService,
      DataSourceTypeCollection dataSourceTypeCollection) : DataSourceControllerBase(dataSourceService, dataSourceTypeCollection)
    {
        /// <summary>Management API endpoint for creating a datasource.</summary>
        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(typeof(ProblemDetails), 500)]
        public IActionResult Create(FormDataSource dataSource)
        {
            switch (TryValidateProviderType(dataSource, out ProblemDetails? problemDetails))
            {
                case ProviderValidationResult.FailedTypeNotFound:
                    return NotFound();

                case ProviderValidationResult.FailedValidation:
                    return BadRequest(problemDetails);

                default:
                    DataSourceService.Insert(dataSource);
                    return CreatedAtAction<GetByKeyDataSourceController>(controller => "GetByKey", dataSource.Id);
            }
        }
    }
}