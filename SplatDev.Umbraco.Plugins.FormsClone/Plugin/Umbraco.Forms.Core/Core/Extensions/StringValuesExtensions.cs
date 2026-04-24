
// Type: Umbraco.Forms.Core.Extensions.StringValuesExtensions
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using Microsoft.Extensions.Primitives;


#nullable enable
namespace Umbraco.Forms.Core.Extensions
{
    public static class StringValuesExtensions
    {
        public static object[] ToObjectArray(this StringValues stringValues) => stringValues.Where<string>(x => x != null).Select<string, object>(x => x).ToArray<object>();
    }
}
