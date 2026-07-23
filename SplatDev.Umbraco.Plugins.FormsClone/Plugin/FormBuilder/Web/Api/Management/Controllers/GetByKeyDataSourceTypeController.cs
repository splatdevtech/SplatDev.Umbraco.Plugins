using FormBuilder.Core.Configuration;
using FormBuilder.Core.DataSources;
using FormBuilder.Core.Models;
using FormBuilder.Core.Providers.Collections;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

using Umbraco.Cms.Core.Hosting;

namespace FormBuilder.Web.Api.Management.Controllers
{
    /// <summary>
    /// Management API controller for retrieving a single data source type by Id.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the     /// </remarks>
    public class GetByKeyDataSourceTypeController(
      DataSourceTypeCollection dataSourceTypeCollection,
      IHostingEnvironment hostingEnvironment,
      IOptions<FormDesignSettings> formDesignSettings) : DataSourceTypeControllerBase(dataSourceTypeCollection, hostingEnvironment, formDesignSettings)
    {
        /// <summary>
        /// Management API controller for retrieving a single data source type by Id.
        /// </summary>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(DataSourceTypeWithSettings), 200)]
        [ProducesResponseType(404)]
        public IActionResult GetByKey(Guid id)
        {
            FormDataSourceType? type = DataSourceTypeCollection.SingleOrDefault(x => x.Id == id);
            return type is null ? NotFound() : Ok(CreateDataSourceTypeWithSettings(type));
        }
    }
}