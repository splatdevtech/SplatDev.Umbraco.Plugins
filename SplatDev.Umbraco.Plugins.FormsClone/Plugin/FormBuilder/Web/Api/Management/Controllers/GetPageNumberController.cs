using FormBuilder.Core.Models;
using FormBuilder.Core.Searches.Interfaces;
using FormBuilder.Core.Security.Interfaces;
using FormBuilder.Core.Services.Interfaces;
using FormBuilder.Core.Storage.Interfaces;

using Microsoft.AspNetCore.Mvc;

namespace FormBuilder.Web.Api.Management.Controllers
{
    /// <summary>
    /// Management API controller for retrieving the page pageNumber of a given record within a collection of records (form submissions).
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the         /// </remarks>
    public class GetPageNumberController(
      IFormService formService,
      IRecordStorage recordStorage,
      IFormsSecurity formsSecurity,
      IPlaceholderParsingService placeholderParsingService,
      IFormRecordSearcher formRecordSearcher) : GetRecordsControllerBase(formService, recordStorage, formsSecurity, placeholderParsingService, formRecordSearcher)
    {

        /// <summary>
        /// Management API endpoint for retrieving the page pageNumber of a given record within a collection of records (form submissions).
        /// </summary>
        [HttpGet("page-number")]
        [ProducesResponseType(typeof(int), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(ProblemDetails), 400)]
        public IActionResult GetPageNumber(Guid formId, [FromQuery] RecordFilter filter) => Ok(FormRecordSearcher.GetPageNumberForRecord(formId, filter));
    }
}