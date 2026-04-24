
// Type: Umbraco.Forms.Core.Services.PlaceholderParsingServiceExtensions
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using Umbraco.Forms.Core.Models;


#nullable enable
namespace Umbraco.Forms.Core.Services
{
    public static class PlaceholderParsingServiceExtensions
    {
        public static string ParsePlaceholdersForValidationErrorMessage(
          this IPlaceholderParsingService placeholderParsingService,
          Form form,
          Field field,
          string err)
        {
            string validationErrorMessage = err;
            if (string.IsNullOrWhiteSpace(err))
                validationErrorMessage = string.Format(placeholderParsingService.ParsePlaceHolders(form.InvalidErrorMessage ?? string.Empty, false, form: form), field.Caption);
            return validationErrorMessage;
        }

        public static string ParsePlaceHolders(
          this IPlaceholderParsingService placeholderParsingService,
          string value,
          bool htmlEncodeValues,
          WorkflowExecutionContext context)
        {
            return placeholderParsingService.ParsePlaceHolders(value, htmlEncodeValues, context.Record, context.Form, context.PageElements, context.AdditionalData);
        }
    }
}
