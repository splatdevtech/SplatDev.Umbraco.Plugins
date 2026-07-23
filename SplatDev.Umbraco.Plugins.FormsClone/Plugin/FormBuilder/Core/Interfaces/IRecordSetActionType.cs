using FormBuilder.Core.Enums;
using FormBuilder.Core.Models;
using FormBuilder.Core.Persistence.Fields;

namespace FormBuilder.Core.Interfaces
{
    public interface IRecordSetActionType
    {
        string Alias { get; set; }

        string Icon { get; set; }

        Task<RecordActionStatus> ExecuteAsync(List<Record> records, Form form);
    }
}