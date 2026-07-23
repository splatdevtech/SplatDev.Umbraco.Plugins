
// Type: Umbraco.Forms.Web.Attributes.ValidateFormsApiFilterBase
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;


#nullable enable
namespace Umbraco.Forms.Web.Attributes
{
  public abstract class ValidateFormsApiFilterBase : IAuthorizationFilter, IFilterMetadata
  {
    public abstract void OnAuthorization(AuthorizationFilterContext context);

    protected static void SetForbiddenResult(AuthorizationFilterContext context, string message)
    {
      ProblemDetails problemDetails = new ProblemDetails()
      {
        Status = new int?(403),
        Type = "https://httpstatuses.io/403",
        Title = "Forbidden",
        Detail = message
      };
      AuthorizationFilterContext authorizationFilterContext = context;
      ObjectResult objectResult = new ObjectResult((object) problemDetails);
      objectResult.ContentTypes.Add("application/problem+json");
      objectResult.StatusCode = new int?(403);
      authorizationFilterContext.Result = (IActionResult) objectResult;
    }
  }
}
