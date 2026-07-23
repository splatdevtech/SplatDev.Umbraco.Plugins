using FormBuilder.Core.Prevalues;
using FormBuilder.Core.Providers.Collections;
using FormBuilder.Core.Services.Interfaces;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FormBuilder.Web.Api.Management.Controllers
{
    /// <summary>
    /// Management API controller for retrieving a prevalue source scaffold.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the     /// </remarks>
    [Authorize(Policy = "ManagePrevalueSources")]
    public class GetScaffoldPrevalueSourceController(
      IPrevalueSourceService prevalueSourceService,
      FieldPrevalueSourceCollection fieldPreValueSources,
      IFieldPrevalueSourceTypeService fieldPreValueSourceTypeService) : PrevalueSourceControllerBase(fieldPreValueSources, prevalueSourceService, fieldPreValueSourceTypeService)
    {
        /// <summary>
        /// Management API endpoint for retrieving a prevalue source scaffold.
        /// </summary>
        [HttpGet("scaffold")]
        [ProducesResponseType(typeof(FieldPrevalueSource), 200)]
        [ProducesResponseType(404)]
        public IActionResult GetScaffold() => Ok(new FieldPrevalueSource()
        {
            Id = Guid.NewGuid()
        });
    }
}