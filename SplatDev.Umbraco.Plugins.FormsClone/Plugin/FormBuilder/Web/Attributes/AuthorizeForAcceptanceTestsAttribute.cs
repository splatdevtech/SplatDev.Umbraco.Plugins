using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

using Umbraco.Extensions;

namespace FormBuilder.Web.Attributes
{
    internal class AuthorizeForAcceptanceTestsAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!IsLocalRequest(context) && !AcceptanceTestEnvironmentVariableExists())
                context.Result = new UnauthorizedResult();
            else
                base.OnActionExecuting(context);
        }

        private static bool IsLocalRequest(ActionExecutingContext context) => context.HttpContext.Request.IsLocal();

        private static bool AcceptanceTestEnvironmentVariableExists() => !string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("UMBRACO_ACCEPTANCE_TEST_EXECUTION"));
    }
}