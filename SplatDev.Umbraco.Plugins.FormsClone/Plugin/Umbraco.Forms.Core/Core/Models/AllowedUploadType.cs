
// Type: Umbraco.Forms.Core.Models.AllowedUploadType
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using System.Runtime.Serialization;


#nullable enable
namespace Umbraco.Forms.Core.Models
{
  public class AllowedUploadType
  {
    [DataMember(Name = "type")]
    public string Type { get; set; } = string.Empty;

    [DataMember(Name = "name")]
    public string Name { get; set; } = string.Empty;

    [DataMember(Name = "checked")]
    public string Checked { get; set; } = string.Empty;
  }
}
