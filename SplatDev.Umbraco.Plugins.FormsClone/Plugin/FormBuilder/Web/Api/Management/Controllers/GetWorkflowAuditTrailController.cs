using FormBuilder.Core.Enums;
using FormBuilder.Core.Models;
using FormBuilder.Core.Security.Interfaces;
using FormBuilder.Core.Services.Interfaces;
using FormBuilder.Core.Storage.Interfaces;

using Microsoft.AspNetCore.Mvc;

namespace FormBuilder.Web.Api.Management.Controllers
{
    /// <summary>
    /// Management API controller for retrieving the workflow audit trail of a given record.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the     /// </remarks>
    public class GetWorkflowAuditTrailController(
      IFormService formService,
      IRecordStorage recordStorage,
      IFormsSecurity formsSecurity,
      IPlaceholderParsingService placeholderParsingService,
      IRecordWorkflowAuditStorage recordWorkflowAuditStorage) : RecordControllerBase(formService, recordStorage, formsSecurity, placeholderParsingService)
    {
        private readonly IRecordWorkflowAuditStorage _recordWorkflowAuditStorage = recordWorkflowAuditStorage;

        /// <summary>
        /// Management API endpoint for retrieving the workflow audit trail of a given record.
        /// </summary>
        [HttpGet("{recordId:guid}/workflow-audit-trail")]
        [ProducesResponseType(typeof(IEnumerable<RecordWorkflowAuditEntry>), 200)]
        [ProducesResponseType(404)]
        public IActionResult GetAuditTrail(Guid formId, Guid recordId)
        {
            Form? form = FormService.Get(formId);
            if (form is null)
                return NotFound();
            return RecordStorage.GetRecordByUniqueId(recordId, form) is null ? NotFound() : Ok(_recordWorkflowAuditStorage.GetRecordWorkflowAuditTrail(recordId).Select(x => new RecordWorkflowAuditEntry()
            {
                Id = x.Id,
                WorkflowKey = x.WorkflowKey,
                Name = x.WorkflowName,
                TypeName = x.WorkflowTypeName,
                ExecutedOn = x.ExecutedOn,
                ExecutionStage = x.ExecutionStage.HasValue ? ((FormState)x.ExecutionStage.Value).ToString() : string.Empty,
                Result = ((WorkflowExecutionStatus)x.ExecutionStatus).ToString()
            }).ToList());
        }
    }
}