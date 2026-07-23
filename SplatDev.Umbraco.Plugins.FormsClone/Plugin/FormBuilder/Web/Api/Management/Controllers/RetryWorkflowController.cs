using FormBuilder.Core.Enums;
using FormBuilder.Core.Models;
using FormBuilder.Core.Persistence.Fields;
using FormBuilder.Core.Security.Interfaces;
using FormBuilder.Core.Services.Interfaces;
using FormBuilder.Core.Services.Results;
using FormBuilder.Core.Storage.Interfaces;

using Microsoft.AspNetCore.Mvc;

using System.Globalization;

namespace FormBuilder.Web.Api.Management.Controllers
{
    /// <summary>
    /// Management API controller for re-running a workflow for a given record.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the     /// </remarks>
    public class RetryWorkflowController(
      IFormService formService,
      IRecordStorage recordStorage,
      IFormsSecurity formsSecurity,
      IPlaceholderParsingService placeholderParsingService,
      IWorkflowService workflowService,
      IWorkflowExecutionService workflowExecutionService) : RecordControllerBase(formService, recordStorage, formsSecurity, placeholderParsingService)
    {
        private readonly IWorkflowService _workflowService = workflowService;
        private readonly IWorkflowExecutionService _workflowExecutionService = workflowExecutionService;

        /// <summary>
        /// Management API endpoint for re-running a workflow for a given record.
        /// </summary>
        [HttpPost("{recordId:guid}/workflow/{workflowId:guid}/retry")]
        [ProducesResponseType(200)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Retry(
          Guid formId,
          Guid recordId,
          Guid workflowId)
        {
            RetryWorkflowController workflowController = this;
            RetryWorkflowResult result = new();
            if (!workflowController.FormsSecurity.CanCurrentUserViewEntries())
                return workflowController.Forbid("Current user does not have permissions to view entries in the back-office.");
            Form? form1 = workflowController.FormService.Get(formId);
            if (form1 is null)
                return workflowController.NotFound();
            Record? recordByUniqueId = workflowController.RecordStorage.GetRecordByUniqueId(recordId, form1);
            if (recordByUniqueId is null)
                return workflowController.NotFound();
            Workflow? workflow = workflowController._workflowService.Get(form1).SingleOrDefault(x => x.Id == workflowId);
            if (workflow is null)
                return workflowController.NotFound();
            bool changedThreadCulture = false;
            CultureInfo currentThreadCulture = Thread.CurrentThread.CurrentCulture;
            CultureInfo currentThreadUiCulture = Thread.CurrentThread.CurrentUICulture;
            try
            {
                if (!string.IsNullOrEmpty(recordByUniqueId.Culture) && (currentThreadCulture.Name != recordByUniqueId.Culture || currentThreadUiCulture.Name != recordByUniqueId.Culture))
                {
                    CultureInfo cultureInfo = new(recordByUniqueId.Culture);
                    Thread.CurrentThread.CurrentCulture = cultureInfo;
                    Thread.CurrentThread.CurrentUICulture = cultureInfo;
                    changedThreadCulture = true;
                }
                IWorkflowExecutionService executionService = workflowController._workflowExecutionService;
                List<Workflow> workflows = [workflow];
                Record record = recordByUniqueId;
                Form form2 = form1;
                int state = (int)recordByUniqueId.State;
                WorkflowExecutionResult workflowExecutionResult = await executionService.ExecuteWorkflowsAsync(workflows, record, form2, (FormState)state);
            }
            finally
            {
                if (changedThreadCulture)
                {
                    Thread.CurrentThread.CurrentCulture = currentThreadCulture;
                    Thread.CurrentThread.CurrentUICulture = currentThreadUiCulture;
                }
            }
            result.Success = true;
            return workflowController.Ok(result);
        }
    }
}