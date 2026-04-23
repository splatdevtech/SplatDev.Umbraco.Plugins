using FormBuilder.Core.Configuration;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;

namespace FormBuilder.Web.Attributes
{
    /// <summary>
    /// Attribute attached to Forms API endpoints ensuring that the API is enabled in configuration.
    /// </summary>
    public class ValidateFormsApiIsEnabledAttribute : TypeFilterAttribute
    {
        /// <summary>
        /// Initializes a new instance of the         /// </summary>
        public ValidateFormsApiIsEnabledAttribute()
          : base(typeof(ValidateFormsApiIsEnabledFilter))
        {
        }

        private sealed class ValidateFormsApiIsEnabledFilter : ValidateFormsApiFilterBase
        {
            private readonly PackageOptionSettings _config;

            public ValidateFormsApiIsEnabledFilter(IOptions<PackageOptionSettings> config) => _config = config.Value;

            public override void OnAuthorization(AuthorizationFilterContext context)
            {
                if (_config.EnableFormsApi)
                    return;
                SetForbiddenResult(context, "The Forms API is not enabled.");
            }
        }
    }
}