using FormBuilder.Core.Attributes;
using FormBuilder.Core.DataSources;
using FormBuilder.Core.Enums;
using FormBuilder.Core.Fields;
using FormBuilder.Core.Models;
using FormBuilder.Core.Persistence.Fields;

namespace FormBuilder.Core.Interfaces
{
    public interface IFormDataSourceType
    {
        Guid Id { get; set; }

        string Alias { get; set; }

        bool SupportsGetRecords { get; set; }

        bool SupportsInsert { get; set; }

        bool SupportsPreValues { get; set; }

        FormDataSource? DataSource { get; }

        Dictionary<object, FormDataSourceField> GetAvailableFields();

        Dictionary<object, string> GetPreValues(Field field, Form form);

        List<Record> GetRecords(
          Form form,
          int page,
          int maxItems,
          object sortByField,
          RecordSorting order);

        Record InsertRecord(Record record);

        Dictionary<string, SettingAttribute> Settings();

        List<Exception> ValidateSettings();
    }
}