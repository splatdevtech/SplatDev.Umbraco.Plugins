
// Type: Umbraco.Forms.Core.Interfaces.IValidationPattern
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using System.Text.Json.Serialization;


#nullable enable
namespace Umbraco.Forms.Core.Interfaces
{
  public interface IValidationPattern
  {
    [JsonPropertyName("alias")]
    string Alias { get; }

    [JsonPropertyName("labelKey")]
    string LabelKey { get; }

    [JsonPropertyName("name")]
    string Name { get; }

    [JsonPropertyName("pattern")]
    string Pattern { get; }

    [JsonPropertyName("readOnly")]
    bool ReadOnly { get; }
  }
}
