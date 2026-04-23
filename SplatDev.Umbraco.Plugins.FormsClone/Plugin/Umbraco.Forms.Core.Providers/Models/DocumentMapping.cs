
// Type: Umbraco.Forms.Core.Providers.Models.DocumentMapping
// Assembly: Umbraco.Forms.Core.Providers, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 747CF8D1-007A-4431-9ECE-D9510ED65D68

using System.Runtime.Serialization;
using System.Text.Json.Serialization;


#nullable enable
namespace Umbraco.Forms.Core.Providers.Models
{
  [DataContract(Name = "documentMapping")]
  public class DocumentMapping
  {
    [DataMember(Name = "id")]
    [JsonPropertyName("id")]
    public string Alias { get; set; } = string.Empty;

    [DataMember(Name = "field")]
    public string Field { get; set; } = string.Empty;

    [DataMember(Name = "staticValue")]
    public string StaticValue { get; set; } = string.Empty;

    public bool HasValue => !string.IsNullOrEmpty(this.Field) || !string.IsNullOrEmpty(this.StaticValue);
  }
}
