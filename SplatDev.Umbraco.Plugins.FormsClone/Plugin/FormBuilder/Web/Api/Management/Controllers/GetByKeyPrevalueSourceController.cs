using FormBuilder.Core.Prevalues;
using FormBuilder.Core.Providers.Collections;
using FormBuilder.Core.Services.Interfaces;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FormBuilder.Web.Api.Management.Controllers
{
    /// <summary>
    /// Management API controller for retrieving a single prevalue source by Id.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the     /// </remarks>
    [Authorize(Policy = "ManagePrevalueSources")]
    public class GetByKeyPrevalueSourceController(
      IPrevalueSourceService prevalueSourceService,
      FieldPrevalueSourceCollection fieldPreValueSources,
      IFieldPrevalueSourceTypeService fieldPreValueSourceTypeService) : PrevalueSourceControllerBase(fieldPreValueSources, prevalueSourceService, fieldPreValueSourceTypeService)
    {
        /// <summary>
        /// Management API endpoint for retrieving a single prevalue source by Id.
        /// </summary>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(FieldPrevalueSource), 200)]
        [ProducesResponseType(404)]
        public IActionResult GetByKey(Guid id)
        {
            FieldPrevalueSource? fieldPreValueSource = PrevalueSourceService.Get(id);
            return fieldPreValueSource is null ? NotFound() : Ok(fieldPreValueSource);
        }
    }
}