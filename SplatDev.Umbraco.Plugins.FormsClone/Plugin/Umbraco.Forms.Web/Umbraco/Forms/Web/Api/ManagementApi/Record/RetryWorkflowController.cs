
// Type: Umbraco.Forms.Web.Api.ManagementApi.Record.RetryWorkflowController
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using Microsoft.AspNetCore.Mvc;

using System.Globalization;

using Umbraco.Forms.Core.Data.Storage;
using Umbraco.Forms.Core.Enums;
using Umbraco.Forms.Core.Models;
using Umbraco.Forms.Core.Security;
using Umbraco.Forms.Core.Services;
using Umbraco.Forms.Web.Models.Backoffice;


#nullable enable
namespace Umbraco.Forms.Web.Api.ManagementApi.Record
{
    public class RetryWorkflowController : RecordControllerBase
    {
        private readonly IWorkflowService _workflowService;
        private readonly IWorkflowExecutionService _workflowExecutionService;

        public RetryWorkflowController(
          IFormService formService,
          IRecordStorage recordStorage,
          IFormsSecurity formsSecurity,
          IPlaceholderParsingService placeholderParsingService,
          IWorkflowService workflowService,
          IWorkflowExecutionService workflowExecutionService)
          : base(formService, recordStorage, formsSecurity, placeholderParsingService)
        {
            this._workflowService = workflowService;
            this._workflowExecutionService = workflowExecutionService;
        }

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
            RetryWorkflowResult result = new RetryWorkflowResult();
            if (!workflowController.FormsSecurity.CanCurrentUserViewEntries())
                return workflowController.Forbid("Current user does not have permissions to view entries in the back-office.");
            Umbraco.Forms.Core.Models.Form form1 = workflowController.FormService.Get(formId);
            if (form1 == null)
                return workflowController.NotFound();
            Umbraco.Forms.Core.Persistence.Dtos.Record recordByUniqueId = workflowController.RecordStorage.GetRecordByUniqueId(recordId, form1);
            if (recordByUniqueId == null)
                return workflowController.NotFound();
            Workflow workflow = workflowController._workflowService.Get(form1).SingleOrDefault<Workflow>(x => x.Id == workflowId);
            if (workflow == null)
                return workflowController.NotFound();
            bool changedThreadCulture = false;
            CultureInfo currentThreadCulture = Thread.CurrentThread.CurrentCulture;
            CultureInfo currentThreadUiCulture = Thread.CurrentThread.CurrentUICulture;
            try
            {
                if (!string.IsNullOrEmpty(recordByUniqueId.Culture) && (currentThreadCulture.Name != recordByUniqueId.Culture || currentThreadUiCulture.Name != recordByUniqueId.Culture))
                {
                    CultureInfo cultureInfo = new CultureInfo(recordByUniqueId.Culture);
                    Thread.CurrentThread.CurrentCulture = cultureInfo;
                    Thread.CurrentThread.CurrentUICulture = cultureInfo;
                    changedThreadCulture = true;
                }
                IWorkflowExecutionService executionService = workflowController._workflowExecutionService;
                List<Workflow> workflows = new List<Workflow>();
                workflows.Add(workflow);
                Umbraco.Forms.Core.Persistence.Dtos.Record record = recordByUniqueId;
                Umbraco.Forms.Core.Models.Form form2 = form1;
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
