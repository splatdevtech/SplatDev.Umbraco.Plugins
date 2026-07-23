using System.ComponentModel;

namespace FormBuilder.Core.Configuration
{
    public class Recaptcha3Settings
    {
        private const RecaptchaDomain StaticDomain = RecaptchaDomain.Google;
        private const string StaticVerificationUrl = "https://www.google.com/recaptcha/api/siteverify";

        public string SiteKey { get; set; } = string.Empty;

        public string PrivateKey { get; set; } = string.Empty;

        [DefaultValue(RecaptchaDomain.Google)]
        public RecaptchaDomain Domain { get; set; }

        [DefaultValue("https://www.google.com/recaptcha/api/siteverify")]
        public Uri VerificationUrl { get; set; } = new Uri("https://www.google.com/recaptcha/api/siteverify");

        public bool ShowFieldValidation { get; set; } = true;
    }
}