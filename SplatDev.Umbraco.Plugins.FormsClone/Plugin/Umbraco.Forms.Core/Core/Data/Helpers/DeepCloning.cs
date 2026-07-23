
// Type: Umbraco.Forms.Core.Data.Helpers.DeepCloning
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using System.Text.Json;


#nullable enable
namespace Umbraco.Forms.Core.Data.Helpers
{
  public static class DeepCloning
  {
    public static T? Clone<T>(this T obj) => JsonSerializer.Deserialize<T>(JsonSerializer.Serialize<T>(obj));
  }
}
