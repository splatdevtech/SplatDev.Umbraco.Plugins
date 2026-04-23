using FormBuilder.Core.FieldTypes;
using FormBuilder.Core.Models;

namespace FormBuilder.Core.Services.Interfaces
{
    public interface IFieldTypeStorage
    {
        FieldType? GetFieldTypeByField(Field field);

        FieldType? GetFieldTypeByField(Guid fieldTypeId, IDictionary<string, string> settings);
    }
}