using FormBuilder.Core.Security.Interfaces;
using FormBuilder.Core.Services.Interfaces;
using FormBuilder.Core.Storage.Interfaces;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FormBuilder.Web.Api.Management.Controllers
{
    /// <summary>
    /// Management API base controller for common functionality when working with records (form submissions).
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the     /// </remarks>
    [ApiExplorerSettings(GroupName = "Record")]
    [Authorize(Policy = "SectionAccessForms")]
    [Authorize(Policy = "ViewEntries")]
    [Route("/formBuilder/management/api/v1/form/{formId:guid}/record")]
    public abstract class RecordControllerBase(
      IFormService formService,
      IRecordStorage recordStorage,
      IFormsSecurity formsSecurity,
      IPlaceholderParsingService placeholderParsingService) : FormsManagementApiControllerBase
    {
        /// <summary>
        /// Gets the         /// </summary>
        protected IFormService FormService { get; } = formService;

        /// <summary>
        /// Gets the         /// </summary>
        protected IRecordStorage RecordStorage { get; } = recordStorage;

        /// <summary>
        /// Gets the         /// </summary>
        protected IFormsSecurity FormsSecurity { get; } = formsSecurity;

        /// <summary>
        /// Gets the         /// </summary>
        protected IPlaceholderParsingService PlaceholderParsingService { get; } = placeholderParsingService;
    }
}