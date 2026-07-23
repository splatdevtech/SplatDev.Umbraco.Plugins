
// Type: Umbraco.Forms.Core.Models.PreValue
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using System.Runtime.Serialization;


#nullable enable
namespace Umbraco.Forms.Core.Models
{
  [DataContract(Name = "preValue")]
  public class PreValue
  {
    [DataMember(Name = "id")]
    public string Id { get; set; } = string.Empty;

    [DataMember(Name = "value")]
    public string Value { get; set; } = string.Empty;

    [DataMember(Name = "caption")]
    public string? Caption { get; set; }

    [DataMember(Name = "sortOrder")]
    public int SortOrder { get; set; }
  }
}
