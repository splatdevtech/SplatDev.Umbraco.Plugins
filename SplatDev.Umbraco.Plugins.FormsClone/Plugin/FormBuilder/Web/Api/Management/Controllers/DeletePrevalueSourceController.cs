using FormBuilder.Core.Prevalues;
using FormBuilder.Core.Providers.Collections;
using FormBuilder.Core.Services.Interfaces;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FormBuilder.Web.Api.Management.Controllers
{
    /// <summary>
    /// Management API controller for deleting a prevalue source.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the     /// </remarks>
    [Authorize(Policy = "ManagePrevalueSources")]
    public class DeletePrevalueSourceController(
      IPrevalueSourceService prevalueSourceService,
      FieldPrevalueSourceCollection fieldPreValueSources,
      IFieldPrevalueSourceTypeService fieldPreValueSourceTypeService) : PrevalueSourceControllerBase(fieldPreValueSources, prevalueSourceService, fieldPreValueSourceTypeService)
    {
        /// <summary>Management API endpoint for deleting a prevalue source.</summary>
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public IActionResult DeleteByGuid(Guid id)
        {
            FieldPrevalueSource? fieldPreValueSource = PrevalueSourceService.Get(id);
            if (fieldPreValueSource is null)
                return NotFound();
            PrevalueSourceService.Delete(fieldPreValueSource);
            return Ok();
        }
    }
}