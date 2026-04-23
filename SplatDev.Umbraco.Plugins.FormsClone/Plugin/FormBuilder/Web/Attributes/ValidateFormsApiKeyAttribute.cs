using FormBuilder.Core.Configuration;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;

namespace FormBuilder.Web.Attributes
{
    /// <summary>
    /// Attribute attached to Forms API endpoints indicating that the API key provided in a Forms API request should be validated.
    /// </summary>
    public class ValidateFormsApiKeyAttribute : TypeFilterAttribute
    {
        /// <summary>
        /// Initializes a new instance of the         /// </summary>
        public ValidateFormsApiKeyAttribute()
          : base(typeof(ValidateFormsApiKeyFilter))
        {
        }

        private sealed class ValidateFormsApiKeyFilter(IOptions<SecuritySettings> config) : ValidateFormsApiFilterBase
        {
            private const string HeaderKey = "Api-Key";
            private readonly SecuritySettings _config = config.Value;

            public override void OnAuthorization(AuthorizationFilterContext context)
            {
                if (string.IsNullOrEmpty(_config.FormsApiKey))
                    return;
                if (!context.HttpContext.Request.Headers.TryGetValue("Api-Key", out StringValues stringValues))
                {
                    SetForbiddenResult(context, "An API key has been configured for the Umbraco Forms API but no key was found in the request's 'Api-Key' header.");
                }
                else
                {
                    if (!(stringValues != _config.FormsApiKey))
                        return;
                    SetForbiddenResult(context, "An API key has been configured for the Umbraco Forms API but the value provided in the request's 'Api-Key' header does not match.");
                }
            }
        }
    }
}