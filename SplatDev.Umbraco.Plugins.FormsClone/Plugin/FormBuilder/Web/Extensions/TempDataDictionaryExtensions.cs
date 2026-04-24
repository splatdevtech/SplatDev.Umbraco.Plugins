using FormBuilder.Core.Options;

using Microsoft.AspNetCore.Mvc.ViewFeatures;

using System.Text.Json;

namespace FormBuilder.Web.Extensions
{
    /// <summary>
    /// Defines extension methods for     /// data into TempData.
    /// </summary>
    /// <remarks>
    /// Hat-tip: https://www.blakepell.com/asp-net-core-storing-objects-in-tempdata
    /// </remarks>
    public static class TempDataDictionaryExtensions
    {
        /// <summary>
        /// Puts an object into the TempData by first serializing it to JSON.
        /// </summary>
        public static void Put<T>(this ITempDataDictionary tempData, string key, T value) where T : class => tempData[key] = JsonSerializer.Serialize(value, FormsJsonSerializerOptions.Default);

        /// <summary>
        /// Gets an object from the TempData by deserializing it from JSON.
        /// </summary>
        public static T? Get<T>(this ITempDataDictionary tempData, string key) where T : class
        {
            tempData.TryGetValue(key, out object? json);
            return json is not null ? JsonSerializer.Deserialize<T>((string)json) : default;
        }
    }
}