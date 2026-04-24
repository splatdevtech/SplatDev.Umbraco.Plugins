
// Type: Umbraco.Forms.Web.Api.Configuration.SwaggerParameterAttributeFilter
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Forms.Web.Attributes;


#nullable enable
namespace Umbraco.Forms.Web.Api.Configuration
{
  internal class SwaggerParameterAttributeFilter : IOperationFilter
  {
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
      if (context.MethodInfo.DeclaringType == (Type) null)
        return;
      foreach (SwaggerParameterAttribute parameterAttribute in ((IEnumerable<object>) context.MethodInfo.DeclaringType.GetCustomAttributes(true)).Union<object>((IEnumerable<object>) context.MethodInfo.GetCustomAttributes(true)).OfType<SwaggerParameterAttribute>())
      {
        SwaggerParameterAttribute attribute = parameterAttribute;
        OpenApiParameter openApiParameter = ((IEnumerable<OpenApiParameter>) operation.Parameters).SingleOrDefault<OpenApiParameter>((Func<OpenApiParameter, bool>) (x => x.Name == attribute.Name));
        if (openApiParameter != null)
          openApiParameter.Description = attribute.Description;
      }
    }
  }
}
