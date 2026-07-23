
// Type: Umbraco.Forms.Web.Api.ManagementApi.Record.GetWorkflowAuditTrailController
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using Microsoft.AspNetCore.Mvc;

using Umbraco.Forms.Core.Data.Storage;
using Umbraco.Forms.Core.Enums;
using Umbraco.Forms.Core.Persistence.Dtos;
using Umbraco.Forms.Core.Security;
using Umbraco.Forms.Core.Services;
using Umbraco.Forms.Web.Models.Backoffice;


#nullable enable
namespace Umbraco.Forms.Web.Api.ManagementApi.Record
{
    public class GetWorkflowAuditTrailController : RecordControllerBase
    {
        private readonly IRecordWorkflowAuditStorage _recordWorkflowAuditStorage;

        public GetWorkflowAuditTrailController(
          IFormService formService,
          IRecordStorage recordStorage,
          IFormsSecurity formsSecurity,
          IPlaceholderParsingService placeholderParsingService,
          IRecordWorkflowAuditStorage recordWorkflowAuditStorage)
          : base(formService, recordStorage, formsSecurity, placeholderParsingService)
        {
            this._recordWorkflowAuditStorage = recordWorkflowAuditStorage;
        }

        [HttpGet("{recordId:guid}/workflow-audit-trail")]
        [ProducesResponseType(typeof(IEnumerable<RecordWorkflowAuditEntry>), 200)]
        [ProducesResponseType(404)]
        public IActionResult GetAuditTrail(Guid formId, Guid recordId)
        {
            Umbraco.Forms.Core.Models.Form form = this.FormService.Get(formId);
            if (form == null)
                return this.NotFound();
            return this.RecordStorage.GetRecordByUniqueId(recordId, form) == null ? this.NotFound() : this.Ok(this._recordWorkflowAuditStorage.GetRecordWorkflowAuditTrail(recordId).Select<RecordWorkflowAudit, RecordWorkflowAuditEntry>(x => new RecordWorkflowAuditEntry()
            {
                Id = x.Id,
                WorkflowKey = x.WorkflowKey,
                Name = x.WorkflowName,
                TypeName = x.WorkflowTypeName,
                ExecutedOn = x.ExecutedOn,
                ExecutionStage = x.ExecutionStage.HasValue ? ((FormState)x.ExecutionStage.Value).ToString() : string.Empty,
                Result = ((WorkflowExecutionStatus)x.ExecutionStatus).ToString()
            }).ToList<RecordWorkflowAuditEntry>());
        }
    }
}
