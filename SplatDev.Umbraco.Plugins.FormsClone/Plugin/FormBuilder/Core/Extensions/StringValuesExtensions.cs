using Microsoft.Extensions.Primitives;

namespace FormBuilder.Core.Extensions
{
    internal static class StringValuesExtensions
    {
        public static object[] ToObjectArray(this StringValues stringValues) => [.. stringValues.Where(x => x != null)!.Select<string, object>(x => x)!];
    }
}