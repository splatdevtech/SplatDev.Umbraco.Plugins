
// Type: Umbraco.Forms.Core.Helpers.FormRenderingHelper
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using Umbraco.Forms.Core.Models;
using Umbraco.Forms.Core.Services;


#nullable enable
namespace Umbraco.Forms.Core.Helpers
{
    public static class FormRenderingHelper
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
          Form form,
          Field field,
          IFieldPreValueSourceService fieldPreValueSourceService,
          IFieldPreValueSourceTypeService fieldPreValueSourceTypeService)
        {
            if (!(field.PreValueSourceId != Guid.Empty))
                return;
            FieldPreValueSource byId1 = fieldPreValueSourceService.GetById(field.PreValueSourceId);
            if (byId1 == null)
                return;
            FieldPreValueSourceType byId2 = fieldPreValueSourceTypeService.GetById(byId1.FieldPreValueSourceTypeId);
            if (byId2 == null)
                return;
            byId2.LoadSettings(byId1);
            Field field1 = field;
            field1.PreValues = (await byId2.GetPreValuesAsync(field, form).ConfigureAwait(false)).Select<PreValue, FieldPrevalue>(x => new FieldPrevalue()
            {
                Value = x.Value,
                Caption = x.Caption
            }).ToList<FieldPrevalue>();
            field1 = null;
        }
    }
}
