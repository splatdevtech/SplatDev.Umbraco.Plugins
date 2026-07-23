
// Type: Umbraco.Forms.Core.Providers.FieldTypes.RecaptchaBase
// Assembly: Umbraco.Forms.Core.Providers, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 747CF8D1-007A-4431-9ECE-D9510ED65D68

using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;


#nullable enable
namespace Umbraco.Forms.Core.Providers.FieldTypes
{
  [Serializable]
  public abstract class RecaptchaBase : FieldType
  {
    protected static string GetRecaptchaResponse(HttpContext context)
    {
      string recaptchaResponse;
      if (context.Request.HasFormContentType)
      {
        recaptchaResponse = context.Request.Form.ContainsKey("g-recaptcha-response") ? (string) context.Request.Form["g-recaptcha-response"] : string.Empty;
      }
      else
      {
        IDictionary<string, IList<string>> dictionary = context.Items.ContainsKey((object) "ApiSubmittedFormValues") ? (IDictionary<string, IList<string>>) context.Items[(object) "ApiSubmittedFormValues"] : throw new InvalidOperationException("Could not retrieve non-field form values from the HttpContext.");
        recaptchaResponse = dictionary.ContainsKey("g-recaptcha-response") && dictionary["g-recaptcha-response"].Any<string>() ? dictionary["g-recaptcha-response"][0] : throw new InvalidOperationException("Could not retrieve the reCAPTCHA response from the non-field form values stored on the HttpContext.");
      }
      return recaptchaResponse;
    }
  }
}
