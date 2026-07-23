using Asp.Versioning;

using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.Options;

using Umbraco.Cms.Api.Common.OpenApi;

namespace FormBuilder.Core.Handlers
{
    internal class FormsOperationIdHandler(
      IOptions<ApiVersioningOptions> apiVersioningOptions) : OperationIdHandler(apiVersioningOptions)
    {
        protected override bool CanHandle(
          ApiDescription apiDescription,
          ControllerActionDescriptor controllerActionDescriptor)
        {
            return controllerActionDescriptor.ControllerTypeInfo.Namespace?.StartsWith("FormBuilder.Web.Api.Management") ?? false;
        }
    }
}