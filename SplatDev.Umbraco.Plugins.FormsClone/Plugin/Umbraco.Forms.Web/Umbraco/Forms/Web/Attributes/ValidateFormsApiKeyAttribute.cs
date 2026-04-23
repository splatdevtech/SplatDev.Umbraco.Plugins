
// Type: Umbraco.Forms.Web.Attributes.ValidateFormsApiKeyAttribute
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Umbraco.Forms.Core.Configuration;


#nullable enable
namespace Umbraco.Forms.Web.Attributes
{
  public class ValidateFormsApiKeyAttribute : TypeFilterAttribute
  {
    public ValidateFormsApiKeyAttribute()
      : base(typeof (ValidateFormsApiKeyAttribute.ValidateFormsApiKeyFilter))
    {
    }

    private sealed class ValidateFormsApiKeyFilter : ValidateFormsApiFilterBase
    {
      private const string HeaderKey = "Api-Key";
      private readonly SecuritySettings _config;

      public ValidateFormsApiKeyFilter(IOptions<SecuritySettings> config) => this._config = config.Value;

      public override void OnAuthorization(AuthorizationFilterContext context)
      {
        if (string.IsNullOrEmpty(this._config.FormsApiKey))
          return;
        StringValues stringValues;
        if (!context.HttpContext.Request.Headers.TryGetValue("Api-Key", out stringValues))
        {
          ValidateFormsApiFilterBase.SetForbiddenResult(context, "An API key has been configured for the Umbraco Forms API but no key was found in the request's 'Api-Key' header.");
        }
        else
        {
          if (!(stringValues != this._config.FormsApiKey))
            return;
          ValidateFormsApiFilterBase.SetForbiddenResult(context, "An API key has been configured for the Umbraco Forms API but the value provided in the request's 'Api-Key' header does not match.");
        }
      }
    }
  }
}
