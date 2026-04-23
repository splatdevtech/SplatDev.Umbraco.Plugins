using FormBuilder.Core.Providers.Collections;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FormBuilder.Web.Api.Management.Controllers
{
    /// <summary>
    /// Management API base controller for common functionality when working with form workflows.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the     /// </remarks>
    [ApiExplorerSettings(GroupName = "Form")]
    [Authorize(Policy = "SectionAccessForms")]
    [Route("/formBuilder/management/api/v1/form-workflow")]
    public abstract class FormWorkflowControllerBase(WorkflowCollection workflowCollection) : FormsManagementApiControllerBase
    {
        /// <summary>
        /// Gets the         /// </summary>
        protected WorkflowCollection WorkflowCollection { get; } = workflowCollection;
    }
}