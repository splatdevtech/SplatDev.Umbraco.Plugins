using Microsoft.AspNetCore.Mvc.ViewFeatures;

using Newtonsoft.Json;

namespace SplatDev.Umbraco.Common.Extensions
{
    public static class TempDataExtensions
    {
        public static void PutJson<T>(this ITempDataDictionary tempData, string key, T value) where T : class
        {
            tempData[key] = JsonConvert.SerializeObject(value);
        }

        public static T? GetJson<T>(this ITempDataDictionary tempData, string key) where T : class
        {
            _ = tempData.TryGetValue(key, out var o);
            return o == null ? null : JsonConvert.DeserializeObject<T>((string)o);
        }
    }
}
