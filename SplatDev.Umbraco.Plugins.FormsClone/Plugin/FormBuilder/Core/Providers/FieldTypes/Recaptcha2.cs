using FormBuilder.Core.Attributes;
using FormBuilder.Core.Configuration;
using FormBuilder.Core.Enums;
using FormBuilder.Core.Models;
using FormBuilder.Core.Services.Interfaces;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using System.Net;
using System.Text.Json.Nodes;

namespace FormBuilder.Core.Providers.FieldTypes
{
    /// <summary>Provides Recaptcha2 field type for a form.</summary>
    [Serializable]
    public class Recaptcha2 : RecaptchaBase
    {
        private readonly Recaptcha2Settings _config;
        private readonly ILogger<Recaptcha2> _logger;

        /// <summary>
        /// Initializes a new instance of the         /// </summary>
        public Recaptcha2(IOptionsMonitor<Recaptcha2Settings> config, ILogger<Recaptcha2> logger)
        {
            _config = config.CurrentValue;
            _logger = logger;
            Id = new Guid("B69DEAEB-ED75-4DC9-BFB8-D036BF9D3730");
            Name = nameof(Recaptcha2);
            Alias = "recaptcha2";
            FieldTypeViewName = "FieldType.Recaptcha2.cshtml";
            PreviewView = "Forms.FieldPreview.RecaptchaV2";
            Description = "Google Recaptcha v2";
            Icon = "icon-eye";
            DataType = FieldDataType.String;
            SortOrder = 120;
            Category = "Simple";
            ShowLabel = "True";
        }

        /// <summary>Gets or sets the field's theme.</summary>
        [Setting("Theme", Description = "ReCaptcha v2 theme", DisplayOrder = 10, PreValues = "light,dark", View = "Umb.PropertyEditorUi.Dropdown")]
        public virtual string Theme { get; set; } = string.Empty;

        /// <summary>Gets or sets the field size.</summary>
        [Setting("Size", Description = "ReCaptcha v2 size", DisplayOrder = 20, PreValues = "normal,compact", View = "Umb.PropertyEditorUi.Dropdown")]
        public virtual string Size { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the error message to display when the user does not pass the Recaptcha check.
        /// </summary>
        [Setting("Error message", Description = "The error message to display when the user does not pass the reCAPTCHA check", DisplayOrder = 30, SupportsPlaceholders = true)]
        public virtual string ErrorMessage { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether the field label should be shown.
        /// PreValues are a single element, a boolean indicating whether the default for the the checkbox is "checked".
        /// </summary>
        [Setting("Show Label", Description = "Indicate whether the the field's label should be shown when rendering the form.", DisplayOrder = 40, PreValues = "true", View = "Umb.PropertyEditorUi.Toggle")]
        public virtual string ShowLabel { get; set; }

        /// <inheritdoc />
        public override bool HideLabel => ShowLabel == "False";

        /// <inheritdoc />
        public override bool StoresData => false;

        /// <inheritdoc />
        public override IEnumerable<string> ValidateField(
          Form form,
          Field field,
          IEnumerable<object> postedValues,
          HttpContext context,
          IPlaceholderParsingService placeholderParsingService,
          IFieldTypeStorage fieldTypeStorage)
        {
            string privateKey = _config.PrivateKey;
            if (string.IsNullOrWhiteSpace(privateKey))
            {
                string message = "ERROR: ReCaptcha v2 is missing the Secret Key.  Please update the configuration to include a value at: " + Constants.Configuration.SectionKeys.FieldTypes.Recaptcha2 + ":PrivateKey'";
                _logger.LogWarning(message);
                return
                [
                    message
                ];
            }
            string recaptchaResponse = GetRecaptchaResponse(context);
            string address = "https://www.google.com/recaptcha/api/siteverify?secret=" + privateKey + "&response=" + recaptchaResponse;
            bool flag = false;
            List<string> collection1 = [];
#pragma warning disable SYSLIB0014 // Type or member is obsolete
#pragma warning disable IDE0090 // Use 'new(...)'
            using WebClient webClient = new WebClient();
#pragma warning restore IDE0090 // Use 'new(...)'
#pragma warning restore SYSLIB0014 // Type or member is obsolete
            JsonNode? jsonNode1 = JsonNode.Parse(webClient.DownloadString(address));
            JsonNode? jsonNode2 = jsonNode1?["success"];
            if (jsonNode2 is not null)
                flag = jsonNode2.AsValue().GetValue<bool>();
            if (jsonNode1?["error-codes"] is JsonArray source)
            {
                IEnumerable<string>? collection2 = source?.Select(x => x?.AsValue().GetValue<string>())!.Where(x => x is not null)!.Select<string, string>(x => x);
                if (collection2 is not null)
                    collection1.AddRange(collection2);
            }
            if (flag)
                return [];
            string str = field.Settings.TryGetValue("ErrorMessage", out string? value) ? value : "Make sure to complete the \"I am not a robot\" challenge";
            List<string> stringList = [str, .. collection1];
            return stringList;
        }
    }
}