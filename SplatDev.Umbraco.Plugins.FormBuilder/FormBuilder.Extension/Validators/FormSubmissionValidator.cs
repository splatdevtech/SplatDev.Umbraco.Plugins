using FormBuilder.Extension.Interfaces;
using FormBuilder.Extension.Models;

using System.Text.RegularExpressions;

namespace FormBuilder.Extension.Validators
{
    public partial class FormSubmissionValidator(IFormRepository formRepository) : IFormSubmissionValidator
    {
        private readonly IFormRepository _formRepository = formRepository;

        public async Task<ValidationResultModel> ValidateAsync(FormSubmissionModel model)
        {
            var result = new ValidationResultModel();

            // Validate static fields
            if (model.FormId <= 0)
                result.Errors.Add("FormId is required.");

            if (string.IsNullOrWhiteSpace(model.Name))
                result.Errors.Add("Name is required.");

            if (string.IsNullOrWhiteSpace(model.Email))
                result.Errors.Add("Email is required.");
            else if (!IsValidEmail(model.Email))
                result.Errors.Add("Email is not valid.");

            // Get form definition for dynamic field validation
            var form = await _formRepository.GetByIdAsync(model.FormId);
            if (form == null)
            {
                result.Errors.Add("Form not found.");
                result.IsValid = false;
                return result;
            }

            foreach (var field in form.Fields)
            {
                // Required check
                if (field.IsRequired)
                {
                    if (!model.Fields.TryGetValue(field.Alias, out var value) || value == null || string.IsNullOrWhiteSpace(value.ToString()))
                    {
                        result.Errors.Add($"{field.Label} is required.");
                        continue;
                    }
                }

                // Min length check
                if (field.MinLength > 0 && model.Fields.TryGetValue(field.Alias, out var minValue))
                {
                    if (minValue?.ToString()?.Length < field.MinLength)
                        result.Errors.Add($"{field.Label} must be at least {field.MinLength} characters.");
                }

                // Regex check
                if (!string.IsNullOrWhiteSpace(field.Regex) && model.Fields.TryGetValue(field.Alias, out var regexValue))
                {
                    if (!Regex.IsMatch(regexValue?.ToString() ?? "", field.Regex))
                        result.Errors.Add($"{field.Label} is not in the correct format.");
                }
            }

            result.IsValid = result.Errors.Count == 0;
            return result;
        }

        private static bool IsValidEmail(string email)
        {
            // Simple email validation
            return EmailPattern().IsMatch(email);
        }

        [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$")]
        public static partial Regex EmailPattern();
    }
}
