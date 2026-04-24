using FormBuilder.Core.FieldTypes;
using FormBuilder.Core.Models;
using FormBuilder.Core.Prevalues;
using FormBuilder.Core.Services.Interfaces;

namespace FormBuilder.Core.Extensions
{
    internal static class FormExtensions
    {
        public static async Task<Dictionary<Guid, Dictionary<string, string?>?>?> GetPrevalueMaps(
          this Form form,
          IFieldTypeStorage fieldTypeStorage,
          IPrevalueSourceService prevalueSourceService,
          IFieldPrevalueSourceTypeService fieldPreValueSourceTypeService)
        {
            Dictionary<Guid, Dictionary<string, string?>?>? maps = [];
            if (form is null)
                return maps;
            foreach (Field allField in form.AllFields)
            {
                Field? field = allField;
                FieldType? fieldTypeByField = fieldTypeStorage.GetFieldTypeByField(field);
                if (fieldTypeByField is not null && fieldTypeByField.SupportsPreValues)
                {
                    maps.Add(field.Id, (await GetPrevaluesForFormField(field, form, prevalueSourceService, fieldPreValueSourceTypeService).ConfigureAwait(false)).GroupBy(static x => x.Value).Select(x => x.First()).ToDictionary(x => x.Value, x => x.Caption));
                    field = default;
                }
            }
            return maps;
        }

        private static async Task<IList<Prevalue>> GetPrevaluesForFormField(
          Field formField,
          Form form,
          IPrevalueSourceService prevalueSourceService,
          IFieldPrevalueSourceTypeService fieldPreValueSourceTypeService)
        {
            List<Prevalue> prevaluesForFormField = [];
            if (formField.PreValueSourceId != Guid.Empty)
            {
                FieldPrevalueSource? fieldPreValueSource = prevalueSourceService.Get(formField.PreValueSourceId);
                if (fieldPreValueSource is not null)
                {
                    FieldPrevalueSourceType? byId = fieldPreValueSourceTypeService.GetById(fieldPreValueSource.FieldPrevalueSourceTypeId);
                    if (byId is not null)
                    {
                        byId.LoadSettings(fieldPreValueSource);
                        prevaluesForFormField = await byId.GetPreValuesAsync(formField, form).ConfigureAwait(false);
                    }
                }
            }
            else
                prevaluesForFormField = [.. formField.PreValues.Select(x => new Prevalue()
                {
                    Value = x.Value,
                    Caption = x.Caption
                })];
            return prevaluesForFormField;
        }
    }
}