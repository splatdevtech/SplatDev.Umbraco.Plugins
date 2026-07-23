
// Type: Umbraco.Forms.Core.Providers.FieldTypes.Recaptcha2
// Assembly: Umbraco.Forms.Core.Providers, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 747CF8D1-007A-4431-9ECE-D9510ED65D68

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using System.Net;
using System.Text.Json.Nodes;

using Umbraco.Forms.Core.Attributes;
using Umbraco.Forms.Core.Configuration;
using Umbraco.Forms.Core.Enums;
using Umbraco.Forms.Core.Models;
using Umbraco.Forms.Core.Services;


#nullable enable
namespace Umbraco.Forms.Core.Providers.FieldTypes
{
    [Serializable]
    public class Recaptcha2 : RecaptchaBase
    {
        private readonly Recaptcha2Settings _config;
        private readonly ILogger<Recaptcha2> _logger;

        public Recaptcha2(IOptionsMonitor<Recaptcha2Settings> config, ILogger<Recaptcha2> logger)
        {
            this._config = config.CurrentValue;
            this._logger = logger;
            this.Id = new Guid("B69DEAEB-ED75-4DC9-BFB8-D036BF9D3730");
            this.Name = nameof(Recaptcha2);
            this.Alias = "recaptcha2";
            this.FieldTypeViewName = "FieldType.Recaptcha2.cshtml";
            this.PreviewView = "Forms.FieldPreview.RecaptchaV2";
            this.Description = "Google Recaptcha v2";
            this.Icon = "icon-eye";
            this.DataType = FieldDataType.String;
            this.SortOrder = 120;
            this.Category = "Simple";
            this.ShowLabel = "True";
        }

        [Setting("Theme", Description = "ReCaptcha v2 theme", DisplayOrder = 10, PreValues = "light,dark", View = "Umb.PropertyEditorUi.Dropdown")]
        public virtual string Theme { get; set; } = string.Empty;

        [Setting("Size", Description = "ReCaptcha v2 size", DisplayOrder = 20, PreValues = "normal,compact", View = "Umb.PropertyEditorUi.Dropdown")]
        public virtual string Size { get; set; } = string.Empty;

        [Setting("Error message", Description = "The error message to display when the user does not pass the reCAPTCHA check", DisplayOrder = 30, SupportsPlaceholders = true)]
        public virtual string ErrorMessage { get; set; } = string.Empty;

        [Setting("Show Label", Description = "Indicate whether the the field's label should be shown when rendering the form.", DisplayOrder = 40, PreValues = "true", View = "Umb.PropertyEditorUi.Toggle")]
        public virtual string ShowLabel { get; set; }

        public override bool HideLabel => this.ShowLabel == "False";

        public override bool StoresData => false;

        public override IEnumerable<string> ValidateField(
          Form form,
          Field field,
          IEnumerable<object> postedValues,
          HttpContext context,
          IPlaceholderParsingService placeholderParsingService,
          IFieldTypeStorage fieldTypeStorage)
        {
            string privateKey = this._config.PrivateKey;
            if (string.IsNullOrWhiteSpace(privateKey))
            {
                string message = "ERROR: ReCaptcha v2 is missing the Secret Key.  Please update the configuration to include a value at: " + Constants.Configuration.SectionKeys.FieldTypes.Recaptcha2 + ":PrivateKey'";
                this._logger.LogWarning(message);
                return new string[1]
                {
          message
                };
            }
            string recaptchaResponse = RecaptchaBase.GetRecaptchaResponse(context);
            string address = "https://www.google.com/recaptcha/api/siteverify?secret=" + privateKey + "&response=" + recaptchaResponse;
            bool flag = false;
            List<string> collection1 = new List<string>();
            using (WebClient webClient = new WebClient())
            {
                JsonNode jsonNode1 = JsonNode.Parse(webClient.DownloadString(address));
                JsonNode jsonNode2 = jsonNode1?["success"];
                if (jsonNode2 != null)
                    flag = jsonNode2.AsValue().GetValue<bool>();
                if (jsonNode1?["error-codes"] is JsonArray source)
                {
                    IEnumerable<string> collection2 = source.Select<JsonNode, string>(x => x?.AsValue().GetValue<string>()).Where<string>(x => x != null).Select<string, string>(x => x);
                    collection1.AddRange(collection2);
                }
                if (flag)
                    return Enumerable.Empty<string>();
                string str = field.Settings.ContainsKey("ErrorMessage") ? field.Settings["ErrorMessage"] : "Make sure to complete the \"I am not a robot\" challenge";
                List<string> stringList = new List<string>();
                stringList.Add(str);
                stringList.AddRange(collection1);
                return stringList;
            }
        }
    }
}
