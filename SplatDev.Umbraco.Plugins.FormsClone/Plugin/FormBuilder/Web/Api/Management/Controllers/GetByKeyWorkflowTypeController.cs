using FormBuilder.Core.Configuration;
using FormBuilder.Core.Models;
using FormBuilder.Core.Providers.Collections;
using FormBuilder.Core.Workflows;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

using Umbraco.Cms.Core.Hosting;

namespace FormBuilder.Web.Api.Management.Controllers
{
    /// <summary>
    /// Management API controller for retrieving a single workflow type by Id.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the     /// </remarks>
    public class GetByKeyWorkflowTypeController(
      WorkflowCollection workflowCollection,
      IHostingEnvironment hostingEnvironment,
      IOptions<FormDesignSettings> formDesignSettings) : WorkflowTypeControllerBase(workflowCollection, hostingEnvironment, formDesignSettings)
    {
        /// <summary>
        /// Management API controller for retrieving a single workflow type by Id.
        /// </summary>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(WorkflowTypeWithSettings), 200)]
        [ProducesResponseType(404)]
        public IActionResult GetByKey(Guid id)
        {
            WorkflowType? type = WorkflowCollection.SingleOrDefault(x => x.Id == id);
            return type is null ? NotFound() : Ok(CreateWorkflowTypeWithSettings(type));
        }
    }
}