
// Type: Umbraco.Forms.Core.Models.RecordFieldDisplay
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using System.Runtime.Serialization;


#nullable enable
namespace Umbraco.Forms.Core.Models
{
  [DataContract(Name = "recordFieldDisplay")]
  public class RecordFieldDisplay
  {
    [DataMember(Name = "alias")]
    public string Alias { get; set; } = string.Empty;

    [DataMember(Name = "caption")]
    public string Caption { get; set; } = string.Empty;

    [DataMember(Name = "isSystem")]
    public bool IsSystem { get; set; }
  }
}
