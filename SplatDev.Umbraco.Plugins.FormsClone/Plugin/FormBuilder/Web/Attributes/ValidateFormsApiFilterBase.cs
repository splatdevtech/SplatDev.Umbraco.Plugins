using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace FormBuilder.Web.Attributes
{
    /// <summary>
    /// Base class for authorization filters controlling access to the Forms API.
    /// </summary>
    public abstract class ValidateFormsApiFilterBase : IAuthorizationFilter, IFilterMetadata
    {
        /// <inheritdoc />
        public abstract void OnAuthorization(AuthorizationFilterContext context);

        /// <summary>Sets a 404 (Forbidden) result.</summary>
        /// <param name="context">The         /// <param name="message">The message.</param>
        protected static void SetForbiddenResult(AuthorizationFilterContext context, string message)
        {
            ProblemDetails problemDetails = new()
            {
                Status = new int?(403),
                Type = "https://httpstatuses.io/403",
                Title = "Forbidden",
                Detail = message
            };
            AuthorizationFilterContext authorizationFilterContext = context;
            ObjectResult objectResult = new(problemDetails);
            objectResult.ContentTypes.Add("application/problem+json");
            objectResult.StatusCode = new int?(403);
            authorizationFilterContext.Result = objectResult;
        }
    }
}