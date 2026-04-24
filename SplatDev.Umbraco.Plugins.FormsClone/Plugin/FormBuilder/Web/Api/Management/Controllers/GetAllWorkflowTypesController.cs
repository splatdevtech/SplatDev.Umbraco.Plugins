using FormBuilder.Core.Configuration;
using FormBuilder.Core.Models;
using FormBuilder.Core.Providers.Collections;
using FormBuilder.Core.Workflows;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.Hosting;

namespace FormBuilder.Web.Api.Management.Controllers
{
    /// <summary>
    /// Management API controller for retrieving all workflow types.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the     /// </remarks>
    public class GetAllWorkflowTypesController(
      WorkflowCollection workflowCollection,
      IHostingEnvironment hostingEnvironment,
      IOptions<FormDesignSettings> formDesignSettings) : WorkflowTypeControllerBase(workflowCollection, hostingEnvironment, formDesignSettings)
    {
        /// <summary>
        /// Management API controller for retrieving all workflow types.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<WorkflowTypeWithSettings>), 200)]
        public IActionResult GetAll()
        {
            List<WorkflowTypeWithSettings> source = [];
            foreach (WorkflowType workflow in (BuilderCollectionBase<WorkflowType>)WorkflowCollection)
            {
                WorkflowTypeWithSettings typeWithSettings = CreateWorkflowTypeWithSettings(workflow);
                source.Add(typeWithSettings);
            }
            return Ok(source.OrderBy(x => x.Name));
        }
    }
}