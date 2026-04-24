using FormBuilder.Core.Models;
using FormBuilder.Core.Searches;
using FormBuilder.Core.Searches.Interfaces;
using FormBuilder.Core.Security.Interfaces;
using FormBuilder.Core.Services.Interfaces;
using FormBuilder.Core.Storage.Interfaces;

using Microsoft.AspNetCore.Mvc;

namespace FormBuilder.Web.Api.Management.Controllers
{
    /// <summary>
    /// Management API controller for retrieving the meta data of a collection of records (form submissions).
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the         /// </remarks>
    public class GetMetadataController(
      IFormService formService,
      IRecordStorage recordStorage,
      IFormsSecurity formsSecurity,
      IPlaceholderParsingService placeholderParsingService,
      IFormRecordSearcher formRecordSearcher) : GetRecordsControllerBase(formService, recordStorage, formsSecurity, placeholderParsingService, formRecordSearcher)
    {

        /// <summary>
        /// Management API endpoint for retrieving the meta data of a collection of records (form submissions).
        /// </summary>
        [HttpGet("metadata")]
        [ProducesResponseType(typeof(EntrySearchResultMetadata), 200)]
        [ProducesResponseType(typeof(ProblemDetails), 400)]
        public IActionResult Retrieve(Guid formId, [FromQuery] RecordFilter model) => Ok(FormRecordSearcher.QueryDataBaseForMetadata(formId, model));
    }
}