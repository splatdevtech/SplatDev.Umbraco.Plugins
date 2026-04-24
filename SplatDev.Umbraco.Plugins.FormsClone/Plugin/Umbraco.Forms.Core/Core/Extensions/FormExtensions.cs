
// Type: Umbraco.Forms.Core.Extensions.FormExtensions
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using Umbraco.Forms.Core.Models;
using Umbraco.Forms.Core.Services;


#nullable enable
namespace Umbraco.Forms.Core.Extensions
{
    public static class FormExtensions
    {
        public static async Task<Dictionary<Guid, Dictionary<string, string?>>> GetPrevalueMaps(
          this Form form,
          IFieldTypeStorage fieldTypeStorage,
          IPrevalueSourceService prevalueSourceService,
          IFieldPreValueSourceTypeService fieldPreValueSourceTypeService)
        {
            Dictionary<Guid, Dictionary<string, string>> maps = new Dictionary<Guid, Dictionary<string, string>>();
            if (form == null)
                return maps;
            foreach (Field allField in form.AllFields)
            {
                Field field = allField;
                FieldType fieldTypeByField = fieldTypeStorage.GetFieldTypeByField(field);
                if (fieldTypeByField != null && fieldTypeByField.SupportsPreValues)
                {
                    maps.Add(field.Id, (await FormExtensions.GetPrevaluesForFormField(field, form, prevalueSourceService, fieldPreValueSourceTypeService).ConfigureAwait(false)).GroupBy<PreValue, string>(x => x.Value).Select<IGrouping<string, PreValue>, PreValue>(x => x.First<PreValue>()).ToDictionary<PreValue, string, string>(x => x.Value, x => x.Caption));
                    field = null;
                }
            }
            return maps;
        }

        private static async Task<IList<PreValue>> GetPrevaluesForFormField(
          Field formField,
          Form form,
          IPrevalueSourceService prevalueSourceService,
          IFieldPreValueSourceTypeService fieldPreValueSourceTypeService)
        {
            List<PreValue> prevaluesForFormField = new List<PreValue>();
            if (formField.PreValueSourceId != Guid.Empty)
            {
                FieldPreValueSource fieldPreValueSource = prevalueSourceService.Get(formField.PreValueSourceId);
                if (fieldPreValueSource != null)
                {
                    FieldPreValueSourceType byId = fieldPreValueSourceTypeService.GetById(fieldPreValueSource.FieldPreValueSourceTypeId);
                    if (byId != null)
                    {
                        byId.LoadSettings(fieldPreValueSource);
                        prevaluesForFormField = await byId.GetPreValuesAsync(formField, form).ConfigureAwait(false);
                    }
                }
            }
            else
                prevaluesForFormField = formField.PreValues.Select<FieldPrevalue, PreValue>(x => new PreValue()
                {
                    Value = x.Value,
                    Caption = x.Caption
                }).ToList<PreValue>();
            return prevaluesForFormField;
        }
    }
}
