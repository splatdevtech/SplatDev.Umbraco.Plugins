using FormBuilder.Web.Attributes;

using Microsoft.OpenApi.Models;

using Swashbuckle.AspNetCore.SwaggerGen;

namespace FormBuilder.Core.Filters
{
    internal class SwaggerParameterAttributeFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (context.MethodInfo.DeclaringType is null)
                return;
            foreach (SwaggerParameterAttribute parameterAttribute in context.MethodInfo.DeclaringType.GetCustomAttributes(true).Union(context.MethodInfo.GetCustomAttributes(true)).OfType<SwaggerParameterAttribute>())
            {
                SwaggerParameterAttribute attribute = parameterAttribute;
                OpenApiParameter? openApiParameter = operation.Parameters.SingleOrDefault(x => x.Name == attribute.Name);
                if (openApiParameter is not null)
                    openApiParameter.Description = attribute.Description;
            }
        }
    }
}