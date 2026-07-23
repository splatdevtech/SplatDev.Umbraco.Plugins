using FormBuilder.Core.FieldTypes;

using Microsoft.AspNetCore.Http;

namespace FormBuilder.Core.Providers.FieldTypes
{
    /// <summary>
    /// Provides base class functionality for the reCAPTCHA field types.
    /// </summary>
    [Serializable]
    public abstract class RecaptchaBase : FieldType
    {
        /// <summary>
        /// Retrieves the reCAPTCHA response for either traditional or API form submissions.
        /// </summary>
        protected static string GetRecaptchaResponse(HttpContext context)
        {
            string? recaptchaResponse;
            if (context.Request.HasFormContentType)
            {
                recaptchaResponse = context.Request.Form.ContainsKey("g-recaptcha-response") ? (string)context.Request.Form["g-recaptcha-response"]! : string.Empty;
            }
            else
            {
                IDictionary<string, IList<string>>? dictionary = context.Items.ContainsKey("ApiSubmittedFormValues") ? (IDictionary<string, IList<string>>)context.Items["ApiSubmittedFormValues"]! : throw new InvalidOperationException("Could not retrieve non-field form values from the HttpContext.");
                recaptchaResponse = dictionary.TryGetValue("g-recaptcha-response", out IList<string>? value) && value.Any() ? value[0] : throw new InvalidOperationException("Could not retrieve the reCAPTCHA response from the non-field form values stored on the HttpContext.");
            }
            return recaptchaResponse;
        }
    }
}