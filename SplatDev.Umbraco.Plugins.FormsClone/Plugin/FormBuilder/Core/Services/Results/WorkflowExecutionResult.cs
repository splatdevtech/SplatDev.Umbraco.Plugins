using FormBuilder.Core.Persistence.Fields;

namespace FormBuilder.Core.Services.Results
{
    public class WorkflowExecutionResult
    {
        private readonly List<RecordWorkflowAudit> _records = [];

        private WorkflowExecutionResult()
        {
        }

        internal void AddRecordAudit(RecordWorkflowAudit auditRecord) => _records.Add(auditRecord);

        public IReadOnlyList<RecordWorkflowAudit> WorkflowAuditResults => _records.AsReadOnly();

        public bool HasCompletedWorkflows() => _records.Any(x => x.ExecutionStatus == 3);

        public static WorkflowExecutionResult Create() => new();
    }
}