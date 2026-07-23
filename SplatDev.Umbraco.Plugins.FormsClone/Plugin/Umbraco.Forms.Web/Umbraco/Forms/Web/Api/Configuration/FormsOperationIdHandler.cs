
// Type: Umbraco.Forms.Web.Api.Configuration.FormsOperationIdHandler
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using Asp.Versioning;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.Options;
using Umbraco.Cms.Api.Common.OpenApi;


#nullable enable
namespace Umbraco.Forms.Web.Api.Configuration
{
  internal class FormsOperationIdHandler : OperationIdHandler
  {
    public FormsOperationIdHandler(
      IOptions<ApiVersioningOptions> apiVersioningOptions)
      : base(apiVersioningOptions)
    {
    }

    protected override bool CanHandle(
      ApiDescription apiDescription,
      ControllerActionDescriptor controllerActionDescriptor)
    {
      return controllerActionDescriptor.ControllerTypeInfo.Namespace?.StartsWith("Umbraco.Forms.Web.Api.ManagementApi") ?? false;
    }
  }
}
