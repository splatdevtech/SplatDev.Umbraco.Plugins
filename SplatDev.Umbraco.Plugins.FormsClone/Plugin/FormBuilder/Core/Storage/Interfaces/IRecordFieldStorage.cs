using FormBuilder.Core.Models;
using FormBuilder.Core.Persistence.Fields;

namespace FormBuilder.Core.Storage.Interfaces
{
    public interface IRecordFieldStorage
    {
        Dictionary<Guid, RecordField> GetAllRecordFields(
          Record record,
          Form form);

        Dictionary<Guid, RecordField> GetAllRecordFields(
          IEnumerable<Record> records,
          Form form);

        RecordField? GetRecordField(Guid key);

        IEnumerable<RecordField> InsertRecordFields(
          IEnumerable<RecordField> recordfields);

        RecordField InsertRecordField(RecordField recordfield);

        bool DeleteRecordField(RecordField recordField);

        RecordField UpdateRecordField(RecordField recordField);

        IEnumerable<RecordField> UpdateRecordFields(
          IEnumerable<RecordField> recordFields);
    }
}