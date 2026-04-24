using FormBuilder.Core.Searches.Interfaces;
using FormBuilder.Core.Security.Interfaces;
using FormBuilder.Core.Services.Interfaces;
using FormBuilder.Core.Storage.Interfaces;

namespace FormBuilder.Web.Api.Management.Controllers
{
    /// <summary>
    /// Management API base controller for common functionality when working with read operations on records (form submissions).
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the         /// </remarks>
    public abstract class GetRecordsControllerBase(
      IFormService formService,
      IRecordStorage recordStorage,
      IFormsSecurity formsSecurity,
      IPlaceholderParsingService placeholderParsingService,
      IFormRecordSearcher formRecordSearcher) : RecordControllerBase(formService, recordStorage, formsSecurity, placeholderParsingService)
    {

        /// <summary>
        /// Gets the         /// </summary>
        protected IFormRecordSearcher FormRecordSearcher { get; } = formRecordSearcher;
    }
}