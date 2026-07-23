using FormBuilder.Core.Enums;
using FormBuilder.Core.Models;
using FormBuilder.Core.Persistence.Fields;

namespace FormBuilder.Core.Storage.Interfaces
{
    public interface IRecordStorage
    {
        List<Record> GetAllRecords(Form form, bool includeFields = true);

        List<Record> GetRecords(IEnumerable<Guid> keys, Form form, bool includeFields = true);

        Record? GetRecord(int id, Form form);

        Record? GetRecordByUniqueId(Guid uniqueId, Form form);

        Record InsertRecord(Record record, Form form);

        void DeleteFormRecords(Form form);

        int DeleteFormRecords(Form form, FormState formState, DateTime deleteRecordsCreatedEarlierThan);

        void DeleteRecord(Record record, Form form);

        Record UpdateRecord(Record record, Form form);

        Record UpdateRecord(Record record, Form form, int? userId);

        int GetRecordCount(Form form) => GetAllRecords(form, false).Count;
    }
}