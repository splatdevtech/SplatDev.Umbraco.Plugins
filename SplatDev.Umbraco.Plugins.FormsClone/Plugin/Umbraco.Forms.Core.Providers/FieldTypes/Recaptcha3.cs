
// Type: Umbraco.Forms.Core.Providers.FieldTypes.Recaptcha3
// Assembly: Umbraco.Forms.Core.Providers, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 747CF8D1-007A-4431-9ECE-D9510ED65D68

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using System.Globalization;
using System.Text.Json;

using Umbraco.Forms.Core.Attributes;
using Umbraco.Forms.Core.Configuration;
using Umbraco.Forms.Core.Enums;
using Umbraco.Forms.Core.Models;
using Umbraco.Forms.Core.Services;


#nullable enable
namespace Umbraco.Forms.Core.Providers.FieldTypes
{
    [Serializable]
    public class Recaptcha3 : RecaptchaBase
    {
        private const string HttpContextItemKeyForScore = "UmbracoForms_Recaptcha3_Score";
        private readonly Recaptcha3Settings _config;
        private readonly ILogger<Recaptcha3> _logger;
        private readonly IHttpClientFactory _httpClientFactory;

        public Recaptcha3(
          IOptionsMonitor<Recaptcha3Settings> config,
          ILogger<Recaptcha3> logger,
          IHttpClientFactory httpClientFactory)
        {
            this._config = config.CurrentValue;
            this._logger = logger;
            this._httpClientFactory = httpClientFactory;
            this.Id = new Guid("663AA19B-423D-4F38-A1D6-C840C926EF86");
            this.Name = "reCAPTCHA v3 with score";
            this.Alias = "recaptcha3";
            this.FieldTypeViewName = "FieldType.Recaptcha3.cshtml";
            this.PreviewView = "Forms.FieldPreview.RecaptchaV3";
            this.Description = "Google reCAPTCHA v3 with score threshold";
            this.Icon = "icon-eye";
            this.DataType = FieldDataType.String;
            this.SortOrder = 130;
            this.Category = "Simple";
            this.HideLabel = true;
            this.RenderInputType = config.CurrentValue.ShowFieldValidation ? RenderInputType.Single : RenderInputType.Custom;
        }

        [Setting("Score threshold", Description = "A reCAPTCHA v3 determined score between 0 and 10, above which form submissions are accepted. A higher value will catch more spam submissions, but also increase the risk of rejections of valid entries. For most sites, 5 is a sensible choice.", DisplayOrder = 10, PreValues = "0.0,1.0,0.1,0.5", View = "Umb.PropertyEditorUi.Slider")]
        public virtual string ScoreThreshold { get; set; } = string.Empty;

        [Setting("Error message", Description = "The error message to display when the user does not pass the reCAPTCHA check.", DisplayOrder = 20, SupportsPlaceholders = true, View = "Umb.PropertyEditorUi.TextArea")]
        public virtual string ErrorMessage { get; set; } = string.Empty;

        [Setting("Save score", Description = "Save the calculated score with the form submission.", View = "Umb.PropertyEditorUi.Toggle")]
        public virtual string SaveScore { get; set; } = string.Empty;

        public override bool StoresData => string.Equals(this.SaveScore, "True", StringComparison.InvariantCultureIgnoreCase);

        public override IEnumerable<string> ValidateField(
          Form form,
          Field field,
          IEnumerable<object> postedValues,
          HttpContext context,
          IPlaceholderParsingService placeholderParsingService,
          IFieldTypeStorage fieldTypeStorage)
        {
            string str = this.ErrorMessage;
            if (string.IsNullOrWhiteSpace(str))
                str = "The Google reCAPTCHA failed to validate your submission.";
            string recaptchaResponse = RecaptchaBase.GetRecaptchaResponse(context);
            double score;
            if (!this.IsValidResponse(form, field, recaptchaResponse, out score))
                return new string[1]
                {
          str
                };
            if (this.StoresData)
                context.Items.Add("UmbracoForms_Recaptcha3_Score", score);
            return Enumerable.Empty<string>();
        }

        public override IEnumerable<object> ConvertToRecord(
          Field field,
          IEnumerable<object> postedValues,
          HttpContext context)
        {
            if (!this.StoresData || !context.Items.ContainsKey("UmbracoForms_Recaptcha3_Score"))
                return ConvertToRecord(field, postedValues, context);
            return new List<object>()
      {
         ((double) context.Items[ "UmbracoForms_Recaptcha3_Score"]).ToString()
      };
        }

        private bool IsValidResponse(Form form, Field field, string response, out double score)
        {
            string privateKey = this._config.PrivateKey;
            score = 0.0;
            if (string.IsNullOrWhiteSpace(privateKey))
            {
                this._logger.LogWarning("The secret key for reCAPTCHA v3 is missing, please update UmbracoForms.config to include the 'RecaptchaV3PrivateKey'");
                return false;
            }
            double result;
            if (!double.TryParse(this.ScoreThreshold, NumberStyles.Number, CultureInfo.InvariantCulture, out result))
            {
                this._logger.LogWarning("Unable to parse score threshold '{ScoreThreshold}' for reCAPTCHA v3 field '{FieldAlias}' on form '{FormName}'. Using default value of 0.5.", ScoreThreshold, field.Alias, form.Name);
                result = 0.5;
            }
            if (string.IsNullOrWhiteSpace(response))
            {
                this._logger.LogInformation("The 'g-recaptcha-response' hidden field for reCAPTCHA v3 is missing/empty and thus unable to be verified");
                return false;
            }
            HttpClient client = this._httpClientFactory.CreateClient("Umbraco:Forms:HttpClients:Recaptcha3");
            try
            {
                UriBuilder uriBuilder = new UriBuilder(this._config.VerificationUrl);
                QueryString queryString = new QueryString(uriBuilder.Query);
                queryString = queryString.Add("secret", privateKey);
                queryString = queryString.Add(nameof(response), response);
                uriBuilder.Query = queryString.ToUriComponent();
                Uri uri = uriBuilder.Uri;
                HttpResponseMessage httpResponseMessage = client.Send(new HttpRequestMessage(HttpMethod.Get, uri));
                if (!httpResponseMessage.IsSuccessStatusCode)
                {
                    this._logger.LogWarning("The Google reCAPTCHA v3 validation failed for the field {FieldAlias} on {FormName}. Status code: {StatusCode}, reason: {Reason}", field.Alias, form.Name, httpResponseMessage.StatusCode, httpResponseMessage.ReasonPhrase);
                    return false;
                }
                ReCaptchaResponse reCaptchaResponse = JsonSerializer.Deserialize<ReCaptchaResponse>(httpResponseMessage.Content.ReadAsStream(), FormsJsonSerializerOptions.Default) ?? throw new InvalidOperationException("The reCAPTCHA response could not be deserialized into the expected type.");
                if (!reCaptchaResponse.Success)
                {
                    this._logger.LogInformation("The Google reCAPTCHA v3 response was not valid for the field {FieldAlias} on {FormName}", field.Alias, form.Name);
                    return false;
                }
                if (reCaptchaResponse.Score < result)
                {
                    this._logger.LogInformation("The Google reCAPTCHA v3 score {RecaptchaScore} did not meet the user defined threshold of {FieldThreshold} for the field {FieldAlias} on {FormName}", reCaptchaResponse.Score, result, field.Alias, form.Name);
                    return false;
                }
                score = reCaptchaResponse.Score;
            }
            catch (Exception ex)
            {
                this._logger.LogWarning(ex, "The Google reCAPTCHA v3 validation failed for the field {FieldAlias} on {FormName}", field.Alias, form.Name);
                return false;
            }
            return true;
        }
    }
}
