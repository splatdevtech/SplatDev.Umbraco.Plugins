using FormBuilder.Core.Persistence.Fields;

namespace FormBuilder.Core.Storage.Interfaces
{
    public interface IRecordFieldValueStorage
    {
        List<object> GetRecordFieldValues(RecordField recordFieldInForm);

        List<object> InsertRecordFieldValues(RecordField recordFieldInForm);

        bool DeleteRecordFieldValues(RecordField recordFieldInForm);

        bool DeleteRecordFieldValues(IEnumerable<RecordField> recordFieldInForms);

        void DeleteAllRecordFieldValues(Record record);

        void DeleteAllRecordFieldValues(IList<int> recordIds);

        void DeleteAllRecordAuditValues(Record record);

        void DeleteAllRecordAuditValues(IList<int> recordIds);
    }
}