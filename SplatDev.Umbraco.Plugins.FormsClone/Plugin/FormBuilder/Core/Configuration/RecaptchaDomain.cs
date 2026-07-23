using System.ComponentModel;

namespace FormBuilder.Core.Configuration
{
    public enum RecaptchaDomain
    {
        [Description("www.google.com")] Google,
        [Description("www.recaptcha.net")] Recaptcha,
    }
}