using FormBuilder.Core.Models;
using FormBuilder.Core.Prevalues;
using FormBuilder.Core.Services.Interfaces;

namespace FormBuilder.Core.Helpers
{
    internal static class FormRenderingHelper
    {
        public static string FormatValidationErrorMessage(
          string? fieldErrorMessage,
          string formErrorMessage,
          string fieldCaption,
          IPlaceholderParsingService placeholderParsingService,
          Form form,
          IDictionary<string, string?>? additionalData)
        {
            string str = string.IsNullOrWhiteSpace(fieldErrorMessage) ? formErrorMessage : fieldErrorMessage;
            string placeHolders = placeholderParsingService.ParsePlaceHolders(str, false, form: form, additionalData: additionalData);
            return string.IsNullOrWhiteSpace(placeHolders) || !placeHolders.Contains("{0}") ? placeHolders : string.Format(placeHolders, placeholderParsingService.ParsePlaceHolders(fieldCaption, false, form: form, additionalData: additionalData));
        }

        public static async Task EnsurePrevalues(
          Form? form,
          Field? field,
          IFieldPrevalueSourceService fieldPreValueSourceService,
          IFieldPrevalueSourceTypeService fieldPreValueSourceTypeService)
        {
            if (field is null) return;
            if (!(field!.PreValueSourceId != Guid.Empty))
                return;
            FieldPrevalueSource? byId1 = fieldPreValueSourceService.GetById(field.PreValueSourceId);
            if (byId1 is null)
                return;
            FieldPrevalueSourceType? byId2 = fieldPreValueSourceTypeService.GetById(byId1.FieldPrevalueSourceTypeId);
            if (byId2 is null)
                return;
            byId2.LoadSettings(byId1);
            Field? field1 = field;
            field1.PreValues = [.. (await byId2.GetPreValuesAsync(field, form).ConfigureAwait(false)).Select(x => new FieldPrevalue()
            {
                Value = x.Value,
                Caption = x.Caption
            })];
            field1 = null;
        }
    }
}