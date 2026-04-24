
// Type: Umbraco.Forms.Web.Api.ManagementApi.FormsManagementApiControllerBase
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using Microsoft.AspNetCore.Mvc;

using System.Linq.Expressions;

using Umbraco.Cms.Api.Common.Attributes;
using Umbraco.Cms.Api.Common.Builders;
using Umbraco.Cms.Api.Common.Filters;
using Umbraco.Forms.Web.Api.Controllers.ManagementApi;


#nullable enable
namespace Umbraco.Forms.Web.Api.ManagementApi
{
    [ApiController]
    [MapToApi("forms-management")]
    [JsonOptionsName("BackOffice")]
    public abstract class FormsManagementApiControllerBase : Controller
    {
        protected CreatedAtActionResult CreatedAtAction<T>(
          Expression<Func<T, string>> action,
          Guid id)
        {
            return this.CreatedAtAction<T>(action, new
            {
                id = id
            });
        }

        protected CreatedAtActionResult CreatedAtAction<T>(
          Expression<Func<T, string>> action,
          object routeValues)
        {
            if (!(action.Body is ConstantExpression body))
                throw new ArgumentException("Expression must be a constant expression.");
            string controllerName = new System.Text.RegularExpressions.Regex(ManagementApiRegexes._controllerTypeToNameRegex).Replace(typeof(T).Name, string.Empty);
            return this.CreatedAtAction(body.Value?.ToString() ?? throw new ArgumentException("Expression does not have a value."), controllerName, routeValues, null);
        }

        protected static ProblemDetails BuildSettingsValidationProblemDetails(
          Exception exception)
        {
            return FormsManagementApiControllerBase.BuildSettingsValidationProblemDetails(new List<Exception>()
      {
        exception
      });
        }

        protected static ProblemDetails BuildSettingsValidationProblemDetails(
          List<Exception> exceptions)
        {
            return new ProblemDetailsBuilder().WithTitle("Settings validation failed.").WithDetail(string.Join("|", exceptions.Select<Exception, string>(x => x.Message))).Build();
        }
    }
}
