using FormBuilder.Core.FieldTypes;
using FormBuilder.Core.Models;
using FormBuilder.Core.Providers.Collections;
using FormBuilder.Core.Services.Interfaces;

using System.Reflection;

namespace FormBuilder.Core.Services
{
    internal sealed class FieldTypeStorage(FieldCollection fieldCollection) : IFieldTypeStorage
    {
        private readonly FieldCollection _fieldCollection = fieldCollection;

        public FieldType? GetFieldTypeByField(Field? field)
        {
            return field == null ? throw new ArgumentNullException(nameof(field)) : GetFieldTypeByField(field.FieldTypeId, field.Settings);
        }

        public FieldType? GetFieldTypeByField(
          Guid fieldTypeId,
          IDictionary<string, string> settings)
        {
            FieldType? field = _fieldCollection[fieldTypeId];
            if (field is null)
                return null;
            foreach (KeyValuePair<string, string> setting in (IEnumerable<KeyValuePair<string, string>>)settings)
            {
                PropertyInfo? property = field.GetType().GetProperty(setting.Key, BindingFlags.Instance | BindingFlags.Public);
                if (property is not null && property.CanWrite)
                    property.SetValue(field, setting.Value, null);
            }
            return field;
        }
    }
}