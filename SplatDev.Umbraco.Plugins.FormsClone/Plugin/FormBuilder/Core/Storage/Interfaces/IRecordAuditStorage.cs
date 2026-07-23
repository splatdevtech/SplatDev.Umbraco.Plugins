using FormBuilder.Core.Persistence.Fields;

namespace FormBuilder.Core.Storage.Interfaces
{
    public interface IRecordAuditStorage
    {
        List<RecordAudit> GetRecordAuditTrail(int recordId);
    }
}