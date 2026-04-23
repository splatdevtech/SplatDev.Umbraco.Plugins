
// Type: Umbraco.Forms.Core.Models.FormTemplateBase
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using System.Runtime.Serialization;


#nullable enable
namespace Umbraco.Forms.Core.Models
{
  [DataContract(Name = "formTemplateBase")]
  public class FormTemplateBase
  {
    [DataMember(Name = "alias")]
    public string Alias { get; set; } = string.Empty;

    [DataMember(Name = "unique")]
    public string Unique => this.Alias;

    [DataMember(Name = "entityType")]
    public string EntityType => "form-template";

    [DataMember(Name = "name")]
    public string Name { get; set; } = string.Empty;

    [DataMember(Name = "description")]
    public string Description { get; set; } = string.Empty;
  }
}
