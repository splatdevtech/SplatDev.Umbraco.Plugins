using FormBuilder.Core.Attributes;
using FormBuilder.Core.Configuration;
using FormBuilder.Core.Enums;
using FormBuilder.Core.Models;
using FormBuilder.Core.Options;
using FormBuilder.Core.Services.Interfaces;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using System.Globalization;
using System.Text.Json;

namespace FormBuilder.Core.Providers.FieldTypes
{
    /// <summary>Provides Recaptcha3 field type for a form.</summary>
    [Serializable]
    public class Recaptcha3 : RecaptchaBase
    {
        private const string HttpContextItemKeyForScore = "FormBuilders_Recaptcha3_Score";
        private readonly Recaptcha3Settings _config;
        private readonly ILogger<Recaptcha3> _logger;
        private readonly IHttpClientFactory _httpClientFactory;

        /// <summary>
        /// Initializes a new instance of the         /// </summary>
        public Recaptcha3(
          IOptionsMonitor<Recaptcha3Settings> config,
          ILogger<Recaptcha3> logger,
          IHttpClientFactory httpClientFactory)
        {
            _config = config.CurrentValue;
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            Id = new Guid("663AA19B-423D-4F38-A1D6-C840C926EF86");
            Name = "reCAPTCHA v3 with score";
            Alias = "recaptcha3";
            FieldTypeViewName = "FieldType.Recaptcha3.cshtml";
            PreviewView = "Forms.FieldPreview.RecaptchaV3";
            Description = "Google reCAPTCHA v3 with score threshold";
            Icon = "icon-eye";
            DataType = FieldDataType.String;
            SortOrder = 130;
            Category = "Simple";
            HideLabel = true;
            RenderInputType = config.CurrentValue.ShowFieldValidation ? RenderInputType.Single : RenderInputType.Custom;
        }

        /// <summary>
        /// Gets or sets the numeric threshold for the hidden score approval of 0.0 being a bot to 1.0 being all OK.
        /// PreValues are an array (min, max, step and default).
        /// </summary>
        [Setting("Score threshold", Description = "A reCAPTCHA v3 determined score between 0 and 10, above which form submissions are accepted. A higher value will catch more spam submissions, but also increase the risk of rejections of valid entries. For most sites, 5 is a sensible choice.", DisplayOrder = 10, PreValues = "0.0,1.0,0.1,0.5", View = "Umb.PropertyEditorUi.Slider")]
        public virtual string ScoreThreshold { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the error message to display when the user does not pass the reCAPTCHA check.
        /// </summary>
        [Setting("Error message", Description = "The error message to display when the user does not pass the reCAPTCHA check.", DisplayOrder = 20, SupportsPlaceholders = true, View = "Umb.PropertyEditorUi.TextArea")]
        public virtual string ErrorMessage { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether to save the reCAPTCHA score.
        /// </summary>
        [Setting("Save score", Description = "Save the calculated score with the form submission.", View = "Umb.PropertyEditorUi.Toggle")]
        public virtual string SaveScore { get; set; } = string.Empty;

        /// <inheritdoc />
        public override bool StoresData => string.Equals(SaveScore, "True", StringComparison.InvariantCultureIgnoreCase);

        /// <inheritdoc />
        public override IEnumerable<string> ValidateField(
          Form form,
          Field field,
          IEnumerable<object> postedValues,
          HttpContext context,
          IPlaceholderParsingService placeholderParsingService,
          IFieldTypeStorage fieldTypeStorage)
        {
            string str = ErrorMessage;
            if (string.IsNullOrWhiteSpace(str))
                str = "The Google reCAPTCHA failed to validate your submission.";
            string recaptchaResponse = GetRecaptchaResponse(context);
            if (!IsValidResponse(form, field, recaptchaResponse, out double score))
                return
                [
          str
                ];
            if (StoresData)
                context.Items.Add("FormBuilders_Recaptcha3_Score", score);
            return [];
        }

        /// <inheritdoc />
        public override IEnumerable<object> ConvertToRecord(
          Field field,
          IEnumerable<object> postedValues,
          HttpContext context)
        {
            if (!StoresData || !context.Items.ContainsKey("FormBuilders_Recaptcha3_Score"))
                return ConvertToRecord(field, postedValues, context);
            return
            [
                ((double) context.Items[ "FormBuilders_Recaptcha3_Score"]!).ToString()
            ];
        }

        /// <summary>Determines whether the specified response is valid.</summary>
        /// <param name="form">The form.</param>
        /// <param name="field">The field.</param>
        /// <param name="response">The reCAPTCHA response.</param>
        /// <param name="score">The score from the reCAPTCHA response provided as an out parameter.</param>
        /// <returns>
        ///   <c>true</c> if the response is valid; otherwise, <c>false</c>.
        /// </returns>
        private bool IsValidResponse(Form form, Field field, string response, out double score)
        {
            string privateKey = _config.PrivateKey;
            score = 0.0;
            if (string.IsNullOrWhiteSpace(privateKey))
            {
                _logger.LogWarning("The secret key for reCAPTCHA v3 is missing, please update FormBuilder.config to include the 'RecaptchaV3PrivateKey'");
                return false;
            }
            if (!double.TryParse(ScoreThreshold, NumberStyles.Number, CultureInfo.InvariantCulture, out double result))
            {
                _logger.LogWarning("Unable to parse score threshold '{ScoreThreshold}' for reCAPTCHA v3 field '{FieldAlias}' on form '{FormName}'. Using default value of 0.5.", ScoreThreshold, field.Alias, form.Name);
                result = 0.5;
            }
            if (string.IsNullOrWhiteSpace(response))
            {
                _logger.LogInformation("The 'g-recaptcha-response' hidden field for reCAPTCHA v3 is missing/empty and thus unable to be verified");
                return false;
            }
            HttpClient client = _httpClientFactory.CreateClient("Umbraco:Forms:HttpClients:Recaptcha3");
            try
            {
                UriBuilder uriBuilder = new(_config.VerificationUrl);
                QueryString queryString = new(uriBuilder.Query);
                queryString = queryString.Add("secret", privateKey);
                queryString = queryString.Add(nameof(response), response);
                uriBuilder.Query = queryString.ToUriComponent();
                Uri uri = uriBuilder.Uri;
                HttpResponseMessage httpResponseMessage = client.Send(new HttpRequestMessage(HttpMethod.Get, uri));
                if (!httpResponseMessage.IsSuccessStatusCode)
                {
                    _logger.LogWarning("The Google reCAPTCHA v3 validation failed for the field {FieldAlias} on {FormName}. Status code: {StatusCode}, reason: {Reason}", field.Alias, form.Name, httpResponseMessage.StatusCode, httpResponseMessage.ReasonPhrase);
                    return false;
                }
                ReCaptchaResponse reCaptchaResponse = JsonSerializer.Deserialize<ReCaptchaResponse>(httpResponseMessage.Content.ReadAsStream(), FormsJsonSerializerOptions.Default) ?? throw new InvalidOperationException("The reCAPTCHA response could not be deserialized into the expected type.");
                if (!reCaptchaResponse.Success)
                {
                    _logger.LogInformation("The Google reCAPTCHA v3 response was not valid for the field {FieldAlias} on {FormName}", field.Alias, form.Name);
                    return false;
                }
                if (reCaptchaResponse.Score < result)
                {
                    _logger.LogInformation("The Google reCAPTCHA v3 score {RecaptchaScore} did not meet the user defined threshold of {FieldThreshold} for the field {FieldAlias} on {FormName}", reCaptchaResponse.Score, result, field.Alias, form.Name);
                    return false;
                }
                score = reCaptchaResponse.Score;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "The Google reCAPTCHA v3 validation failed for the field {FieldAlias} on {FormName}", field.Alias, form.Name);
                return false;
            }
            return true;
        }
    }
}