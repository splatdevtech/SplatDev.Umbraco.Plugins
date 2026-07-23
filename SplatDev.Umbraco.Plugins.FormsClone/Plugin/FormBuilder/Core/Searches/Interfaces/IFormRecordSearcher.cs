using FormBuilder.Core.Models;

namespace FormBuilder.Core.Searches.Interfaces
{
    public interface IFormRecordSearcher
    {
        EntrySearchResultCollection QueryDataBase(
          Guid formId,
          RecordFilter filter);

        EntrySearchResultMetadata QueryDataBaseForMetadata(
          Guid formId,
          RecordFilter fllter);

        int? GetPageNumberForRecord(Guid formId, RecordFilter filter);
    }
}