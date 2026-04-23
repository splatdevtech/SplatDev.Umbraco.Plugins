using System.Text.Json;

namespace FormBuilder.Core.Helpers
{
    public static class DeepCloning
    {
        public static T? Clone<T>(this T obj) => JsonSerializer.Deserialize<T>(JsonSerializer.Serialize(obj));
    }
}