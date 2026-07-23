
// Type: Umbraco.Forms.Web.Attributes.AuthorizeForAcceptanceTestsAttribute
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using Umbraco.Extensions;


#nullable enable
namespace Umbraco.Forms.Web.Attributes
{
  internal class AuthorizeForAcceptanceTestsAttribute : ActionFilterAttribute
  {
    public override void OnActionExecuting(ActionExecutingContext context)
    {
      if (!this.IsLocalRequest(context) && !this.AcceptanceTestEnvironmentVariableExists())
        context.Result = (IActionResult) new UnauthorizedResult();
      else
        OnActionExecuting(context);
    }

    private bool IsLocalRequest(ActionExecutingContext context) => HttpRequestExtensions.IsLocal(context.HttpContext.Request);

    private bool AcceptanceTestEnvironmentVariableExists() => !string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("UMBRACO_ACCEPTANCE_TEST_EXECUTION"));
  }
}
