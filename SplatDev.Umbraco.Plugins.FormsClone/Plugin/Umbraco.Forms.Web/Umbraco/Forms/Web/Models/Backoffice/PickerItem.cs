
// Type: Umbraco.Forms.Web.Models.Backoffice.PickerItem
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using System;
using System.Runtime.Serialization;


#nullable enable
namespace Umbraco.Forms.Web.Models.Backoffice
{
  [DataContract(Name = "item")]
  [Serializable]
  public class PickerItem
  {
    [DataMember(Name = "id")]
    public string Id { get; set; } = string.Empty;

    [DataMember(Name = "value")]
    public string Value { get; set; } = string.Empty;
  }
}
