
// Type: Umbraco.Forms.Core.Services.WorkflowExecutionResult
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Forms.Core.Persistence.Dtos;


#nullable enable
namespace Umbraco.Forms.Core.Services
{
  public class WorkflowExecutionResult
  {
    private readonly List<RecordWorkflowAudit> _records = new List<RecordWorkflowAudit>();

    private WorkflowExecutionResult()
    {
    }

    internal void AddRecordAudit(RecordWorkflowAudit auditRecord) => this._records.Add(auditRecord);

    public IReadOnlyList<RecordWorkflowAudit> WorkflowAuditResults => (IReadOnlyList<RecordWorkflowAudit>) this._records.AsReadOnly();

    public bool HasCompletedWorkflows() => this._records.Any<RecordWorkflowAudit>((Func<RecordWorkflowAudit, bool>) (x => x.ExecutionStatus == 3));

    public static WorkflowExecutionResult Create() => new WorkflowExecutionResult();
  }
}
