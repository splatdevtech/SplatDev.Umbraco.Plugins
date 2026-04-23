using FormBuilder.Core.Models;
using FormBuilder.Core.Persistence.Fields;

using Umbraco.Extensions;

namespace FormBuilder.Core.Extensions
{
    public static class RecordFieldExtensions
    {
        public static int FormFieldOrder(this RecordField recordField, IEnumerable<Field> formFields)
        {
            int index = formFields.FindIndex(x => x.Id == recordField.FieldId);
            return index != -1 ? index : int.MaxValue;
        }

        public static IEnumerable<string> GetSelectedPrevalues(this RecordField recordField)
        {
            if (recordField.Field is null)
                return [];
            IEnumerable<FieldPrevalue> preValues = recordField.Field.PreValues;
            string fieldValue = recordField.ValuesAsString();
            return RecordFieldExtensionsHelpers.GetSelectedPrevalues(preValues.Select(x => x.Value), fieldValue);
        }
    }
}