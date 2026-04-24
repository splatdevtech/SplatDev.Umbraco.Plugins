
// Type: Umbraco.Forms.Web.Attributes.UmbracoFormsDeliveryApiControllerAttribute
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Formatters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;


#nullable enable
namespace Umbraco.Forms.Web.Attributes
{
  public class UmbracoFormsDeliveryApiControllerAttribute : ActionFilterAttribute
  {
    public override void OnActionExecuted(ActionExecutedContext ctx)
    {
      if (!(ctx.Result is ObjectResult result))
        return;
      JsonSerializerOptions serializerOptions = new JsonSerializerOptions();
      serializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
      serializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
      serializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
      DefaultJsonTypeInfoResolver typeInfoResolver = new DefaultJsonTypeInfoResolver();
      typeInfoResolver.Modifiers.Add(UmbracoFormsDeliveryApiControllerAttribute.AlphabetizeProperties());
      serializerOptions.TypeInfoResolver = (IJsonTypeInfoResolver) typeInfoResolver;
      JsonSerializerOptions jsonSerializerOptions = serializerOptions;
      jsonSerializerOptions.Converters.Add((JsonConverter) new JsonStringEnumConverter());
      result.Formatters.Add((IOutputFormatter) new SystemTextJsonOutputFormatter(jsonSerializerOptions));
    }

    private static Action<JsonTypeInfo> AlphabetizeProperties(Type type) => (Action<JsonTypeInfo>) (typeInfo =>
    {
      if (typeInfo.Kind != JsonTypeInfoKind.Object || !type.IsAssignableFrom(typeInfo.Type))
        return;
      UmbracoFormsDeliveryApiControllerAttribute.AlphabetizeProperties()(typeInfo);
    });

    private static Action<JsonTypeInfo> AlphabetizeProperties() => (Action<JsonTypeInfo>) (typeInfo =>
    {
      if (typeInfo.Kind != JsonTypeInfoKind.Object)
        return;
      List<JsonPropertyInfo> list = typeInfo.Properties.OrderBy<JsonPropertyInfo, string>((Func<JsonPropertyInfo, string>) (p => p.Name), (IComparer<string>) StringComparer.Ordinal).ToList<JsonPropertyInfo>();
      typeInfo.Properties.Clear();
      for (int index = 0; index < list.Count; ++index)
      {
        list[index].Order = index;
        typeInfo.Properties.Add(list[index]);
      }
    });
  }
}
