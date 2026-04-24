using System.Dynamic;

namespace FormBuilder.Core.Models
{
    public static class FormsHtmlModelHelpers
    {
        public static object ToDynamic(IDictionary<string, FormFieldHtmlModel> fields)
        {
            ArgumentNullException.ThrowIfNull(fields);

            ExpandoObject dynamic = new();
            ICollection<KeyValuePair<string, object?>> keyValuePairs = dynamic;
            foreach (KeyValuePair<string, FormFieldHtmlModel> field in (IEnumerable<KeyValuePair<string, FormFieldHtmlModel>>)fields)
            {
                object? obj = null;
                if (field.Value is not null && field.Value.FieldValue is not null)
                    obj = field.Value.FieldValue.Length != 1 ? field.Value.FieldValue : field.Value.FieldValue[0];
                keyValuePairs.Add(new KeyValuePair<string, object?>(field.Key, obj));
            }
            return dynamic;
        }
    }
}