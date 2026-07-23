
// Type: Umbraco.Forms.Core.Configuration.Recaptcha3Settings
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using System;
using System.ComponentModel;


#nullable enable
namespace Umbraco.Forms.Core.Configuration
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
