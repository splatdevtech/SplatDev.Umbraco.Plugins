
// Type: Umbraco.Forms.Core.Providers.Models.StandardFieldMapping
// Assembly: Umbraco.Forms.Core.Providers, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 747CF8D1-007A-4431-9ECE-D9510ED65D68

using System.Runtime.Serialization;


#nullable enable
namespace Umbraco.Forms.Core.Providers.Models
{
  [DataContract(Name = "standardFieldMapping")]
  public class StandardFieldMapping
  {
    [DataMember(Name = "field")]
    public StandardField Field { get; set; }

    [DataMember(Name = "include")]
    public bool Include { get; set; }

    [DataMember(Name = "keyName")]
    public string KeyName { get; set; } = string.Empty;
  }
}
