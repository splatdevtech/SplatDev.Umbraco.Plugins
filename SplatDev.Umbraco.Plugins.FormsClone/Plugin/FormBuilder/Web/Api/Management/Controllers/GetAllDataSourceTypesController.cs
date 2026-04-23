using FormBuilder.Core.Configuration;
using FormBuilder.Core.DataSources;
using FormBuilder.Core.Models;
using FormBuilder.Core.Providers.Collections;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.Hosting;

namespace FormBuilder.Web.Api.Management.Controllers
{
    /// <summary>
    /// Management API controller for retrieving all data source types.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the     /// </remarks>
    public class GetAllDataSourceTypesController(
      DataSourceTypeCollection dataSourceTypeCollection,
      IHostingEnvironment hostingEnvironment,
      IOptions<FormDesignSettings> formDesignSettings) : DataSourceTypeControllerBase(dataSourceTypeCollection, hostingEnvironment, formDesignSettings)
    {
        /// <summary>
        /// Management API controller for retrieving all data source types.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<DataSourceTypeWithSettings>), 200)]
        public IActionResult GetAll()
        {
            List<DataSourceTypeWithSettings> typeWithSettingsList = [];
            foreach (FormDataSourceType dataSourceType in (BuilderCollectionBase<FormDataSourceType>)DataSourceTypeCollection)
            {
                DataSourceTypeWithSettings typeWithSettings = CreateDataSourceTypeWithSettings(dataSourceType);
                typeWithSettingsList.Add(typeWithSettings);
            }
            return Ok(typeWithSettingsList);
        }
    }
}