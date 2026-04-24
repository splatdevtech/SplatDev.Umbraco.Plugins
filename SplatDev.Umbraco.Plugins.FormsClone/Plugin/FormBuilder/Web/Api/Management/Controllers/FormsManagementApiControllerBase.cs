using FormBuilder.Extension.Forms.Web.Management;

using Microsoft.AspNetCore.Mvc;

using System.Linq.Expressions;

using Umbraco.Cms.Api.Common.Attributes;
using Umbraco.Cms.Api.Common.Builders;
using Umbraco.Cms.Api.Common.Filters;

namespace FormBuilder.Web.Api.Management.Controllers
{
    /// <summary>
    /// Management API base controller for common functionality across all entities.
    /// </summary>
    [ApiController]
    [MapToApi("formBuilder-management")]
    [JsonOptionsName("BackOffice")]
    public abstract class FormsManagementApiControllerBase : Controller
    {
        /// <summary>
        /// Builds a         /// </summary>
        protected CreatedAtActionResult CreatedAtAction<T>(
          Expression<Func<T, string>> action,
          Guid id)
        {
            return CreatedAtAction(action, new
            {
                id
            });
        }

        /// <summary>
        /// Builds a         /// </summary>
        protected CreatedAtActionResult CreatedAtAction<T>(
          Expression<Func<T, string>> action,
          object? routeValues)
        {
            if (action.Body is not ConstantExpression body)
                throw new ArgumentException("Expression must be a constant expression.");
            string controllerName = ManagementApiRegexes.ControllerTypeToNameRegex().Replace(typeof(T).Name, string.Empty);
            return CreatedAtAction(body.Value?.ToString() ?? throw new ArgumentException("Expression does not have a value."), controllerName, routeValues, null);
        }

        /// <summary>
        /// Builds a         /// </summary>
        protected static ProblemDetails BuildSettingsValidationProblemDetails(
          Exception exception)
        {
            return BuildSettingsValidationProblemDetails(
      [
        exception
      ]);
        }

        /// <summary>
        /// Builds a         /// </summary>
        protected static ProblemDetails BuildSettingsValidationProblemDetails(
          List<Exception> exceptions)
        {
            return new ProblemDetailsBuilder().WithTitle("Settings validation failed.").WithDetail(string.Join("|", exceptions.Select(x => x.Message))).Build();
        }
    }
}