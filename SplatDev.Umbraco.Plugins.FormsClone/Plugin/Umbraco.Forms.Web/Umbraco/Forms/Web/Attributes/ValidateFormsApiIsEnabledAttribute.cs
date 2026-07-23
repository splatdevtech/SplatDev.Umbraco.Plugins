
// Type: Umbraco.Forms.Web.Attributes.ValidateFormsApiIsEnabledAttribute
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using Umbraco.Forms.Core.Configuration;


#nullable enable
namespace Umbraco.Forms.Web.Attributes
{
  public class ValidateFormsApiIsEnabledAttribute : TypeFilterAttribute
  {
    public ValidateFormsApiIsEnabledAttribute()
      : base(typeof (ValidateFormsApiIsEnabledAttribute.ValidateFormsApiIsEnabledFilter))
    {
    }

    private sealed class ValidateFormsApiIsEnabledFilter : ValidateFormsApiFilterBase
    {
      private readonly PackageOptionSettings _config;

      public ValidateFormsApiIsEnabledFilter(IOptions<PackageOptionSettings> config) => this._config = config.Value;

      public override void OnAuthorization(AuthorizationFilterContext context)
      {
        if (this._config.EnableFormsApi)
          return;
        ValidateFormsApiFilterBase.SetForbiddenResult(context, "The Forms API is not enabled.");
      }
    }
  }
}
