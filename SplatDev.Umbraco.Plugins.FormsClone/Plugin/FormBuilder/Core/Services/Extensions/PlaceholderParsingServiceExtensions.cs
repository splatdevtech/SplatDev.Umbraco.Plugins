using FormBuilder.Core.Models;
using FormBuilder.Core.Services.Interfaces;
using FormBuilder.Core.Workflows;

namespace FormBuilder.Core.Services.Extensions
{
    internal static class PlaceholderParsingServiceExtensions
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