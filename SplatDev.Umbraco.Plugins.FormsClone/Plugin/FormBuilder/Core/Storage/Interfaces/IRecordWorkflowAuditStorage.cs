using FormBuilder.Core.Persistence.Fields;

namespace FormBuilder.Core.Storage.Interfaces
{
    public interface IRecordWorkflowAuditStorage
    {
        List<RecordWorkflowAudit> GetRecordWorkflowAuditTrail(Guid recordId);
    }
}