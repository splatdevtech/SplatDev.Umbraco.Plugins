
// Type: Umbraco.Forms.Web.Models.Backoffice.SettingWithValue
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;


#nullable enable
namespace Umbraco.Forms.Web.Models.Backoffice
{
  [DataContract(Name = "setting")]
  [Serializable]
  public class SettingWithValue
  {
    [DataMember(Name = "name")]
    public string Name { get; set; } = string.Empty;

    [DataMember(Name = "alias")]
    public string Alias { get; set; } = string.Empty;

    [DataMember(Name = "description")]
    public string Description { get; set; } = string.Empty;

    [DataMember(Name = "prevalues")]
    public IEnumerable<string> Prevalues { get; set; } = Enumerable.Empty<string>();

    [DataMember(Name = "view")]
    public string View { get; set; } = string.Empty;

    [DataMember(Name = "value")]
    public string Value { get; set; } = string.Empty;

    [DataMember(Name = "displayOrder")]
    public int DisplayOrder { get; set; }

    [DataMember(Name = "isReadOnly")]
    public bool IsReadOnly { get; set; }

    [DataMember(Name = "isMandatory")]
    public bool IsMandatory { get; set; }
  }
}
