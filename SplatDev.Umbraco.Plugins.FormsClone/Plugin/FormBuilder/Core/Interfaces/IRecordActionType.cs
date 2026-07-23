using FormBuilder.Core.Enums;
using FormBuilder.Core.Models;
using FormBuilder.Core.Persistence.Fields;

namespace FormBuilder.Core.Interfaces
{
    public interface IRecordActionType
    {
        string Icon { get; set; }

        string JsAction { get; set; }

        RecordActionStatus Execute(Record record, Form form);
    }
}