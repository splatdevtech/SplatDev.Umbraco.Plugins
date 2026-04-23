using FormBuilder.Core.Configuration;
using FormBuilder.Core.Models;
using FormBuilder.Core.Prevalues;
using FormBuilder.Core.Providers.Collections;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.Hosting;

namespace FormBuilder.Web.Api.Management.Controllers
{
    /// <summary>
    /// Management API controller for retrieving all prevalue source types.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the     /// </remarks>
    public class GetAllPrevalueSourceTypesController(
      FieldPrevalueSourceCollection prevalueSourceTypeCollection,
      IHostingEnvironment hostingEnvironment,
      IOptions<FormDesignSettings> formDesignSettings) : PrevalueSourceTypeControllerBase(prevalueSourceTypeCollection, hostingEnvironment, formDesignSettings)
    {
        /// <summary>
        /// Management API controller for retrieving all prevalue source types.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<PrevalueSourceTypeWithSettings>), 200)]
        public IActionResult GetAll()
        {
            List<PrevalueSourceTypeWithSettings> typeWithSettingsList = [];
            foreach (FieldPrevalueSourceType prevalueSourceType in (BuilderCollectionBase<FieldPrevalueSourceType>)PrevalueSourceTypeCollection)
            {
                PrevalueSourceTypeWithSettings typeWithSettings = CreatePrevalueSourceTypeWithSettings(prevalueSourceType);
                typeWithSettingsList.Add(typeWithSettings);
            }
            return Ok(typeWithSettingsList);
        }
    }
}