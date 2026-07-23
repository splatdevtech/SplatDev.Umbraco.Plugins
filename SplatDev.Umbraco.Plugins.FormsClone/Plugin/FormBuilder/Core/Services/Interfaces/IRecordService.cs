using FormBuilder.Core.Models;
using FormBuilder.Core.Persistence.Fields;

namespace FormBuilder.Core.Services.Interfaces
{
    public interface IRecordService
    {
        Task SubmitAsync(Record record, Form? form);

        Task ApproveAsync(Record record, Form form);

        Task RejectAsync(Record record, Form form);

        Task DeleteAsync(Record record, Form form);

        IReadOnlyList<Record> GetAllRecords(Form form, bool includeFields = true);

        int GetRecordCount(Form form) => GetAllRecords(form, false).Count;
    }
}