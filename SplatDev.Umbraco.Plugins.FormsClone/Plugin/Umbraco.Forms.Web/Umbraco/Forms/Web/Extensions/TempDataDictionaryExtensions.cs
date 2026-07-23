
// Type: Umbraco.Forms.Web.Extensions.TempDataDictionaryExtensions
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.Text.Json;
using Umbraco.Forms.Core;


#nullable enable
namespace Umbraco.Forms.Web.Extensions
{
  public static class TempDataDictionaryExtensions
  {
    public static void Put<T>(this ITempDataDictionary tempData, string key, T value) where T : class => tempData[key] = (object) JsonSerializer.Serialize<T>(value, FormsJsonSerializerOptions.Default);

    public static T? Get<T>(this ITempDataDictionary tempData, string key) where T : class
    {
      object json;
      tempData.TryGetValue(key, out json);
      return json != null ? JsonSerializer.Deserialize<T>((string) json) : default (T);
    }
  }
}
