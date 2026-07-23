using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Formatters;

using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace FormBuilder.Web.Attributes
{
    /// <summary>
    /// Defines an action filter attribute for use on Umbraco Forms Delivery API endpoints to set serialization rules.
    /// </summary>
    public class FormBuilderDeliveryApiControllerAttribute : ActionFilterAttribute
    {
        /// <inheritdoc />
        public override void OnActionExecuted(ActionExecutedContext ctx)
        {
            if (ctx.Result is not ObjectResult result)
                return;
            JsonSerializerOptions serializerOptions = new()
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            DefaultJsonTypeInfoResolver typeInfoResolver = new();
            typeInfoResolver.Modifiers.Add(AlphabetizeProperties());
            serializerOptions.TypeInfoResolver = typeInfoResolver;
            JsonSerializerOptions jsonSerializerOptions = serializerOptions;
            jsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            result.Formatters.Add(new SystemTextJsonOutputFormatter(jsonSerializerOptions));
        }

        private static Action<JsonTypeInfo> AlphabetizeProperties(Type type) => typeInfo =>
        {
            if (typeInfo.Kind != JsonTypeInfoKind.Object || !type.IsAssignableFrom(typeInfo.Type))
                return;
            AlphabetizeProperties()(typeInfo);
        };

        private static Action<JsonTypeInfo> AlphabetizeProperties() => typeInfo =>
        {
            if (typeInfo.Kind != JsonTypeInfoKind.Object)
                return;
            List<JsonPropertyInfo> list = [.. typeInfo.Properties.OrderBy(p => p.Name, StringComparer.Ordinal)];
            typeInfo.Properties.Clear();
            for (int index = 0; index < list.Count; ++index)
            {
                list[index].Order = index;
                typeInfo.Properties.Add(list[index]);
            }
        };
    }
}