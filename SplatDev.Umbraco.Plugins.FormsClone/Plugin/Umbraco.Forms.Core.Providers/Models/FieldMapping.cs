
// Type: Umbraco.Forms.Core.Providers.Models.FieldMapping
// Assembly: Umbraco.Forms.Core.Providers, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 747CF8D1-007A-4431-9ECE-D9510ED65D68

using System.Runtime.Serialization;


#nullable enable
namespace Umbraco.Forms.Core.Providers.Models
{
  [DataContract(Name = "fieldMapping")]
  public class FieldMapping
  {
    [DataMember(Name = "alias")]
    public string Alias { get; set; } = string.Empty;

    [DataMember(Name = "value")]
    public string Value { get; set; } = string.Empty;

    [DataMember(Name = "staticValue")]
    public string StaticValue { get; set; } = string.Empty;
  }
}
