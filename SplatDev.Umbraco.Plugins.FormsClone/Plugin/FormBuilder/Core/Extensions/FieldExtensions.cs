using FormBuilder.Core.FieldTypes;
using FormBuilder.Core.Models;
using FormBuilder.Core.Services.Interfaces;

using Microsoft.AspNetCore.Http;

namespace FormBuilder.Core.Extensions
{
    internal static class FieldExtensions
    {
        public static void PopulateDefaultValue(
          this Field field,
          IPlaceholderParsingService placeholderParsingService,
          IDictionary<string, string?>? additionalData)
        {
            if (field.Values is not null && field.Values.Count > 0 && field.Values.All(x => x is not null) || !field.Settings.TryGetValue("DefaultValue", out string? str) || string.IsNullOrEmpty(str))
                return;
            field.Values =
            [
                placeholderParsingService.ParsePlaceHolders(str, false, additionalData: additionalData)
            ];
        }

        public static object[] GetValueToStore(
          this Field field,
          Dictionary<string, object[]> formState,
          IFieldTypeStorage fieldTypeStorage,
          HttpContext httpContext,
          bool includeNonIdempotentFieldTypes)
        {
            object[] postedValues = [];
            if (formState is not null)
            {
                Dictionary<string, object[]> dictionary1 = formState;
                Guid id = field.Id;
                string key1 = id.ToString();
                if (dictionary1.ContainsKey(key1))
                {
                    Dictionary<string, object[]> dictionary2 = formState;
                    id = field.Id;
                    string key2 = id.ToString();
                    postedValues = dictionary2[key2];
                }
            }
            FieldType? fieldTypeByField = fieldTypeStorage.GetFieldTypeByField(field);
            if (fieldTypeByField is null)
                return [];
            if (includeNonIdempotentFieldTypes || !fieldTypeByField.SupportsUploadTypes)
                postedValues = [.. fieldTypeByField.ConvertToRecord(field, postedValues, httpContext)];
            return postedValues;
        }
    }
}