
// Type: Umbraco.Forms.Core.Extensions.FieldExtensions
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using Microsoft.AspNetCore.Http;

using Umbraco.Forms.Core.Models;
using Umbraco.Forms.Core.Services;


#nullable enable
namespace Umbraco.Forms.Core.Extensions
{
    public static class FieldExtensions
    {
        public static void PopulateDefaultValue(
          this Field field,
          IPlaceholderParsingService placeholderParsingService,
          IDictionary<string, string?>? additionalData)
        {
            string str;
            if (field.Values != null && field.Values.Count > 0 && field.Values.All<object>(x => x != null) || !field.Settings.TryGetValue("DefaultValue", out str) || string.IsNullOrEmpty(str))
                return;
            field.Values = new List<object>()
      {
         placeholderParsingService.ParsePlaceHolders(str, false, additionalData: additionalData)
      };
        }

        public static object[] GetValueToStore(
          this Field field,
          Dictionary<string, object[]> formState,
          IFieldTypeStorage fieldTypeStorage,
          HttpContext httpContext,
          bool includeNonIdempotentFieldTypes)
        {
            object[] postedValues = Array.Empty<object>();
            if (formState != null)
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
            FieldType fieldTypeByField = fieldTypeStorage.GetFieldTypeByField(field);
            if (fieldTypeByField == null)
                return Array.Empty<object>();
            if (includeNonIdempotentFieldTypes || !fieldTypeByField.SupportsUploadTypes)
                postedValues = fieldTypeByField.ConvertToRecord(field, postedValues, httpContext).ToArray<object>();
            return postedValues;
        }
    }
}
